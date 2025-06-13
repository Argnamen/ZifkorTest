using System;
using System.Threading;

public interface IRequestFactory
{
    RequestData CreateRequest(
        string url,
        CancellationToken ct,
        Action<object> onSuccess,
        Action<string> onError
    );
}