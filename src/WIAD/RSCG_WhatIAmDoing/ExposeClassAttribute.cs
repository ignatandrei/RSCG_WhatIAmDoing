namespace RSCG_WhatIAmDoing;
public class ExposeClassAttribute: Attribute
{
    public ExposeClassAttribute(Type typeTo, string transformToString)
    {
        TypeTo = typeTo.FullName;
        TransformToString = transformToString;
    }

    public string TypeTo { get; }
    public string TransformToString { get; }
}


public class DataFromExposeClass : IEquatable<DataFromExposeClass>
{
    public DataFromExposeClass(INamedTypeSymbol type)
    {
        var attr = type.GetAttributes();
        foreach (var item in attr)
        {
            if (item.AttributeClass?.ToDisplayString() == "RSCG_WhatIAmDoing.ExposeClassAttribute")
            {
                FullNameClass = item.ConstructorArguments[0].Value?.ToString() ?? "";

                ApplyData += item.ConstructorArguments[1].Value?.ToString() ?? "";
            }
        }
        ApplyData ??= "ToString()";
        FullNameClass = type.ToString();
    }
    public string FullNameClass { get; set; } = "";
    public string ApplyData { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is DataFromExposeClass other && Equals(other);
    }
    public bool Equals(DataFromExposeClass? other)
    {
        if (ReferenceEquals(null, other))
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return ApplyData == other.ApplyData && FullNameClass == other.FullNameClass;
    }
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = FullNameClass.GetHashCode();
            hashCode = (hashCode * 397) ^ ApplyData.GetHashCode();
            return hashCode;
        }
    }
}