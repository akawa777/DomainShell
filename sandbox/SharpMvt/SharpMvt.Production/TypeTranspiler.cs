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
        bool IsTsService { get; }
        bool IsTsClass { get; }
        bool IsTsArray { get; }
        ITsTypeInfo TsElementTypeInfo { get; set; }
    }

    public interface ITsTypeTranspiler : ITsTypeInfo
    {        
        string GetTsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap);
    }  

    public class VoidTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set;} = typeof(void);
        public string TsType => "void";
        public bool IsTsService => false;
        public bool IsTsClass => false;
        public bool IsTsArray => false;
        public ITsTypeInfo TsElementTypeInfo { get { throw new InvalidOperationException($"{TsType} is not array."); } set { throw new InvalidOperationException($"{TsType} is not array."); } }
        public string GetTsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            return string.Empty;
        }
    }

    public class NumberTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set;}
        public string TsType => "number";
        public bool IsTsService => false;
        public bool IsTsClass => false;
        public bool IsTsArray => false;
        public ITsTypeInfo TsElementTypeInfo { get { throw new InvalidOperationException($"{TsType} is not array."); } set { throw new InvalidOperationException($"{TsType} is not array."); } }
        public string GetTsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            return string.Empty;
        }
    }

    public class BoolTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set;} =  typeof(bool);
        public string TsType => "boolean";
        public bool IsTsService => false;
        public bool IsTsClass => false;
        public bool IsTsArray => false;
        public ITsTypeInfo TsElementTypeInfo { get { throw new InvalidOperationException($"{TsType} is not array."); } set { throw new InvalidOperationException($"{TsType} is not array."); } }
        public string GetTsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap) => string.Empty;
    }

    public class StringTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set;} =  typeof(string);
        public string TsType => "string";
        public bool IsTsService => false;
        public bool IsTsClass => false;
        public bool IsTsArray => false;
        public ITsTypeInfo TsElementTypeInfo { get { throw new InvalidOperationException($"{TsType} is not array."); } set { throw new InvalidOperationException($"{TsType} is not array."); } }
        public string GetTsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap) => string.Empty;
    }

    public class ArrayTranspiler : ITsTypeTranspiler
    {
        public Type Type { get; set; } 
        public string TsType => $"{TsElementTypeInfo.TsType}[]";
        public bool IsTsService => false;
        public bool IsTsClass => true;
        public bool IsTsArray => true;
        public ITsTypeInfo TsElementTypeInfo { get; set; }
        public string GetTsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap) => string.Empty;
    }

    public class TsProperty
    {
        public PropertyInfo PropertyInfo { get; set; }
        public string TsName => PropertyInfo.Name.ToFirstLower();
        public string TsType { get; set; }
        public bool IsTsArray { get; set; }
        public string TsElementType { get; set;}
        public string GetConstructorCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            string code;

            if (IsTsArray)
            {
                code = $@"
                    this.{TsName} = []
                    
                    arguments[0].{TsName}.forEach(x =>{{
                        var item = new {TsElementType}(x)
                        this.{TsName}.push(item)
                    }})";
            }
            else
            {
                code = $@"
                    this.{TsName} = arguments[0].{TsName}";
            }

            return code;
        }

        public string GetPropertyCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
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
        public bool IsTsService => false;
        public bool IsTsClass => true;
        public bool IsTsArray => false;
        public ITsTypeInfo TsElementTypeInfo { get { throw new InvalidOperationException($"{TsType} is not array."); } set { throw new InvalidOperationException($"{TsType} is not array."); } }
        public string GetTsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            TsProperty[] tsProperties = GetTsProperties(tsTypeInfoMap);
            StringBuilder constructorCode = new StringBuilder();
            StringBuilder propertiesCode = new StringBuilder();

            foreach (var tsProperty in tsProperties)
            {
                constructorCode.Append(tsProperty.GetConstructorCode(tsTypeInfoMap));
                propertiesCode.Append(tsProperty.GetPropertyCode(tsTypeInfoMap));
            }

            string code = $@"
                {TsType} {{
                    constructor({TsType} {Type.Name.ToFirstLower()}) {{                        
                        {constructorCode}
                    }}

                    {propertiesCode}
                }}
            ";

            return code;
        }
        
        private TsProperty[] GetTsProperties(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            PropertyInfo[] propertyInfos = Type.GetProperties();

            List<TsProperty> tsProperties = new List<TsProperty>();

            foreach (var propertyInfo in propertyInfos)
            {
                ITsTypeInfo tsTypeInfo = tsTypeInfoMap.Get(propertyInfo.PropertyType);

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
        public string GetTsSignatureCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            ParameterInfo[] parameterInfos = MethodBase.GetParametersWithoutInject();
            StringBuilder signatureCode = new StringBuilder();            

            int index = 0;
            foreach (var parameterInfo in parameterInfos)
            {
                if (index > 0) signatureCode.Append(", ");
                signatureCode.Append($"{tsTypeInfoMap.Get(parameterInfo.ParameterType).TsType} {parameterInfo.Name.ToFirstLower()}");
                index++;
            }

            string code = signatureCode.ToString();
            return code;
        }

        public string GetTsReturnType(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            if (MethodBase is MethodInfo methodInfo)
            {
                if (Attribute.GetCustomAttribute(methodInfo, typeof(LinkAttrubute)) == null)
                {
                    return tsTypeInfoMap.Get(methodInfo.ReturnType).TsType;
                }
                else
                {
                    return tsTypeInfoMap.Get(typeof(string)).TsType;
                }
            } 
            else
            {
                throw new InvalidOperationException($"{MethodBase.ReflectedType.GetFullName()} {MethodBase.Name} not has return type.");            
            } 
        }

        public string GetJudgmentCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            ParameterInfo[] parameterInfos = MethodBase.GetParametersWithoutInject();
            StringBuilder judgmentCode = new StringBuilder();

            if (parameterInfos.Length == 0) return "true";

            int index = 0;
            foreach (var parameterInfo in parameterInfos)
            {   
                if (index > 0) judgmentCode.Append(" && ");

                ITsTypeInfo tsTypeInfo = tsTypeInfoMap.Get(parameterInfo.ParameterType);

                if (tsTypeInfo.IsTsArray)
                {
                    judgmentCode.Append($"arguments[{index}] instanceof Array");                    
                }
                else if (tsTypeInfo.IsTsClass)
                {
                    judgmentCode.Append($"arguments[{index}] instanceof {tsTypeInfo.TsType}");
                }
                else
                {
                    judgmentCode.Append($"typeof(arguments[{index}]) === '{tsTypeInfo.TsType}'");
                }
                
                index++;
            }

            string code = judgmentCode.ToString();

            return code;
        }

        public string GetSettingParameterCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap, string parameters)
        {
            ParameterInfo[] parameterInfos = MethodBase.GetParametersWithoutInject();            
            StringBuilder settingParameterCode = new StringBuilder();

            int index = 0;
            foreach (var parameterInfo in parameterInfos)
            {   
                ITsTypeInfo tsTypeInfo = tsTypeInfoMap.Get(parameterInfo.ParameterType);

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
        private Type _type;
        public Type Type 
        { 
            get 
            { 
                return _type; 
            } 
            set 
            { 
                if (_type.GetConstructorsWithoutIgnore().Length == 0) throw new InvalidOperationException($"{_type} not has no parameter constructor");
                _type = value;
            } 
        }
        public string TsType => Type.GetCamelCaseFullName();
        public bool IsTsService => true;
        public bool IsTsClass => true;
        public bool IsTsArray => false;
        public ITsTypeInfo TsElementTypeInfo { get { throw new InvalidOperationException($"{TsType} is not array."); } set { throw new InvalidOperationException($"{TsType} is not array."); } }
        public string GetTsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            string constructorCode = GetConstructorCode(tsTypeInfoMap);
            string methodsCode = GetMethodsCode(tsTypeInfoMap);

            string code = $@"
                {TsType} {{
                    {constructorCode}
                    private constructorHash:string
                    private constructorParameters = {{}}
                    {methodsCode}
                }}
            ";

            return code;
        }

        public string GetConstructorCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            TsMethod[] tsMethods = GetTsMethods(tsTypeInfoMap, Type.GetConstructorsWithoutIgnore());
            StringBuilder signatureCodes = new StringBuilder();
            StringBuilder settingParameterCode = new StringBuilder();

            foreach (var tsMethod in tsMethods.OrderBy(x => x.ValidParameterCount))
            {                
                signatureCodes.Append($@"
                constructor({tsMethod.GetTsSignatureCode(tsTypeInfoMap)})");
            }
           
            foreach (var tsMethod in tsMethods.OrderByDescending(x => x.ValidParameterCount))
            {                
                settingParameterCode.Append($@"
                        else if({tsMethod.GetJudgmentCode(tsTypeInfoMap)}) {{
                        valid = true
                        this.constructorHash = {tsMethod.MethodBase.GetMethodHash()}
                        {tsMethod.GetSettingParameterCode(tsTypeInfoMap, "this.constructorParameters")}
                    }}");
            }

            StringBuilder lastSignatureCode = new StringBuilder();
            for (int i = 0; i < tsMethods.Max(x => x.ValidParameterCount); i++)
            {
                if (i > 0) lastSignatureCode.Append(", ");
                lastSignatureCode.Append($"param{i}?: any");
            }

            signatureCodes.Append($@"
                constructor({lastSignatureCode})");            

            string code = $@"
                {signatureCodes} {{    
                    var valid: boolean = false                
                    if (false) {{
                    }} {settingParameterCode}
                    if(!valid) throw new Error('constructor parameter is invalid.')
                }}
            ";

            return code;
        }

        public string GetMethodsCode(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap)
        {
            var groups = Type.GetMethodsWithoutIgnore().GroupBy(x => x.Name);
            StringBuilder code = new StringBuilder();

            foreach (var group in groups)
            {
                TsMethod[] tsMethods = GetTsMethods(tsTypeInfoMap, group.ToArray());
                StringBuilder signatureCodes = new StringBuilder();
                StringBuilder settingParameterCode = new StringBuilder();
            
                foreach (var tsMethod in tsMethods.OrderBy(x => x.ValidParameterCount))
                {
                    signatureCodes.Append($@"
                    {group.Key}({tsMethod.GetTsSignatureCode(tsTypeInfoMap)}): {tsMethod.GetTsReturnType(tsTypeInfoMap)}");
                }

                foreach (var tsMethod in tsMethods.OrderByDescending(x => x.ValidParameterCount))
                {
                    settingParameterCode.Append($@"
                        else if({tsMethod.GetJudgmentCode(tsTypeInfoMap)}) {{
                        valid = true
                        methodHash = {tsMethod.MethodBase.GetMethodHash()}
                        {tsMethod.GetSettingParameterCode(tsTypeInfoMap, "methodParameters")}
                    }}");
                }

                StringBuilder lastSignatureCode = new StringBuilder();
                for (int i = 0; i < tsMethods.Max(x => x.ValidParameterCount); i++)
                {
                    if (i > 0) lastSignatureCode.Append(", ");
                    lastSignatureCode.Append($"param{i}?: any");
                }

                signatureCodes.Append($@"
                    {group.Key}({lastSignatureCode})");            

                code.Append($@"
                    {signatureCodes} {{     
                        var valid: boolean = false
                        var methodHash:string = ''  
                        var methodParameters = {{}}                 
                        if (false) {{
                        }} {settingParameterCode}
                        if(!valid) throw new Error('method parameter is invalid.')
                    }}");
            }
            
            return code.ToString();
        }

        private TsMethod[] GetTsMethods(Dictionary<Type, ITsTypeInfo> tsTypeInfoMap, MethodBase[] methodBases)
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