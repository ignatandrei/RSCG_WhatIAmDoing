using System.Xml.Linq;

namespace RSCG_WhatIAmDoing;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class InterceptInstanceClassAttribute
    : Attribute
    
{
    public readonly string method;
    public readonly string typeTo;
    public InterceptInstanceClassAttribute(Type type,string method="*")
    {
        this.method = method;
        this.typeTo = type.FullName;
    }
}
public class DataFromInterceptClass : IEquatable<DataFromInterceptClass>
{
    public DataFromInterceptClass(INamedTypeSymbol type)
    {
        var attr = type.GetAttributes();
        foreach (var item in attr)
        {
            if (item.AttributeClass?.ToDisplayString() == "RSCG_WhatIAmDoing.InterceptInstanceClassAttribute")
            {
                TypeTo = item.ConstructorArguments[0].Value?.ToString() ?? "";
                
                MethodsTo += item.ConstructorArguments[1].Value?.ToString() ?? "";
                MethodsTo += ",";
            }
        }
        MethodsTo = MethodsTo?.TrimEnd(',') ?? "";
        TypeTo ??= string.Empty;
    }

    public string TypeTo { get; set; }
    public string MethodsTo { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is DataFromInterceptClass other && Equals(other);
    }
    public bool Equals(DataFromInterceptClass? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return MethodsTo == other.MethodsTo && TypeTo==other.TypeTo ;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = TypeTo.GetHashCode() ;
            hashCode = (hashCode * 397) ^ MethodsTo.GetHashCode();
            return hashCode;
        }
    }
}