using System.Net;
using MyServerCode.Summer.Service.Http;
using MyServerCode.Summer.Service.Https;
using MyServerCode.Summer.Service.SSL;

namespace MyServerCore.Server.Https.Client;

/// <summary>
 /// HTTPS extended client make requests to HTTPS Web server with returning Task as a synchronization primitive.
 /// </summary>
 /// <remarks>Thread-safe.</remarks>
 public class CHttpsClient : HttpsClient
 {
     /// <summary>
     /// Initialize HTTPS client with a given IP address and port number
     /// </summary>
     /// <param name="context">SSL context</param>
     /// <param name="address">IP address</param>
     /// <param name="port">Port number</param>
     public CHttpsClient(SslContext context, IPAddress address, int port) : base(context, address, port) {}
     /// <summary>
     /// Initialize HTTPS client with a given IP address and port number
     /// </summary>
     /// <param name="context">SSL context</param>
     /// <param name="address">IP address</param>
     /// <param name="port">Port number</param>
     public CHttpsClient(SslContext context, string address, int port) : base(context, address, port) {}
     /// <summary>
     /// Initialize HTTPS client with a given DNS endpoint
     /// </summary>
     /// <param name="context">SSL context</param>
     /// <param name="endpoint">DNS endpoint</param>
     public CHttpsClient(SslContext context, DnsEndPoint endpoint) : base(context, endpoint) {}
     /// <summary>
     /// Initialize HTTPS client with a given IP endpoint
     /// </summary>
     /// <param name="context">SSL context</param>
     /// <param name="endpoint">IP endpoint</param>
     public CHttpsClient(SslContext context, IPEndPoint endpoint) : base(context, endpoint) {}

     #region Send request

     /// <summary>
     /// Send current HTTP request
     /// </summary>
     /// <param name="timeout">Current HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendRequest(TimeSpan? timeout = null) => SendRequest(Request, timeout);
     /// <summary>
     /// Send HTTP request
     /// </summary>
     /// <param name="request">HTTP request</param>
     /// <param name="timeout">HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendRequest(HttpRequest request, TimeSpan? timeout = null)
     {
         timeout ??= TimeSpan.FromMinutes(1);

         _tcs = new TaskCompletionSource<HttpResponse>();
         Request = request;

         // Check if the HTTP request is valid
         if (Request.IsEmpty || Request.IsErrorSet)
         {
             SetResultError("Invalid HTTP request!");
             return _tcs.Task;
         }

         if (!IsHandshaked)
         {
             // Connect to the Web server
             if (!ConnectAsync())
             {
                 SetResultError("Connection failed!");
                 return _tcs.Task;
             }
         }
         else
         {
             // Send prepared HTTP request
             if (!SendRequestAsync())
             {
                 SetResultError("Failed to send HTTP request!");
                 return _tcs.Task;
             }
         }

         void TimeoutHandler(object state)
         {
             // Disconnect on timeout
             OnReceivedResponseError(Response, "Timeout!");
             Response.Clear();
             DisconnectAsync();
         }

         // Create a new timeout timer
         if (_timer == null)
             _timer = new Timer(TimeoutHandler, null, Timeout.Infinite, Timeout.Infinite);

         // Start the timeout timer
         _timer.Change((int)timeout.Value.TotalMilliseconds, Timeout.Infinite);

         return _tcs.Task;
     }

     /// <summary>
     /// Send HEAD request
     /// </summary>
     /// <param name="url">URL to request</param>
     /// <param name="timeout">Current HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendHeadRequest(string url, TimeSpan? timeout = null) => SendRequest(Request.MakeHeadRequest(url), timeout);
     /// <summary>
     /// Send GET request
     /// </summary>
     /// <param name="url">URL to request</param>
     /// <param name="timeout">Current HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendGetRequest(string url, TimeSpan? timeout = null) => SendRequest(Request.MakeGetRequest(url), timeout);
     /// <summary>
     /// Send POST request
     /// </summary>
     /// <param name="url">URL to request</param>
     /// <param name="content">Content</param>
     /// <param name="timeout">Current HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendPostRequest(string url, string content, TimeSpan? timeout = null) => SendRequest(Request.MakePostRequest(url, content), timeout);
     /// <summary>
     /// Send PUT request
     /// </summary>
     /// <param name="url">URL to request</param>
     /// <param name="content">Content</param>
     /// <param name="timeout">Current HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendPutRequest(string url, string content, TimeSpan? timeout = null) => SendRequest(Request.MakePutRequest(url, content), timeout);
     /// <summary>
     /// Send DELETE request
     /// </summary>
     /// <param name="url">URL to request</param>
     /// <param name="timeout">Current HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendDeleteRequest(string url, TimeSpan? timeout = null) => SendRequest(Request.MakeDeleteRequest(url), timeout);
     /// <summary>
     /// Send OPTIONS request
     /// </summary>
     /// <param name="url">URL to request</param>
     /// <param name="timeout">Current HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendOptionsRequest(string url, TimeSpan? timeout = null) => SendRequest(Request.MakeOptionsRequest(url), timeout);
     /// <summary>
     /// Send TRACE request
     /// </summary>
     /// <param name="url">URL to request</param>
     /// <param name="timeout">Current HTTP request timeout (default is 1 minute)</param>
     /// <returns>HTTP request Task</returns>
     public Task<HttpResponse> SendTraceRequest(string url, TimeSpan? timeout = null) => SendRequest(Request.MakeTraceRequest(url), timeout);

     #endregion

     #region Session handlers

     protected override void OnHandshaked()
     {
         // Send prepared HTTP request on connect
         if (!Request.IsEmpty && !Request.IsErrorSet)
             if (!SendRequestAsync())
                 SetResultError("Failed to send HTTP request!");
     }

     protected override void OnDisconnected()
     {
         // Cancel timeout check timer
         _timer?.Change(Timeout.Infinite, Timeout.Infinite);

         base.OnDisconnected();
     }

     protected override void OnReceivedResponse(HttpResponse response)
     {
         // Cancel timeout check timer
         _timer?.Change(Timeout.Infinite, Timeout.Infinite);

         SetResultValue(response);
     }

     protected override void OnReceivedResponseError(HttpResponse response, string error)
     {
         // Cancel timeout check timer
         _timer?.Change(Timeout.Infinite, Timeout.Infinite);

         SetResultError(error);
     }

     #endregion

     private TaskCompletionSource<HttpResponse> _tcs = new TaskCompletionSource<HttpResponse>();
     private Timer _timer;

     private void SetResultValue(HttpResponse response)
     {
         Response = new HttpResponse();
         _tcs.SetResult(response);
         Request.Clear();
     }

     private void SetResultError(string error)
     {
         _tcs.SetException(new Exception(error));
         Request.Clear();
     }

     #region IDisposable implementation

     // Disposed flag.
     private bool _disposed;

     protected override void Dispose(bool disposingManagedResources)
     {
         if (!_disposed)
         {
             if (disposingManagedResources)
             {
                 // Dispose managed resources here...
                 _timer?.Dispose();
             }

             // Dispose unmanaged resources here...

             // Set large fields to null here...

             // Mark as disposed.
             _disposed = true;
         }

         // Call Dispose in the base class.
         base.Dispose(disposingManagedResources);
     }

     // The derived class does not have a Finalize method
     // or a Dispose method without parameters because it inherits
     // them from the base class.

     #endregion
 }