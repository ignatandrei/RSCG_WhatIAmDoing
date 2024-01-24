
namespace RSCG_WhatIAmDoing;

class MethodsToIntercept
{
    public MethodsToIntercept()
    {
        List<MethodToIntercept> methodToIntercepts = new()
        {
            //new MethodToIntercept { 
            //    ClassName = "System.IO.File", 
            //    MethodName = "WriteAllText",
            //    IsStatic = true
            //},
            new MethodToIntercept {
                ClassName = "System.IO.File",
                MethodName = "Exists",
                IsStatic = true
            },
            //new MethodToIntercept {
            //    ClassName = "System.IO.File",
            //    MethodName = "Delete",
            //    IsStatic = true
            //}
        };   
        Methods = methodToIntercepts
            .Where(static m => m is not null)
            .Where(static m => m!.IsValid())
            .ToArray();
    }
    public MethodToIntercept[] Methods { get; set; } = new MethodToIntercept[0];
}
