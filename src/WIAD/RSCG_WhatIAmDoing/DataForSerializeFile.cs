﻿namespace RSCG_WhatIAmDoing;
class DataForSerializeFile
{

    static int count = 0;
    public DataForSerializeFile(TypeAndMethod item,string nameClassToSerialize)
    {
        this.item = item;
        NameClassToSerialize = nameClassToSerialize;
        if (item.IsStatic)  
            this.itemToSerialize =
            Newtonsoft.Json.JsonConvert.SerializeObject(new TypeAndMethodStatic(item))
            ;
        
        else
            this.itemToSerialize =
                Newtonsoft.Json.JsonConvert.SerializeObject(new TypeAndMethodInstance(item))
                ;

    var x=    $$$""""
    {"TypeOfClass":"{{{item}}}","MethodName":"Exists","ValueArguments":[],"StringArguments":[{"IsValueType":false,"TypeAndName":"string? path","TypeArgument":"String","Name":"path"}]}
"""";

        Interlocked.Increment(ref count);
        nameFileToBeWritten = $"{item.TypeOfClass}_{item.MethodName}_{count}";
    }
    public TypeAndMethod item;
    public string? itemToSerialize { get; set; }
    //public int extraLength
    //{
    //    get
    //    {
    //        var extra = item.NameOfVariable.Length;
    //        if (extra > 0)
    //        {
    //            //acknowledge the dot
    //            extra += 1;
    //        }
    //        return extra;
    //    }
    //}

    public string Declaration
    {
        get
        {
            return $$"""

    [System.Diagnostics.DebuggerStepThrough()]
    public static {{(item.HasTaskReturnType ? "async" : "")}} {{item.TypeReturn}} {{item.MethodSignature}}({{item.ThisArgument}} {{item.ArgumentsForCallMethod}} )  
    {
        {{item.ReturnString}} {{(item.HasTaskReturnType ? "await" : "")}} {{item.CallMethod}};
    }
}                
""";
        }
    }

    public string nameFileToBeWritten { get; }

    internal string startContent = $$"""
static partial class SimpleIntercept
{
            
""";
    public List<DataForEachIntercept> dataForEachIntercepts = new();

    internal static string cntPrefix = $$""""
#pragma warning disable CS1591 
#pragma warning disable CS9113
#pragma warning disable CS8604
namespace System.Runtime.CompilerServices{
[AttributeUsage(AttributeTargets.Method,AllowMultiple =true)]
file class InterceptsLocationAttribute(string filePath, int line, int character) : Attribute
{
}
}//end namespace

namespace RSCG_InterceptorTemplate{

"""";

    internal static string cntSuffix = $$""""

}//namespace RSCG_InterceptorTemplate

"""";

    public string DataToBeWriten
    {
        get
        {
            var cnt = cntPrefix;
            cnt += startContent;
            dataForEachIntercepts.ForEach(it => cnt += it.DataToBeWriten);
            cnt += Declaration;
            cnt += cntSuffix;
            return cnt;
        }
    }

    public string NameClassToSerialize { get; }
}
