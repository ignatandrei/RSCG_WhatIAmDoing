namespace RSCG_WhatIAmDoing_Common;
public interface IInterceptorMethodAfter
{
    abstract static void InterceptMethodAfterWithoutResult(string id);
    abstract static void InterceptMethodAfterWithResult(string id, object? result);
    abstract static void InterceptMethodException(string id, Exception ex);
    abstract static void InterceptMethodFinally(string id);

}
public interface IInterceptorMethodStatic: IInterceptorMethodAfter
{
    abstract static string InterceptStaticMethodBefore(
        string typeAndMethodStatic,
        Dictionary<string, string?> valueValues,
        Dictionary<string, string?> stringValues,
        Dictionary<string, string?> exposeValues
        );
}
public interface IInterceptorMethodInstanceClass: IInterceptorMethodAfter
{
    abstract static string InterceptInstanceMethodBefore<T>(
        T instance,
        string typeAndMethodStatic,
        Dictionary<string, string?> valueValues,
        Dictionary<string, string?> stringValues,
        Dictionary<string, string?> exposeValues
            );
    
}