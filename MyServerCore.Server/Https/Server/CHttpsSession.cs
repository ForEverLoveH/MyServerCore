using System.Net.Sockets;
using MyServerCode.Summer.Service.Http;
using MyServerCode.Summer.Service.Https;
using MyServerCore.Server.Http.Server;

namespace MyServerCore.Server.Https.Server;

public class CHttpsSession :HttpsSession
{
    public CHttpsSession (HttpsServer server) : base(server) {}
    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    protected override void OnReceivedRequest(HttpRequest request)
    {
        // Show HTTP request content
        Console.WriteLine(request);

        // Process HTTP request methods
        if (request.Method == "HEAD")
            SendResponseAsync(Response.MakeHeadResponse());
        else if (request.Method == "GET")
        {
            string key = request.Url;

            // Decode the key value
            key = Uri.UnescapeDataString(key);
            key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
            key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);

            if (string.IsNullOrEmpty(key))
            {
                // Response with all cache values
                SendResponseAsync(Response.MakeGetResponse(CommonCache.GetInstance().GetAllCache(), "application/json; charset=UTF-8"));
            }
            // Get the cache value by the given key
            else if (CommonCache.GetInstance().GetCacheValue(key, out var value))
            {
                // Response with the cache value
                SendResponseAsync(Response.MakeGetResponse(value));
            }
            else
                SendResponseAsync(Response.MakeErrorResponse(404, "Required cache value was not found for the key: " + key));
        }
        else if ((request.Method == "POST") || (request.Method == "PUT"))
        {
            string key = request.Url;
            string value = request.Body;

            // Decode the key value
            key = Uri.UnescapeDataString(key);
            key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
            key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);

            // Put the cache value
            CommonCache.GetInstance().PutCacheValue(key, value);

            // Response with the cache value
            SendResponseAsync(Response.MakeOkResponse());
        }
        else if (request.Method == "DELETE")
        {
            string key = request.Url;

            // Decode the key value
            key = Uri.UnescapeDataString(key);
            key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
            key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);

            // Delete the cache value
            if (CommonCache.GetInstance().DeleteCacheValue(key, out var value))
            {
                // Response with the cache value
                SendResponseAsync(Response.MakeGetResponse(value));
            }
            else
                SendResponseAsync(Response.MakeErrorResponse(404, "Deleted cache value was not found for the key: " + key));
        }
        else if (request.Method == "OPTIONS")
            SendResponseAsync(Response.MakeOptionsResponse());
        else if (request.Method == "TRACE")
            SendResponseAsync(Response.MakeTraceResponse(request));
        else
            SendResponseAsync(Response.MakeErrorResponse("Unsupported HTTP method: " + request.Method));
    }

    protected override void OnReceivedRequestError(HttpRequest request, string error)
    {
        Console.WriteLine($"Request error: {error}");
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTPS session caught an error: {error}");
    }
    
}