using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SharpMvt
{
    public interface IServiceResolver
    {
        Result Invoke(string assemblyName, string className, string methodName);
    }

    public interface IServiceFactory
    {
        object Create(Type serviceType, HttpRequestBase request);
    }    

    public class Result
    {
        public Result(object value, MethodInfo method)
        {
            Value = value;
            Method = method;
        }

        public object Value { get; }
        public MethodInfo Method { get; }
    }

    public class ServiceResolver : IServiceResolver
    {
        public ServiceResolver(Controller controller)
        {
            _controller = controller;
        }

        public ServiceResolver(Controller controller, IServiceFactory serviceFactory) : this(controller)
        {            
            _serviceFactory = serviceFactory;            
        }

        private class ServiceFactory : IServiceFactory
        {
            public object Create(Type serviceType, HttpRequestBase request)
            {
                return Activator.CreateInstance(serviceType, true);
            }
        }

        private Controller _controller;
        private IServiceFactory _serviceFactory = new ServiceFactory();

        public Result Invoke(string assemblyName, string className, string methodName)
        {
            string bodyText;
            _controller.Request.InputStream.Seek(0, SeekOrigin.Begin);
            using (StreamReader reader = new StreamReader(_controller.Request.InputStream))
            {
                bodyText = reader.ReadToEnd();
            }

            string dllDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");

            Type classType = GetReflectedType(dllDir, assemblyName, className);

            JObject jObject = JsonConvert.DeserializeObject(bodyText) as JObject;

            int constructorMetadataToken = GetMetadataToken("constructorMetadataToken", jObject);
            ConstructorInfo constructor = GetConstructor(dllDir, assemblyName, className, constructorMetadataToken);

            int methodMetadataToken = GetMetadataToken("methodMetadataToken", jObject);
            MethodInfo method = GetMethod(dllDir, classType, methodName, methodMetadataToken);

            object[] constuctorParameterValues = GetParameterValues(constructor.GetParameters(), "constructorParameterValues", jObject);
            object[] methodParameterValues = GetParameterValues(method.GetParameters(), "methodParameterValues", jObject);

            object instance = constructor.Invoke(constuctorParameterValues);
            object result = method.Invoke(instance, methodParameterValues);

            return new Result(result, method);
        }

        private int GetMetadataToken(string key, JObject jObject)
        {
            int metadataToken = 0;

            if (GetIsJsonRequest())
            {
                metadataToken = (int)GetValue(typeof(int), jObject[key]);
            }
            else
            {
                metadataToken = (int)GetValue(typeof(int), key);
            }

            return metadataToken;
        }

        private object[] GetParameterValues(ParameterInfo[] parameterInfos, string key, JObject jObject)
        {
            if (GetIsJsonRequest())
            {
                return GetValues(parameterInfos, jObject[key]);
            }
            else
            {
                return GetValues(parameterInfos, key);
            }
        }

        private object GetValue(Type type, JToken value)
        {
            if (value == null)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }

                return null;
            }
            else if (type.IsClass && type != typeof(string))
            {
                return JsonConvert.DeserializeObject(value.ToString(), type);
            }
            else
            {
                return Convert.ChangeType(value.ToString(), type);
            }
        }

        private object[] GetValues(ParameterInfo[] paramInfos, JToken jToken)
        {
            var valueList = new List<object>();

            int index = 0;
            foreach (var paramInfo in paramInfos)
            {
                object value;

                TypeScriptDiAttribute attr = Attribute.GetCustomAttribute(paramInfo, typeof(TypeScriptDiAttribute)) as TypeScriptDiAttribute;

                if (attr != null)
                {
                    value = _serviceFactory.Create(paramInfo.ParameterType, _controller.Request);
                }
                else
                {
                    value = GetValue(paramInfo.ParameterType, jToken[paramInfo.Name]);
                }

                valueList.Add(value);

                index++;
            }

            return valueList.ToArray();
        }

        private object GetValue(Type type, string prefix)
        {
            object valuebox = Activator.CreateInstance(typeof(ValueBox<>).MakeGenericType(type));

            FormCollection form = new FormCollection();

            System.Collections.Specialized.NameValueCollection collection = _controller.Request.HttpMethod == "POST" ? _controller.Request.Form : _controller.Request.QueryString;

            foreach (var key in collection.AllKeys)
            {
                if (key.StartsWith(prefix))
                {
                    string suffix = new string(key.Skip(prefix.Length).ToArray());

                    form[$"Value{suffix}"] = collection[key];
                }
            }

            BindModel(valuebox, string.Empty, form);

            return (valuebox as ValueBox).GetValue();
        }

        private object[] GetValues(System.Reflection.ParameterInfo[] parameters, string prefixName)
        {
            List<object> values = new List<object>();
            var index = 0;
            foreach (var param in parameters)
            {
                var prefix = $"{prefixName}.{param.Name}";
                object obj = null;

                SharpMvt.TypeScriptDiAttribute attr = Attribute.GetCustomAttribute(param, typeof(SharpMvt.TypeScriptDiAttribute)) as SharpMvt.TypeScriptDiAttribute;

                if (attr != null)
                {
                    obj = DependencyResolver.Current.GetService(param.ParameterType);
                }
                else if (param.ParameterType == typeof(HttpPostedFileBase))
                {
                    obj = _controller.Request.Files[prefix];
                }
                else if ((param.ParameterType.IsArray && param.ParameterType.GetElementType() == typeof(Stream))
                    || param.ParameterType == typeof(List<Stream>))
                {
                    List<Stream> files = new List<Stream>();
                    foreach (var key in _controller.Request.Files.AllKeys)
                    {
                        if (key.StartsWith(prefix))
                        {
                            files.Add(_controller.Request.Files[key].InputStream);
                        }
                    }

                    if (param.ParameterType.IsArray)
                    {
                        obj = files.ToArray();
                    }
                    else
                    {
                        obj = files;
                    }
                }
                else
                {
                    obj = GetValue(param.ParameterType, prefix);
                }

                values.Add(obj);

                index++;
            }

            return values.ToArray();
        }

        public Type GetReflectedType(string dllDir, string assembleyName, string className)
        {
            Assembly assembly = Assembly.LoadFrom(Path.Combine(dllDir, $"{assembleyName }.dll"));
            Type type = assembly.GetType(className);

            return type;
        }

        private List<Type> GetParmeterTypes(string assemblyDirectory, Type reflectedType, string[] parameterTypeTexts)
        {
            List<Type> parameterTypes = new List<Type>();

            if (parameterTypeTexts == null)
            {
                return parameterTypes;
            }

            for (int i = 0; i < parameterTypeTexts.Length; i++)
            {
                string parameterTypeText = parameterTypeTexts[i];

                Type parameterType = Type.GetType(parameterTypeText);

                if (parameterType == null)
                {
                    parameterType = reflectedType.Assembly.GetType(parameterTypeText);
                }

                if (parameterType == null)
                {
                    foreach (AssemblyName refAssemblyName in reflectedType.Assembly.GetReferencedAssemblies())
                    {
                        var refAssembly = Assembly.Load(refAssemblyName);

                        parameterType = refAssembly.GetType(parameterTypeText);

                        if (parameterType != null)
                        {
                            break;
                        }
                    }
                }

                parameterTypes.Add(parameterType);
            }

            return parameterTypes;
        }

        public ConstructorInfo GetConstructor(string dllDir, string assemblyName, string className, int metadataToken)
        {
            ConstructorInfo constructor;

            Type classType = GetReflectedType(dllDir, assemblyName, className);

            if (metadataToken == 0)
            {
                constructor = classType.GetConstructor(Type.EmptyTypes);
            }
            else
            {
                constructor = classType.GetConstructors().Where(x => x.MetadataToken == metadataToken).First();
            }

            return constructor;
        }

        public MethodInfo GetMethod(string dllDir, Type classType, string methodName, int metadataToken)
        {            
            MethodInfo method;            

            if (metadataToken == 0)
            {
                method = classType.GetMethod(methodName);
            }
            else
            {
                method = classType.GetMethods().Where(x => x.MetadataToken == metadataToken).First();
            }

            return method;
        }

        private abstract class ValueBox
        {
            public abstract object GetValue();
        }

        private class ValueBox<T> : ValueBox
        {
            public T Value { get; set; }

            public override object GetValue()
            {
                return Value;
            }
        }

        private bool GetIsJsonRequest()
        {
            if (_controller.Request == null)
            {
                throw new ArgumentNullException("request");
            }

            bool rtn = false;

            const string jsonMime = "application/json";

            if (_controller.Request.AcceptTypes != null)
            {
                rtn = _controller.Request.AcceptTypes.Any(t => t.Equals(jsonMime, StringComparison.OrdinalIgnoreCase));
            }

            return rtn || _controller.Request.ContentType.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Any(t => t.Equals(jsonMime, StringComparison.OrdinalIgnoreCase));
        }

        private void BindModel(object model, string prefix, IValueProvider valueProvider)
        {
            //TryUpdateModel<TModel>(TModel model, string prefix, IValueProvider valueProvider) where TModel : class;

            MethodInfo method = _controller.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(x =>
            {
                return x.Name == "TryUpdateModel"
                    && x.GetParameters().Length == 3
                    && x.GetParameters()[1].ParameterType == typeof(string)
                    && x.GetParameters()[2].ParameterType == typeof(IValueProvider);
            }).First().MakeGenericMethod(model.GetType());

            method.Invoke(_controller, new object[] { model, prefix, valueProvider });            
        }
    }
}
