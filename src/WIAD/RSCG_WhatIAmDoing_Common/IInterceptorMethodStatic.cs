namespace RSCG_WhatIAmDoing_Common;

public interface IInterceptorMethodStatic
{
    abstract static string InterceptStaticMethodBefore(
        string typeAndMethodStatic,
        Dictionary<string, string?> valueValues,
        Dictionary<string, string?> stringValues,
        Dictionary<string, string?> exposeValues
        );
    abstract static void InterceptStaticMethodAfterWithoutResult(string id);
    abstract static void InterceptStaticMethodAfterWithResult(string id, object? result);
    abstract static void InterceptStaticMethodException(string id, Exception ex);
    abstract static void InterceptStaticMethodFinally(string id);
}
public interface IInterceptorMethodInstanceClass
{
    abstract static string InterceptInstanceMethodBefore<T>(
        T instance,
        string typeAndMethodStatic,
        Dictionary<string, string?> valueValues,
        Dictionary<string, string?> stringValues,
        Dictionary<string, string?> exposeValues
            );
    abstract static void InterceptInstanceMethodAfterWithoutResult(string id);
    abstract static void InterceptInstanceMethodAfterWithResult(string id, object? result);
    abstract static void InterceptInstanceMethodException(string id, Exception ex);

    abstract static void InterceptInstanceMethodFinally(string id);
}