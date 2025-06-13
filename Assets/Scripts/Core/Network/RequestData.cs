using System;
using System.Threading;

public class RequestData
{
    public string Url { get; }
    public CancellationToken CancellationToken { get; }
    public Action<object> OnComplete { get; }
    public Action<string> OnError { get; }

    public RequestData(
        string url,
        CancellationToken ct,
        Action<object> onComplete,
        Action<string> onError)
    {
        Url = url;
        CancellationToken = ct;
        OnComplete = onComplete;
        OnError = onError;
    }
}
