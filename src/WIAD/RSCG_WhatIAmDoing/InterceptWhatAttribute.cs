namespace RSCG_WhatIAmDoing;
[AttributeUsage(AttributeTargets.Class , AllowMultiple = true)]
public class InterceptStaticAttribute: Attribute
{
    private readonly string typeTo;
    public InterceptStaticAttribute(string typeTo)
    {
        this.typeTo = typeTo;
    }
}
