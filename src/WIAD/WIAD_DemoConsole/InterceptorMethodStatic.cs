using Microsoft.Extensions.Caching.Memory;
using RSCG_WhatIAmDoing_Common;

namespace WIAD_DemoConsole;

[ExposeClass(typeof(Encoding), nameof(Encoding.EncodingName))]
[InterceptStatic("System.IO.File.*ts")]
[InterceptStatic("System.IO.File.*")]
//[InterceptStatic("System.Console.*")]
[InterceptStatic("WIAD_DemoConsole.Fib.*")]
internal class InterceptorMethodStatic : InterceptorMethodStaticBase, IInterceptorMethodStatic
{
    
}
