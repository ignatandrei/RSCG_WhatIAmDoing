
namespace RSCG_WhatIAmDoing;

public class TypeAndMethodStatic
{
    public TypeAndMethodStatic()
    {

    }
    public TypeAndMethodStatic(TypeAndMethod? source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        RunBeforeMap(source);
        this.MethodName = source.MethodName;
        this.TypeOfClass = source.TypeOfClass;
        this.ValueArguments = source.ValueArguments;
        this.StringArguments =source.StringArguments;
        this.IsVoid = source.IsVoid();
        //this.MeSer=System.Text.Json.JsonSerializer.Serialize("a");
        //this.MeSer = Newtonsoft.Json.JsonConvert.SerializeObject(this);
    }
            
    
    public static void RunBeforeMap(TypeAndMethod? source)
    {

        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (!source.IsStatic)
            throw new ArgumentException("NOT IsStatic");

        if (!source.IsValid())
            throw new ArgumentException("NOT IsValid");

    }
    public string TypeOfClass { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    
    //[Newtonsoft.Json.JsonIgnore]
    internal Argument[] ValueArguments { get; set; } = Array.Empty<Argument>();
    //[Newtonsoft.Json.JsonIgnore] 
    internal Argument[] StringArguments { get; set; } = Array.Empty<Argument>();
    public bool IsVoid { get; set; }
    public string Tag { get; set; }=string.Empty;
}


