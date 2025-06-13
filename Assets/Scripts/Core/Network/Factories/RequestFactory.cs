using System;
using System.Threading;

public class RequestFactory : IRequestFactory
{
    public RequestData CreateRequest(
        string url,
        CancellationToken ct,
        Action<object> onSuccess,
        Action<string> onError)
    {
        return new RequestData(url, ct, onSuccess, onError);
    }
}
