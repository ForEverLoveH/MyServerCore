﻿using System.Net;
using MyServerCode.Summer.Service.Http;
using MyServerCode.Summer.Service.SSL;
using MyServerCode.Summer.Service.SSL.Client;

namespace MyServerCode.Summer.Service.Https;

 public class HttpsClient : SslClient
 {
     /// <summary>
     /// Initialize HTTPS client with a given IP address and port number
     /// </summary>
     /// <param name="context">SSL context</param>
     /// <param name="address">IP address</param>
     /// <param name="port">Port number</param>
     public HttpsClient(SslContext context, IPAddress address, int port) : base(context, address, port) { Request = new HttpRequest(); Response = new HttpResponse(); }
     /// <summary>
     /// Initialize HTTPS client with a given IP address and port number
     /// </summary>
     /// <param name="context">SSL context</param>
     /// <param name="address">IP address</param>
     /// <param name="port">Port number</param>
     public HttpsClient(SslContext context, string address, int port) : base(context, address, port) { Request = new HttpRequest(); Response = new HttpResponse(); }
     /// <summary>
     /// Initialize HTTPS client with a given DNS endpoint
     /// </summary>
     /// <param name="context">SSL context</param>
     /// <param name="endpoint">DNS endpoint</param>
     public HttpsClient(SslContext context, DnsEndPoint endpoint) : base(context, endpoint) { Request = new HttpRequest(); Response = new HttpResponse(); }
     /// <summary>
     /// Initialize HTTPS client with a given IP endpoint
     /// </summary>
     /// <param name="context">SSL context</param>
     /// <param name="endpoint">IP endpoint</param>
     public HttpsClient(SslContext context, IPEndPoint endpoint) : base(context, endpoint) { Request = new HttpRequest(); Response = new HttpResponse(); }

     /// <summary>
     /// Get the HTTP request
     /// </summary>
     public HttpRequest Request { get; protected set; }

     /// <summary>
     /// Get the HTTP response
     /// </summary>
     protected HttpResponse Response { get; set; }

     #region Send request / Send request body

     /// <summary>
     /// Send the current HTTP request (synchronous)
     /// </summary>
     /// <returns>Size of sent data</returns>
     public long SendRequest() => SendRequest(Request);
     /// <summary>
     /// Send the HTTP request (synchronous)
     /// </summary>
     /// <param name="request">HTTP request</param>
     /// <returns>Size of sent data</returns>
     public long SendRequest(HttpRequest request) => Send(request.Cache.Data, request.Cache.Offset, request.Cache.Size);

     /// <summary>
     /// Send the HTTP request body (synchronous)
     /// </summary>
     /// <param name="body">HTTP request body</param>
     /// <returns>Size of sent data</returns>
     public long SendRequestBody(string body) => Send(body);
     /// <summary>
     /// Send the HTTP request body (synchronous)
     /// </summary>
     /// <param name="body">HTTP request body as a span of characters</param>
     /// <returns>Size of sent data</returns>
     public long SendRequestBody(ReadOnlySpan<char> body) => Send(body);
     /// <summary>
     /// Send the HTTP request body (synchronous)
     /// </summary>
     /// <param name="buffer">HTTP request body buffer</param>
     /// <returns>Size of sent data</returns>
     public long SendRequestBody(byte[] buffer) => Send(buffer);
     /// <summary>
     /// Send the HTTP request body (synchronous)
     /// </summary>
     /// <param name="buffer">HTTP request body buffer</param>
     /// <param name="offset">HTTP request body buffer offset</param>
     /// <param name="size">HTTP request body size</param>
     /// <returns>Size of sent data</returns>
     public long SendRequestBody(byte[] buffer, long offset, long size) => Send(buffer, offset, size);

     /// <summary>
     /// Send the HTTP request body (synchronous)
     /// </summary>
     /// <param name="buffer">HTTP request body buffer as a span of bytes</param>
     /// <returns>Size of sent data</returns>
     public long SendRequestBody(ReadOnlySpan<byte> buffer) => Send(buffer);

     /// <summary>
     /// Send the current HTTP request (asynchronous)
     /// </summary>
     /// <returns>'true' if the current HTTP request was successfully sent, 'false' if the session is not connected</returns>
     public bool SendRequestAsync() => SendRequestAsync(Request);
     /// <summary>
     /// Send the HTTP request (asynchronous)
     /// </summary>
     /// <param name="request">HTTP request</param>
     /// <returns>'true' if the current HTTP request was successfully sent, 'false' if the session is not connected</returns>
     public bool SendRequestAsync(HttpRequest request) => SendAsync(request.Cache.Data, request.Cache.Offset, request.Cache.Size);

     /// <summary>
     /// Send the HTTP request body (asynchronous)
     /// </summary>
     /// <param name="body">HTTP request body</param>
     /// <returns>'true' if the HTTP request body was successfully sent, 'false' if the session is not connected</returns>
     public bool SendRequestBodyAsync(string body) => SendAsync(body);
     /// <summary>
     /// Send the HTTP request body (asynchronous)
     /// </summary>
     /// <param name="body">HTTP request body as a span of characters</param>
     /// <returns>'true' if the HTTP request body was successfully sent, 'false' if the session is not connected</returns>
     public bool SendRequestBodyAsync(ReadOnlySpan<char> body) => SendAsync(body);
     /// <summary>
     /// Send the HTTP request body (asynchronous)
     /// </summary>
     /// <param name="buffer">HTTP request body buffer</param>
     /// <returns>'true' if the HTTP request body was successfully sent, 'false' if the session is not connected</returns>
     public bool SendRequestBodyAsync(byte[] buffer) => SendAsync(buffer);
     /// <summary>
     /// Send the HTTP request body (asynchronous)
     /// </summary>
     /// <param name="buffer">HTTP request body buffer</param>
     /// <param name="offset">HTTP request body buffer offset</param>
     /// <param name="size">HTTP request body size</param>
     /// <returns>'true' if the HTTP request body was successfully sent, 'false' if the session is not connected</returns>
     public bool SendRequestBodyAsync(byte[] buffer, long offset, long size) => SendAsync(buffer, offset, size);
     /// <summary>
     /// Send the HTTP request body (asynchronous)
     /// </summary>
     /// <param name="buffer">HTTP request body buffer as a span of bytes</param>
     /// <returns>'true' if the HTTP request body was successfully sent, 'false' if the session is not connected</returns>
     public bool SendRequestBodyAsync(ReadOnlySpan<byte> buffer) => SendAsync(buffer);

     #endregion

     #region Session handlers

     protected override void OnReceived(byte[] buffer, long offset, long size)
     {
         // Receive HTTP response header
         if (Response.IsPendingHeader())
         {
             if (Response.ReceiveHeader(buffer, (int)offset, (int)size))
                 OnReceivedResponseHeader(Response);

             size = 0;
         }

         // Check for HTTP response error
         if (Response.IsErrorSet)
         {
             OnReceivedResponseError(Response, "Invalid HTTP response!");
             Response.Clear();
             Disconnect();
             return;
         }

         // Receive HTTP response body
         if (Response.ReceiveBody(buffer, (int)offset, (int)size))
         {
             OnReceivedResponse(Response);
             Response.Clear();
             return;
         }

         // Check for HTTP response error
         if (Response.IsErrorSet)
         {
             OnReceivedResponseError(Response, "Invalid HTTP response!");
             Response.Clear();
             Disconnect();
             return;
         }
     }

     protected override void OnDisconnected()
     {
         // Receive HTTP response body
         if (Response.IsPendingBody())
         {
             OnReceivedResponse(Response);
             Response.Clear();
             return;
         }
     }

     /// <summary>
     /// Handle HTTP response header received notification
     /// </summary>
     /// <remarks>Notification is called when HTTP response header was received from the server.</remarks>
     /// <param name="response">HTTP request</param>
     protected virtual void OnReceivedResponseHeader(HttpResponse response) {}

     /// <summary>
     /// Handle HTTP response received notification
     /// </summary>
     /// <remarks>Notification is called when HTTP response was received from the server.</remarks>
     /// <param name="response">HTTP response</param>
     protected virtual void OnReceivedResponse(HttpResponse response) {}

     /// <summary>
     /// Handle HTTP response error notification
     /// </summary>
     /// <remarks>Notification is called when HTTP response error was received from the server.</remarks>
     /// <param name="response">HTTP response</param>
     /// <param name="error">HTTP response error</param>
     protected virtual void OnReceivedResponseError(HttpResponse response, string error) {}

     #endregion
 }

  