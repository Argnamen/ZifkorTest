using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class RequestQueueSystem : IInitializable, IDisposable
{
    private readonly Queue<RequestData> _requestQueue = new();
    private readonly IRequestFactory _requestFactory;
    private bool _isProcessing;
    private CancellationTokenSource _cts;

    // Инъекция фабрики через конструктор
    public RequestQueueSystem(IRequestFactory requestFactory)
    {
        _requestFactory = requestFactory;
        _cts = new CancellationTokenSource();
    }

    public async Task<T> EnqueueRequest<T>(string url, CancellationToken externalToken = default)
    {
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(
            _cts.Token,
            externalToken
        ).Token;

        var completionSource = new TaskCompletionSource<T>();

        // Использование фабрики для создания запроса
        var request = _requestFactory.CreateRequest(
            url,
            linkedToken,
            result => completionSource.SetResult((T)result),
            error => completionSource.SetException(new Exception(error))
        );

        _requestQueue.Enqueue(request);
        ProcessQueue();

        return await completionSource.Task;
    }

    private async void ProcessQueue()
    {
        if (_isProcessing) return;
        _isProcessing = true;

        while (_requestQueue.Count > 0)
        {
            var request = _requestQueue.Dequeue();
            try
            {
                using var webRequest = UnityWebRequest.Get(request.Url);
                var asyncOp = webRequest.SendWebRequest();

                while (!asyncOp.isDone)
                {
                    if (request.CancellationToken.IsCancellationRequested)
                    {
                        webRequest.Abort();
                        break;
                    }
                    await Task.Yield();
                }

                if (request.CancellationToken.IsCancellationRequested)
                    continue;

                if (webRequest.result == UnityWebRequest.Result.Success)
                    request.OnComplete?.Invoke(webRequest.downloadHandler.text);
                else
                    request.OnError?.Invoke(webRequest.error);
            }
            catch (Exception e)
            {
                request.OnError?.Invoke(e.Message);
            }
        }

        _isProcessing = false;
    }

    public void Dispose() => _cts?.Cancel();

    public void Initialize()
    {
        
    }
}