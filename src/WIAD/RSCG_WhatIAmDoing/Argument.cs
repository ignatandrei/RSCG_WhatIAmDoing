namespace RSCG_WhatIAmDoing;
[DebuggerDisplay("{TypeArgument} {Name}" )]
public partial struct Argument
{
    internal Argument(IParameterSymbol p)   
    {
        string typeAndName =p.ToDisplayString();
        this.IsValueType=p.Type.IsValueType;
        this.TypeAndName = typeAndName;
        //this.Type = typeAndName.Split(' ')[0];
        this.TypeArgument = p.Type.ToDisplayString();
        this.Name = typeAndName.Split(' ')[1];
    }
    public bool IsValueType { get; set; }
    public string TypeAndName { get; set; }
    public string TypeArgument { get; set; }
    public string Name { get; set; }
}
