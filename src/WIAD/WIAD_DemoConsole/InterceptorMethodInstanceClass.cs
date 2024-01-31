using Microsoft.Extensions.Caching.Memory;
using RSCG_WhatIAmDoing_Common;
namespace WIAD_DemoConsole;

//[InterceptInstanceClass(typeof(Person),"ame")]
//[InterceptInstanceClass(typeof(Person), "parat")]
//[InterceptInstanceClass(typeof(Person), "ncodi")]
[InterceptInstanceClass(typeof(Person), ".*")]

public class InterceptorMethodInstanceClass: InterceptorMethodInstanceClassBase, IInterceptorMethodInstanceClass
{
    
    public InterceptorMethodInstanceClass()
    {
        
    }

}
