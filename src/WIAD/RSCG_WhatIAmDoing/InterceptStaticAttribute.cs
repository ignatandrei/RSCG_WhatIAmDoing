﻿
using System.Xml.Linq;

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

public class DataFromInterceptStatic: IEquatable<DataFromInterceptStatic>
{
    public DataFromInterceptStatic(INamedTypeSymbol type) 
    {
        var attr = type.GetAttributes();
        foreach (var item in attr)
        {
            if (item.AttributeClass?.ToDisplayString() == "RSCG_WhatIAmDoing.InterceptStaticAttribute")
            {
                TypesTo += item.ConstructorArguments[0].Value?.ToString() ?? "";
                TypesTo += ",";
            }
        }
        TypesTo = TypesTo?.TrimEnd(',')??"";
    }

    public string TypesTo { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is DataFromInterceptStatic other && Equals(other);
    }
    public bool Equals(DataFromInterceptStatic? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return TypesTo == other.TypesTo;
    }
    public override int GetHashCode()
    {
        return TypesTo.GetHashCode();
    }
}
