using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Xml;
using Mustache;
using System.Web;

namespace SharpMvt
{
    public class TranspileOptions
    {
        public virtual string BaseDir { get; set; }
        public virtual string Project { get; set; }
        public virtual string DllDir { get; set; }
        public virtual PostHandler PostHandler { get; set; }
        public virtual string DestFile { get; set; }
    }

    public class PostHandler
    {
        public string Import { get; set; }
        public string Function { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TypeScriptAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TypeScriptMethodAttribute : Attribute
    {
        public bool Ignore { get; set; }
        public bool Link { get; set; }
        public bool Form { get; set; }
    }

    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class TypeScriptDiAttribute : Attribute
    {

    }

    public class State
    {
        public Dictionary<Type, ParameterTypeTranspilerBase> ParameterTypeTranspilerMap { get; set; } = new Dictionary<Type, ParameterTypeTranspilerBase>();
        public Dictionary<Type, EntryTypeTranspilerBase> EntryTypeTranspilerMap { get; set; } = new Dictionary<Type, EntryTypeTranspilerBase>();

        public bool IsArray(Type type, out Type elementType)
        {
            elementType = null;

            if (type == typeof(string))
            {
                return false;
            }

            foreach (var contract in type.GetInterfaces())
            {
                if (contract == typeof(IDictionary) || contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    return false;
                }
                else if (contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    elementType = contract.GetGenericArguments()[0];
                    return true;
                }
            }

            return false;
        }
    }

    public abstract class TypeTranspilerBase
    {
        public Type Type { get; set; }        
        public string TsType { get; set; }

        public string TsShortType {  get { return TsType.Split('.').Last(); } }

        public abstract string GetTypeCode(TranspileOptions options, State state);

    }

    public abstract class EntryTypeTranspilerBase : TypeTranspilerBase
    {
        
    }

    public class EntryTypeTranspiler : EntryTypeTranspilerBase
    {
        private class InvokePrividerCollection
        {
            public List<InvokeProvider> InvokeProviders { get; set; } = new List<InvokeProvider>();
        }

        private class InvokeProvider
        {
            public Type Type { get; set; }
            private string _methodName;
            public string MethodName
            {
                get
                {
                    return _methodName;
                }
                set
                {
                    Invokers.ForEach(x => x.MethodName = value);

                    _methodName = value;
                }
            }            
            //public string ReturnTsType { get; set; }
            public bool HasOverloads
            {
                get
                {
                    return Invokers.Count() > 1;
                }
            }
            public List<Invoker> Invokers { get; set; }
            
            public List<Invoker> ReverseInvokers
            {
                get
                {
                    var reverse = new List<Invoker>(Invokers);
                    reverse.Reverse();

                    return reverse;

                }
            }
            public Invoker LastInvoker
            {
                get
                {
                    return Invokers.Last();
                }
            }

            public string Method { get; set; }
            //public string ContentType { get; set; }
            //public string ResultType { get; set; }
            public string AssemblyName { get; set; }
            public string ClassName { get; set; }

            public bool NotExistsNoParamInvoker
            {
                get { return !Invokers.Any(x => x.Parameters.Count == 0); }
            }
        }

        private class Invoker
        {
            public MethodBase Method { get; set; }
            public string MethodName { get; set; }            
            public List<Parameter> Parameters { get; set; } = new List<Parameter>();                 
            public string ReturnTsType { get; set; }
            public string ResultType { get; set; }
            public string ContentType { get; set; }
        }

        private class Parameter
        {
            public string TsType { get; set; }
            public Type Type { get; set; }
            public string Validation { get; set; }
            public string Index { get; set; }
            public string Name { get; set; }
            public bool IsDI { get; set; }
            public bool IsLastParam { get; set; }
        }

        private List<Invoker> GetInvorkers(MethodBase[] methods, TranspileOptions options, State state)
        {
            List<Invoker> invokers = new List<Invoker>();

            foreach (var method in methods)
            {
                string returnTsType = string.Empty;
                string contentType = string.Empty;
                string resultType = string.Empty;

                if (method is MethodInfo methodInfo)
                {
                    if (state.IsArray(methodInfo.ReturnType, out Type elementType))
                    {
                        returnTsType = $"{state.ParameterTypeTranspilerMap[elementType].TsType}[]";
                    }
                    else
                    {
                        returnTsType = state.ParameterTypeTranspilerMap[methodInfo.ReturnType].TsType;
                    }

                    contentType = "json";
                    resultType = methodInfo.ReturnType == typeof(Stream) || methodInfo.ReturnType.IsSubclassOf(typeof(Stream)) ? "content" : "json";

                    if (Attribute.GetCustomAttribute(method, typeof(TypeScriptMethodAttribute)) is TypeScriptMethodAttribute attr)
                    {
                        if (attr.Link)
                        {
                            returnTsType = "string";
                        }

                        if (attr.Form)
                        {
                            contentType = "form";
                        }

                        if (attr.Link)
                        {
                            resultType = "link";
                        }
                    }
                }

                if (method.GetParameters().Length == 0)
                {
                    invokers.Add(new Invoker() { Method = method, MethodName = method.Name, ReturnTsType = returnTsType, ContentType = contentType, ResultType = resultType });
                }
                else
                {
                    List<Parameter> parameters = new List<Parameter>();

                    int index = 0;
                    foreach (var parameter in method.GetParameters())
                    {
                        Parameter item = null;

                        if (Attribute.GetCustomAttribute(parameter, typeof(TypeScriptDiAttribute)) == null)
                        {   
                            string tsType = string.Empty;
                            string validation = string.Empty;

                            if (state.IsArray(parameter.ParameterType, out Type elementType))
                            {
                                tsType = $"{state.ParameterTypeTranspilerMap[elementType].TsType}[]";
                                validation = $"Array.isArray(arguments[{index.ToString()}])";
                            }
                            else
                            {
                                tsType = state.ParameterTypeTranspilerMap[parameter.ParameterType].TsType;
                                validation = state.ParameterTypeTranspilerMap[parameter.ParameterType].GetValidationCode(options, $"arguments[{index.ToString()}]");
                            }

                            item = new Parameter
                            {
                                TsType = tsType,
                                Type = parameter.ParameterType,
                                Validation = validation,
                                Index = index.ToString(),
                                Name = parameter.Name,
                                IsDI = Attribute.GetCustomAttribute(parameter, typeof(TypeScriptDiAttribute)) != null
                            };

                            index++;
                        }
                        else
                        {
                            item = new Parameter
                            {
                                Type = parameter.ParameterType,
                                Name = parameter.Name,
                                IsDI = Attribute.GetCustomAttribute(parameter, typeof(TypeScriptDiAttribute)) != null
                            };
                        }

                        parameters.Add(item);                        
                    }

                    if (!(parameters.Count == 1 && parameters.First().IsDI))
                    {
                        parameters.Last().IsLastParam = true;                        

                        invokers.Add(new Invoker { Parameters = parameters, Method = method, MethodName = method.Name, ReturnTsType = returnTsType, ContentType = contentType, ResultType = resultType });
                    }
                }
            }            

            return invokers.OrderBy(x => x.Parameters.Count).ToList();
        }

        public override string GetTypeCode(TranspileOptions options, State state)
        {
            List<Invoker> constructors = GetInvorkers(Type.GetConstructors(), options, state);            

            var constructorProvider = new InvokeProvider
            {
                Type = Type,
                Invokers = constructors,
                MethodName = "constructor"             
            };

            InvokePrividerCollection methodProviderCollection = new InvokePrividerCollection();            

            foreach (var methodGroup in Type.GetMethods().Where(x => x.DeclaringType != typeof(object)).GroupBy(x => x.Name))
            {
                List<Invoker> overloads = GetInvorkers(methodGroup.ToArray(), options, state);
                
                string tsType = string.Empty;                

                if (state.IsArray(methodGroup.First().ReturnType, out Type elementType))
                {
                    tsType = $"{state.ParameterTypeTranspilerMap[elementType].TsType}[]";                    
                }
                else
                {
                    tsType = state.ParameterTypeTranspilerMap[methodGroup.First().ReturnType].TsType;                    
                }

                var methodProvider = new InvokeProvider
                {
                    Type = Type,
                    Invokers = overloads,
                    MethodName = methodGroup.Key,                    
                    AssemblyName = Type.Assembly.GetName().Name,
                    ClassName = Type.FullName,                    
                };

                methodProviderCollection.InvokeProviders.Add(methodProvider);
            }            

            var source = new
            {
                Options = options,
                Type = Type,
                ConstructorProvider = constructorProvider,                
                MethodProviderCollection = methodProviderCollection                
            };            

            string template = @"
                export namespace {{Type.Namespace}} {
                    export class {{Type.Name}} {                        
                        {{#each ConstructorProvider.Invokers}}
                        {{MethodName}}({{#each Parameters}}{{#if IsDI}}{{#else}}{{Name}}: {{TsType}}{{#if IsLastParam}}{{#else}}, {{/if}}{{/if}}{{/each}})
                        {{/each}}                        
                        {{ConstructorProvider.MethodName}}({{#each ConstructorProvider.LastInvoker.Parameters}}{{#if IsDI}}{{#else}}{{Name}}? : any{{#if IsLastParam}}{{#else}}, {{/if}}{{/if}}{{/each}}) {                                                                        
                            if ((() => false)()) {
                            }
                            {{#each ConstructorProvider.ReverseInvokers}}
                            else if (
                                true
                                {{#each Parameters}}
                                {{#if IsDI}}{{#else}}                                
                                && typeof arguments[{{Index}}] !== 'undefined' && ({{Validation}})                                
                                {{/if}}
                                {{/each}}
                            ) {
                                
                                this._$_constructorMetadataToken = {{Method.MetadataToken}};

                                {{#each Parameters}}
                                {{#if IsDI}}{{#else}}this._$_constructorParameterValues.{{Name}} = arguments[{{Index}}];{{/if}}
                                {{/each}}
                            }
                            {{/each}}  
                            {{#if ConstructorProvider.NotExistsNoParamInvoker}}
                            else {
                                this._$_validConstructorParams = false;
                            }
                            {{/if}}
                        }
                        
                        private _$_constructorMetadataToken: number = 0;
                        private _$_constructorParameterValues: any = {};
                        private _$_validConstructorParams = true                        

                        {{#each MethodProviderCollection.InvokeProviders}}
                        {{#each Invokers}}
                        {{MethodName}}({{#each Parameters}}{{#if IsDI}}{{#else}}{{Name}}: {{TsType}}{{#if IsLastParam}}{{#else}}, {{/if}}{{/if}}{{/each}}): Promise<{{ReturnTsType}}>                        
                        {{/each}}
                        {{MethodName}}({{#each LastInvoker.Parameters}}{{#if IsDI}}{{#else}}{{Name}}? : any{{#if IsLastParam}}{{#else}}, {{/if}}{{/if}}{{/each}}): Promise<any> {                        
                            if (!this._$_validConstructorParams) {
                                throw new Error('invalid constructor arguments');
                            }
                            
                            if ((() => false)()) {
                            }
                            {{#each ReverseInvokers}}
                            else if (
                                true
                                {{#each Parameters}}
                                {{#if IsDI}}{{#else}}                                
                                && typeof arguments[{{Index}}] !== 'undefined' && ({{Validation}})                                
                                {{/if}}
                                {{/each}}
                            ) {
                                var _$_methodMetadataToken: number = {{Method.MetadataToken}};
                                var _$_methodParameterValues: any = {};                                

                                {{#each Parameters}}
                                {{#if IsDI}}{{#else}}_$_methodParameterValues.{{Name}} = arguments[{{Index}}];{{/if}}                                
                                {{/each}}

                                return {{Options.PostHandler.Function}}<{{ReturnTsType}}>({                                
                                    contentType: '{{ContentType}}',
                                    resultType: '{{ResultType}}',
                                    assemblyName: '{{AssemblyName}}',
                                    className: '{{ClassName}}',
                                    constructorMetadataToken: this._$_constructorMetadataToken,
                                    constructorParameterValues: this._$_constructorParameterValues,
                                    methodName: '{{MethodName}}',
                                    methodMetadataToken: _$_methodMetadataToken,
                                    methodParameterValues: _$_methodParameterValues
                                } as any);  
                            }
                            {{/each}}  
                            {{#if NotExistsNoParamInvoker}}
                            else {
                                throw new Error('invalid method arguments');
                            }
                            {{/if}}
                        }
                        {{/each}}
                    }
                }
            ";

            FormatCompiler compiler = new FormatCompiler();
            Generator generator = compiler.Compile(template.Replace(Environment.NewLine, "{{#newline}}"));
            string result = generator.Render(source);

            return result;
        }
    }

    public abstract class ParameterTypeTranspilerBase : TypeTranspilerBase
{
        public abstract string GetValidationCode(TranspileOptions options, string parameterName);
        
    }

    public class PrimitiveTypeTranspiler : ParameterTypeTranspilerBase
    {
        public bool IsInstance { get; set; }

        public override string GetTypeCode(TranspileOptions options, State state)
        {
            return string.Empty;
        }

        public override string GetValidationCode(TranspileOptions options, string parameterName)
        {
            if (IsInstance)
            {
                return $"{parameterName}.constructor.name === '{TsType}'";
            }
            else
            {
                return $"typeof {parameterName}  === '{TsType}'";
            }
        }
    }

    public class ClassTypeTranspiler : ParameterTypeTranspilerBase
    {
        public override string GetTypeCode(TranspileOptions options, State state)
        {
            List<object> properties = new List<object>();

            foreach (var property in Type.GetProperties())
            {
                Type type = property.PropertyType;
                string tsType = string.Empty;

                if (state.IsArray(type, out Type elementType))
                {                    
                    tsType = $"{state.ParameterTypeTranspilerMap[elementType].TsType}[]";
                }
                else
                {
                    tsType = state.ParameterTypeTranspilerMap[type].TsType;
                }

                var item = new
                {
                    Name = property.Name,
                    TsType = tsType
                };

                properties.Add(item);
            }

            var source = new
            {
                Type = Type,
                Properties = properties,
                TsTypeShortName = state.ParameterTypeTranspilerMap[Type].TsShortType
            };            

            if (source.TsTypeShortName == "object")
            {
                return string.Empty;
            }

            string template = @"
                export namespace {{Type.Namespace}} {
                    export class {{TsTypeShortName}} {
                        {{#each Properties}}
                        {{Name}}: {{TsType}}                        
                        {{/each}}
                    }
                }
            ";

            FormatCompiler compiler = new FormatCompiler();
            Generator generator = compiler.Compile(template.Replace(Environment.NewLine, "{{#newline}}"));
            string result = generator.Render(source);

            return result;
        }

        public override string GetValidationCode(TranspileOptions options, string parameterName)
        {
            List<string> validators = new List<string>();
            foreach (var property in Type.GetProperties())
            {
                validators.Add($"typeof {parameterName}['{property.Name}'] !== 'undefined'");
            }
            return string.Join(" && ", validators);
        }
    }    

    public class TypeScriptTranspiler
    {   
        public string Transpile(TranspileOptions options)
        {
            string[] dlls = GetDlls(options);
            Type[] classTypes = GetClassTypes(dlls);

            State state = new State();

            SetTranspilers(classTypes, state);

            string code = $"                {options.PostHandler.Import}{Environment.NewLine}";

            code += GetParametersCode(options, state);

            code += GetEntriesCode(options, state);

            string result = System.Text.RegularExpressions.Regex.Replace(code, @"^\s+$[\r\n\r\n]*", "\r\n", System.Text.RegularExpressions.RegexOptions.Multiline);

            return result;
        }

        private string GetEntriesCode(TranspileOptions options, State state)
        {
            StringBuilder code = new StringBuilder();
            foreach (var transpiler in state.EntryTypeTranspilerMap.Values)
            {
                code.Append(transpiler.GetTypeCode(options, state));
            }

            return code.ToString();
        }

        private string GetParametersCode(TranspileOptions options, State state)
        {
            StringBuilder code = new StringBuilder();
            foreach (var transpiler in state.ParameterTypeTranspilerMap.Values)
            {
                if (transpiler is PrimitiveTypeTranspiler)
                {
                    continue;
                }

                if (transpiler is ParameterTypeTranspilerBase)
                {
                    code.Append(transpiler.GetTypeCode(options, state));
                }
            }

            return code.ToString();
        }
        

        private void SetTranspilers(Type[] classTypes, State state)
        {
            SetPrimitiveTypeTranspilers(state);
            SetParameterTypeTranspilers(classTypes, state);
            SetEntryTypeTranspilers(classTypes, state);
        }

        private void SetEntryTypeTranspilers(Type[] classTypes, State state)
        {
            List<EntryTypeTranspiler> transpilers = new List<EntryTypeTranspiler>();

            foreach (var type in classTypes)
            {
                if (state.EntryTypeTranspilerMap.ContainsKey(type))
                {
                    continue;
                }

                EntryTypeTranspiler transpiler = new EntryTypeTranspiler
                {
                    Type = type,
                    TsType = $"{type.Namespace}.{type.Name}"
                };

                state.EntryTypeTranspilerMap[type] = transpiler;
            }
        }

        private void SetParameterTypeTranspilers(Type[] classTypes, State state)
        {
            foreach (var type in classTypes)
            {
                List<MethodBase> methods = new List<MethodBase>();

                foreach (var classType in classTypes)
                {
                    methods.AddRange(classType.GetConstructors());
                    methods.AddRange(classType.GetMethods());
                }

                foreach (var method in methods)
                {
                    if (method.DeclaringType == typeof(object))
                    {
                        continue;
                    }

                    foreach (var parameter in method.GetParameters())
                    {
                        if (Attribute.GetCustomAttribute(parameter, typeof(TypeScriptDiAttribute)) != null)
                        {
                            continue;
                        }

                        SetCustomTypeTranspiler(parameter.ParameterType, state);
                    }

                    if (method is MethodInfo methodInfo)
                    {
                        if (methodInfo.ReturnType != typeof(void))
                        {
                            SetCustomTypeTranspiler(methodInfo.ReturnType, state);
                        }
                    }
                }
            }
        }

        private void SetCustomTypeTranspiler(Type type, State state)
        {
            if (type == typeof(object))
            {
                return;
            }

            if (state.IsArray(type, out Type elementType))
            {
                type = elementType;
            }

            if (state.ParameterTypeTranspilerMap.ContainsKey(type))
            {
                return;
            }

            foreach (var property in type.GetProperties())
            {
                if (property.DeclaringType == typeof(object))
                {
                    continue;
                }

                SetCustomTypeTranspiler(property.PropertyType, state);
            }

            string tsType = GetTsTypeName(type);

            state.ParameterTypeTranspilerMap[type] = new ClassTypeTranspiler
            {
                Type = type,
                TsType = tsType
            };
        }

        private string GetTsTypeName(Type type)
        {
            foreach (var contract in type.GetInterfaces())
            {
                if (contract == typeof(IDictionary) || contract.IsGenericType && contract.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {                    
                    return "object";
                }
            }

            if (type.GenericTypeArguments.Length > 0)
            {                
                StringBuilder genericTsTypes = new StringBuilder();
                foreach (var genericType in type.GenericTypeArguments)
                {
                    if (genericTsTypes.ToString() != string.Empty)
                    {
                        genericTsTypes.Append("_");
                    }

                    string genericTsType = GetTsTypeName(genericType).Replace(".", string.Empty);                    

                    genericTsTypes.Append(genericTsType);
                }

                return $"{type.Namespace}.{type.Name.Replace("`", "_")}_{genericTsTypes.ToString()}";
            }
            else
            {
                return $"{type.Namespace}.{type.Name}";
            }
        }

        private void SetPrimitiveTypeTranspilers(State state)
        {
            List<ParameterTypeTranspilerBase> list = new List<ParameterTypeTranspilerBase>();

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(void),
                    TsType = "void"
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(int),
                    TsType = "number"
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(double),
                    TsType = "number"
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(float),
                    TsType = "number"
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(decimal),
                    TsType = "number"
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(bool),
                    TsType = "boolean"
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(string),
                    TsType = "string"
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(char),
                    TsType = "string"
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(Stream),
                    TsType = "Blob",
                    IsInstance = true                    
                });

            list.Add(
                new PrimitiveTypeTranspiler
                {
                    Type = typeof(object),
                    TsType = "object",
                    IsInstance = true
                });

            list.ForEach(x => state.ParameterTypeTranspilerMap[x.Type] = x);
        }

        private Type[] GetClassTypes(string[] dlls)
        {            
            List<Type> types = new List<Type>();
            foreach (var dll in dlls)
            {
                Assembly assembly = Assembly.LoadFrom(dll);

                foreach (var type in assembly.GetTypes())
                {
                    if (Attribute.GetCustomAttribute(type, typeof(TypeScriptAttribute)) == null)
                    {
                        continue;
                    }

                    types.Add(type);
                }
            }

            return types.ToArray();
        }

        private string[] GetDlls(TranspileOptions options)
        {            
            List<string> dlls = new List<string>();

            XmlDocument xml = new XmlDocument();
            xml.Load(Path.Combine(options.BaseDir, options.Project));

            foreach (XmlNode node in xml.ChildNodes)
            {
                foreach (XmlNode subNode in node.ChildNodes)
                {
                    if (subNode.Name == "PropertyGroup")
                    {
                        foreach (XmlNode item in subNode.ChildNodes)
                        {
                            if (item.Name == "AssemblyName")
                            {
                                string dll = Path.Combine(options.BaseDir, options.DllDir, $"{item.InnerText}.dll");

                                if (File.Exists(dll))
                                {
                                    Console.WriteLine("converte to ts : " + dll);
                                    dlls.Add(dll);
                                }
                            }
                        }
                    }

                    if (subNode.Name == "ItemGroup")
                    {
                        foreach (XmlNode item in subNode.ChildNodes)
                        {
                            if (item.Name == "Reference")
                            {
                                foreach (XmlNode child in item.ChildNodes)
                                {
                                    if (child.Name == "HintPath")
                                    {
                                        string dll = Path.Combine(options.BaseDir, $"{child.InnerText}");

                                        if (File.Exists(dll))
                                        {
                                            Console.WriteLine("converte to ts : " + dll);
                                            dlls.Add(dll);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (subNode.Name == "ItemGroup")
                    {
                        foreach (XmlNode item in subNode.ChildNodes)
                        {
                            if (item.Name == "ProjectReference")
                            {
                                foreach (XmlNode child in item.ChildNodes)
                                {
                                    if (child.Name == "Name")
                                    {
                                        string dll = Path.Combine(options.BaseDir, options.DllDir, $"{child.InnerText}.dll");

                                        if (File.Exists(dll))
                                        {
                                            Console.WriteLine("converte to ts : " + dll);
                                            dlls.Add(dll);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }

            return dlls.ToArray();
        }
    }
}
