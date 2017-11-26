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

    [AttributeUsage(AttributeTargets.Method)]
    public class FormAttrubute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LinkAttrubute : Attribute
    {

    }
}