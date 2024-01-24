
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
        this.ValueArguments = ValueArgs(source);
        this.StringArguments = StringArgs(source);
        
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
    public Argument[] ValueArguments { get; set; } = Array.Empty<Argument>();
    public Argument[] StringArguments { get; set; } = Array.Empty<Argument>();

    private static Argument[] ValueArgs(TypeAndMethod source)
    {
        return source.Arguments.Where(it => it.IsValueType).ToArray();
    }
    const string typeofString = "String";
    private static Argument[] StringArgs(TypeAndMethod source)
    {
        return source.Arguments
            .Where(it => !it.IsValueType)
            .Where(it =>
                (it.TypeArgument == typeofString)
                ||
                (it.TypeArgument == typeofString + "?")
                )
            .ToArray();
    }
    //[IgnoreProperty]
    //public string FullName
    //{
    //    get
    //    {
    //        return FirstName + " " + LastName;
    //    }
    //}
}


