using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpMvt.Production
{
    public interface ITsTypeTranspiler
    {
        Type Type { get;}
        string GetTsType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap);
        bool IsService { get; }
        bool IsTsClass { get; }
        bool IsArray { get; }
        string GetTsElementType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap);
        string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap);
    }   

    public class VoidTranspiler : ITsTypeTranspiler
    {
        public Type Type => typeof(void);
        public string GetTsType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => "void";
        public bool IsService => false;
        public bool IsTsClass => false;
        public bool IsArray => false;
        public string GetTsElementType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => throw new InvalidOperationException($"{GetTsType(tsTypeTranspilerMap)} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class NumberTranspiler : ITsTypeTranspiler
    {
        public NumberTranspiler(Type type)
        {
            Type = type;
        }        

        public Type Type { get; }
        public string GetTsType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => "number";
        public bool IsService => false;
        public bool IsTsClass => false;
        public bool IsArray => false;
        public string GetTsElementType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => throw new InvalidOperationException($"{GetTsType(tsTypeTranspilerMap)} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class BoolTranspiler : ITsTypeTranspiler
    {
        public Type Type => typeof(bool);
        public string GetTsType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => "boolean";
        public bool IsService => false;
        public bool IsTsClass => false;
        public bool IsArray => false;
        public string GetTsElementType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => throw new InvalidOperationException($"{GetTsType(tsTypeTranspilerMap)} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class StringTranspiler : ITsTypeTranspiler
    {
        public Type Type => typeof(string);
        public string GetTsType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => "string";
        public bool IsService => false;
        public bool IsTsClass => false;
        public bool IsArray => false;
        public string GetTsElementType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => throw new InvalidOperationException($"{GetTsType(tsTypeTranspilerMap)} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class ArrayTranspiler : ITsTypeTranspiler
    {
        public ArrayTranspiler(Type type, Type elementType)
        {
            Type = type;
            _elementType = elementType;            
        }    

        private Type _elementType;
        public Type Type { get; }
        public string GetTsType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {            
            ITsTypeTranspiler tsTypeTranspiler = tsTypeTranspilerMap.Get(_elementType);
            string tsElementType = tsTypeTranspilerMap[_elementType].GetTsType(tsTypeTranspilerMap);

            return $"{tsElementType}[]";
        }

        public bool IsService => false;
        public bool IsTsClass => true;
        public bool IsArray => true;
        public string GetTsElementType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            ITsTypeTranspiler tsTypeTranspiler = tsTypeTranspilerMap.Get(_elementType);
            return tsTypeTranspilerMap[_elementType].GetTsType(tsTypeTranspilerMap);
        }
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class TsProperty
    {
        public PropertyInfo PropertyInfo { get; set; }
        public string TsName => PropertyInfo.Name.ToFirstLower();
        public ITsTypeTranspiler TsTypeTranspiler { get; set; }
        public string GetConstructorCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap, string self, string arg)
        {
            string code;

            if (TsTypeTranspiler.IsArray)
            {
                code = $@"
                    {self}.{TsName} = new Array<{TsTypeTranspiler.GetTsElementType(tsTypeTranspilerMap)}>()
                    
                    {arg}.{TsName}.forEach(x =>{{
                        var item = new {TsTypeTranspiler.GetTsElementType(tsTypeTranspilerMap)}(x)
                        {self}.{TsName}.push(item);
                    }})";
            }
            else
            {
                code = $@"
                        {self}.{TsName} = {arg}.{TsName}";
            }

            return code;
        }

        public string GetPropertyCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            string code = $@"
                    {TsName}: {TsTypeTranspiler.GetTsType(tsTypeTranspilerMap)}";            

            return code;
        }
    }

    public class ClassParameterTranspiler : ITsTypeTranspiler
    {
        public ClassParameterTranspiler(Type type)
        {
            if (type.GetConstructor(new Type[0]) == null) throw new InvalidOperationException($"{type} not has no parameter constructor");

            Type = type;
        }    

        public Type Type { get; }
        public string GetTsType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => Type.GetCamelCaseFullName();
        public bool IsService => false;
        public bool IsTsClass => true;
        public bool IsArray => false;
        public string GetTsElementType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => throw new InvalidOperationException($"{GetTsType(tsTypeTranspilerMap)} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            TsProperty[] tsProperties = GetTsProperties(tsTypeTranspilerMap);
            StringBuilder constructorCode = new StringBuilder();
            StringBuilder propertiesCode = new StringBuilder();

            foreach (var tsProperty in tsProperties)
            {
                constructorCode.Append(tsProperty.GetConstructorCode(tsTypeTranspilerMap, "self", Type.Name.ToFirstLower()));
                propertiesCode.Append(tsProperty.GetPropertyCode(tsTypeTranspilerMap));
            }

            string code = $@"
                {GetTsType(tsTypeTranspilerMap)} {{
                    constructor({GetTsType(tsTypeTranspilerMap)} {Type.Name.ToFirstLower()}) {{
                        var self = this
                        {constructorCode}
                    }}

                    {propertiesCode}
                }}
            ";

            return code;
        }
        
        private TsProperty[] GetTsProperties(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            PropertyInfo[] propertyInfos = Type.GetProperties();

            List<TsProperty> tsProperties = new List<TsProperty>();

            foreach (var propertyInfo in propertyInfos)
            {
                ITsTypeTranspiler tsTypeTranspiler = tsTypeTranspilerMap.Get(propertyInfo.PropertyType);

                tsProperties.Add(new TsProperty{ PropertyInfo = propertyInfo, TsTypeTranspiler = tsTypeTranspiler });
            }

            return tsProperties.ToArray();
        }
    }

    public class TsMethod
    {
        public MethodBase MethodBase { get; set; }
        public string TsName => MethodBase.Name.ToFirstLower();
        public bool Ignore => Attribute.GetCustomAttribute(MethodBase, typeof(IgnoreAttrubute)) != null;
        public int ValidParameterCount => MethodBase.GetParameters().Where(x => Attribute.GetCustomAttribute(x, typeof(InjectAttribute)) == null).Count();
        public string GetTsSignatureCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            ParameterInfo[] parameterInfos = MethodBase.GetParameters();
            StringBuilder signatureCode = new StringBuilder();            

            int index = 0;
            foreach (var parameterInfo in parameterInfos)
            {
                if (Attribute.GetCustomAttribute(parameterInfo, typeof(InjectAttribute)) != null) continue;

                if (index > 0) signatureCode.Append(", ");
                signatureCode.Append($"{tsTypeTranspilerMap.Get(parameterInfo.ParameterType).GetTsType(tsTypeTranspilerMap)} {parameterInfo.Name.ToFirstLower()}");
                index++;
            }

            string code = signatureCode.ToString();
            return code;
        }

        public string GetTsReturnType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            if (MethodBase is MethodInfo methodInfo)
            {
                if (Attribute.GetCustomAttribute(methodInfo, typeof(LinkAttrubute)) == null)
                {
                    return tsTypeTranspilerMap.Get(methodInfo.ReturnType).GetTsType(tsTypeTranspilerMap);
                }
                else
                {
                    return tsTypeTranspilerMap.Get(typeof(string)).GetTsType(tsTypeTranspilerMap);
                }
            } 
            else
            {
                throw new InvalidOperationException($"{MethodBase.ReflectedType.GetFullName()} {MethodBase.Name} not has return type.");            
            } 
        }

        public string GetJudgmentCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            ParameterInfo[] parameterInfos = MethodBase.GetParameters();
            StringBuilder judgmentCode = new StringBuilder();

            int index = 0;
            foreach (var parameterInfo in parameterInfos)
            {       
                if (Attribute.GetCustomAttribute(parameterInfo, typeof(InjectAttribute)) != null) continue;

                if (index > 0) judgmentCode.Append(" && ");

                ITsTypeTranspiler tsTypeTranspiler = tsTypeTranspilerMap.Get(parameterInfo.ParameterType);

                if (tsTypeTranspiler.IsArray)
                {
                    judgmentCode.Append($"arguments[{index}] instanceof Array");                    
                }
                else if (tsTypeTranspiler.IsTsClass)
                {
                    judgmentCode.Append($"arguments[{index}] instanceof {tsTypeTranspiler.GetTsType(tsTypeTranspilerMap)}");
                }
                else
                {
                    judgmentCode.Append($"typeof(arguments[{index}]) === '{tsTypeTranspiler.GetTsType(tsTypeTranspilerMap)}'");
                }
                
                index++;
            }

            string code = judgmentCode.ToString();

            return code;
        }

        public string GetSettingParameterCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap, string parameters)
        {
            ParameterInfo[] parameterInfos = MethodBase.GetParameters();            
            StringBuilder settingParameterCode = new StringBuilder();

            int index = 0;
            foreach (var parameterInfo in parameterInfos)
            {       
                if (Attribute.GetCustomAttribute(parameterInfo, typeof(InjectAttribute)) != null) continue;                

                ITsTypeTranspiler tsTypeTranspiler = tsTypeTranspilerMap.Get(parameterInfo.ParameterType);

                settingParameterCode.Append($@"
                    {parameters}.{parameterInfo.Name.ToFirstLower()} = arguments[{index}]
                ");
                
                index++;
            }

            string code = settingParameterCode.ToString();

            return code;
        }
    }

    public class ServiceTranspiler : ITsTypeTranspiler
    {
        public ServiceTranspiler(Type type)
        {
            Type = type;
        }    

        public Type Type { get; }
        public string GetTsType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => Type.GetCamelCaseFullName();
        public bool IsService => true;
        public bool IsTsClass => true;
        public bool IsArray => false;
        public string GetTsElementType(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap) => throw new InvalidOperationException($"{GetTsType(tsTypeTranspilerMap)} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            string constructorCode = GetConstructorCode(tsTypeTranspilerMap);
            string methodsCode = GetMethodsCode(tsTypeTranspilerMap);

            string code = $@"
                {GetTsType(tsTypeTranspilerMap)} {{
                    {constructorCode}
                    private constructorParameters = {{}}
                    {methodsCode}
                }}
            ";

            return code;
        }

        public string GetConstructorCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            TsMethod[] tsMethods = GetTsMethods(tsTypeTranspilerMap, Type.GetConstructors());
            StringBuilder signatureCodes = new StringBuilder();
            StringBuilder settingParameterCode = new StringBuilder();
           
            foreach (var tsMethod in tsMethods)
            {
                signatureCodes.Append($@"
                constructor({tsMethod.GetTsSignatureCode(tsTypeTranspilerMap)})");

                settingParameterCode.Append($@"
                else if({tsMethod.GetJudgmentCode(tsTypeTranspilerMap)}) {{
                    {tsMethod.GetSettingParameterCode(tsTypeTranspilerMap, "self.constructorParameters")}
                }}");
            }

            StringBuilder lastSignatureCode = new StringBuilder();
            for (int i = 0; i < tsMethods.Last().ValidParameterCount; i++)
            {
                if (i > 0) lastSignatureCode.Append(", ");
                lastSignatureCode.Append($"param{i}?: any");
            }

            signatureCodes.Append($@"
                constructor({lastSignatureCode})");            

            string code = $@"
                {signatureCodes} {{
                    var self = this;
                    if (false) {{

                    {settingParameterCode}
                    }} else {{

                    }}
                }}
            ";

            return code;
        }

        public string GetMethodsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            var groups = Type.GetMethods().GroupBy(x => x.Name);
            StringBuilder code = new StringBuilder();

            foreach (var group in groups)
            {
                TsMethod[] tsMethods = GetTsMethods(tsTypeTranspilerMap, group.ToArray());
                StringBuilder signatureCodes = new StringBuilder();
                StringBuilder settingParameterCode = new StringBuilder();
            
                foreach (var tsMethod in tsMethods)
                {
                    signatureCodes.Append($@"
                    {group.Key}({tsMethod.GetTsSignatureCode(tsTypeTranspilerMap)}): {tsMethod.GetTsReturnType(tsTypeTranspilerMap)}");

                    settingParameterCode.Append($@"
                    else if({tsMethod.GetJudgmentCode(tsTypeTranspilerMap)}) {{
                        {tsMethod.GetSettingParameterCode(tsTypeTranspilerMap, "methodParameters")}
                    }}");
                }

                StringBuilder lastSignatureCode = new StringBuilder();
                for (int i = 0; i < tsMethods.Last().ValidParameterCount; i++)
                {
                    if (i > 0) lastSignatureCode.Append(", ");
                    lastSignatureCode.Append($"param{i}?: any");
                }

                signatureCodes.Append($@"
                    {group.Key}({lastSignatureCode})");            

                code.Append($@"
                    {signatureCodes} {{
                        var self = this;
                        if (false) {{

                        {settingParameterCode}
                        }} else {{

                        }}
                    }}");

            }
            
            return code.ToString();
        }

        private TsMethod[] GetTsMethods(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap, MethodBase[] methodBases)
        {
            List<TsMethod> tsMethods = new List<TsMethod>();

            foreach (var methodBase in methodBases)
            {
                tsMethods.Add(new TsMethod{ MethodBase = methodBase });
            }

            return tsMethods.OrderBy(x => x.ValidParameterCount).ToArray();
        }
    }
}