using System;

namespace SharpMvt
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class InjectAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    public class IgnoreAttrubute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method)]
    public class HashAttrubute : Attribute
    {
        public HashAttrubute(string hash)
        {
            if (string.IsNullOrEmpty(hash)) throw new ArgumentException("hash is null or empty.");
            Hash = hash;
        }        
        public string Hash { get;}
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class FormAttrubute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LinkAttrubute : Attribute
    {

    }
}