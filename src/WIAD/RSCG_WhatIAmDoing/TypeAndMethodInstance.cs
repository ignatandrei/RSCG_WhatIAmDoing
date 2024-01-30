namespace RSCG_WhatIAmDoing;

public class TypeAndMethodInstance: TypeAndMethodData 
{
    public TypeAndMethodInstance()
    {

    }
    public TypeAndMethodInstance(TypeAndMethod? source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        RunBeforeMap(source);
        this.MethodName = source.MethodName;
        this.TypeOfClass = source.TypeOfClass;
        this.ValueArguments = source.ValueArguments;
        this.StringArguments =source.StringArguments;
        this.IsVoid = source.IsVoid();
    }
            
    
    public static void RunBeforeMap(TypeAndMethod? source)
    {

        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (source.IsStatic)
            throw new ArgumentException("IsStatic");

        if (!source.IsValid())
            throw new ArgumentException("NOT IsValid");

    }
    
    //[Newtonsoft.Json.JsonIgnore]
    internal Argument[] ValueArguments { get; set; } = Array.Empty<Argument>();
    //[Newtonsoft.Json.JsonIgnore] 
    internal Argument[] StringArguments { get; set; } = Array.Empty<Argument>();
    public bool IsVoid { get; set; }
    public string Tag { get; set; }=string.Empty;
}


