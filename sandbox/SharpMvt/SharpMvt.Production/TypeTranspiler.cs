using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SharpMvt.Production
{
    public interface ITsTypeInfo
    {
        Type Type { get; set; }
        string TsType { get; }
        bool IsService { get; }
        bool IsTsClass { get; }
        bool IsArray { get; }
        string TsElementType { get; }
    }

    public interface ITsTypeTranspiler<TTsTypeInfo> where TTsTypeInfo : ITsTypeInfo
    {
        string GetTsCode(TTsTypeInfo tsTypeInfo, Dictionary<Type, ITsTypeInfo> tsTypeInfoMap);
    }

    public interface ITsTypeTranspiler : ITsTypeInfo
    {        
        string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap);
    }  

    public interface ITsArrayTypeTranspler : ITsTypeTranspiler
    {
        void SetElementTypeTranspiler(ITsTypeTranspiler elementTypeTranspiler);
    }

    public class VoidTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set;} = typeof(void);
        public string TsType => "void";
        public bool IsService => false;
        public bool IsTsClass => false;
        public bool IsArray => false;
        public string TsElementType => throw new InvalidOperationException($"{TsType} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class NumberTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set;}
        public string TsType => "number";
        public bool IsService => false;
        public bool IsTsClass => false;
        public bool IsArray => false;
        public string TsElementType => throw new InvalidOperationException($"{TsType} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class BoolTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set;} =  typeof(bool);
        public string TsType => "boolean";
        public bool IsService => false;
        public bool IsTsClass => false;
        public bool IsArray => false;
        public string TsElementType => throw new InvalidOperationException($"{TsType} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class StringTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set;} =  typeof(string);
        public string TsType => "string";
        public bool IsService => false;
        public bool IsTsClass => false;
        public bool IsArray => false;
        public string TsElementType => throw new InvalidOperationException($"{TsType} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
    }

    public class ArrayTranspiler : ITsArrayTypeTranspler
    {
        private ITsTypeTranspiler _elementTypeTranspiler;
        public Type Type { get; set; } 
        public string TsType => $"{_elementTypeTranspiler.TsType}[]";

        public bool IsService => false;
        public bool IsTsClass => true;
        public bool IsArray => true;
        public string TsElementType
        {
            get {                
                return _elementTypeTranspiler.TsType;
            }
        }
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            return string.Empty;
        }
        public void SetElementTypeTranspiler(ITsTypeTranspiler elementTypeTranspiler)
        {
            _elementTypeTranspiler = elementTypeTranspiler;
        }
    }

    public class TsProperty
    {
        public PropertyInfo PropertyInfo { get; set; }
        public string TsName => PropertyInfo.Name.ToFirstLower();
        public string TsType { get; set; }
        public bool IsArray { get; set; }
        public string TsElementType { get; set;}
        public string GetConstructorCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap, string self, string arg)
        {
            string code;

            if (IsArray)
            {
                code = $@"
                    {self}.{TsName} ={TsElementType}[]
                    
                    {arg}.{TsName}.forEach(x =>{{
                        var item = new {TsElementType}(x)
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
                    {TsName}: {TsType}";            

            return code;
        }
    }

    public class ClassParameterTranspiler : ITsTypeTranspiler
    {
        private Type _type;
        public Type Type 
        { 
            get 
            { 
                return _type; 
            } 
            set 
            { 
                if (_type.GetConstructor(new Type[0]) == null) throw new InvalidOperationException($"{_type} not has no parameter constructor");
                _type = value;
            } 
        }
        public string TsType => Type.GetCamelCaseFullName();
        public bool IsService => false;
        public bool IsTsClass => true;
        public bool IsArray => false;
        public string TsElementType => throw new InvalidOperationException($"{TsType} is not array.");
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
                {TsType} {{
                    constructor({TsType} {Type.Name.ToFirstLower()}) {{
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

                tsProperties.Add(new TsProperty{ PropertyInfo = propertyInfo });
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
                signatureCode.Append($"{tsTypeTranspilerMap.Get(parameterInfo.ParameterType).TsType} {parameterInfo.Name.ToFirstLower()}");
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
                    return tsTypeTranspilerMap.Get(methodInfo.ReturnType).TsType;
                }
                else
                {
                    return tsTypeTranspilerMap.Get(typeof(string)).TsType;
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
                    judgmentCode.Append($"arguments[{index}] instanceof {tsTypeTranspiler.TsType}");
                }
                else
                {
                    judgmentCode.Append($"typeof(arguments[{index}]) === '{tsTypeTranspiler.TsType}'");
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
        public Type Type { get; set; }
        public string TsType => Type.GetCamelCaseFullName();
        public bool IsService => true;
        public bool IsTsClass => true;
        public bool IsArray => false;
        public string TsElementType => throw new InvalidOperationException($"{TsType} is not array.");
        public string GetTsCode(Dictionary<Type, ITsTypeTranspiler> tsTypeTranspilerMap)
        {
            string constructorCode = GetConstructorCode(tsTypeTranspilerMap);
            string methodsCode = GetMethodsCode(tsTypeTranspilerMap);

            string code = $@"
                {TsType} {{
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