using System.Net;
using MyServerCode.Summer.Common;
using MyServerCode.Summer.Service.Tcp;

namespace MyServerCode.Summer.Service.Http;

public class HttpService : TcpService
{
    /// <summary>
    /// Initialize HTTP server with a given IP address and port number
    /// </summary>
    /// <param name="address">IP address</param>
    /// <param name="port">Port number</param>
    public HttpService(IPAddress address, int port) : base(address, port) { Cache = new FileCache(); }
    /// <summary>
    /// Initialize HTTP server with a given IP address and port number
    /// </summary>
    /// <param name="address">IP address</param>
    /// <param name="port">Port number</param>
    public HttpService(string address, int port) : base(address, port) { Cache = new FileCache(); }
    /// <summary>
    /// Initialize HTTP server with a given DNS endpoint
    /// </summary>
    /// <param name="endpoint">DNS endpoint</param>
    public HttpService(DnsEndPoint endpoint) : base(endpoint) { Cache = new FileCache(); }
    /// <summary>
    /// Initialize HTTP server with a given IP endpoint
    /// </summary>
    /// <param name="endpoint">IP endpoint</param>
    public HttpService(IPEndPoint endpoint) : base(endpoint) { Cache = new FileCache(); }

    /// <summary>
    /// Get the static content cache
    /// </summary>
    public FileCache Cache { get; }

    /// <summary>
    /// Add static content cache
    /// </summary>
    /// <param name="path">Static content path</param>
    /// <param name="prefix">Cache prefix (default is "/")</param>
    /// <param name="filter">Cache filter (default is "*.*")</param>
    /// <param name="timeout">Refresh cache timeout (default is 1 hour)</param>
    public void AddStaticContent(string path, string prefix = "/", string filter = "*.*", TimeSpan? timeout = null)
    {
        timeout ??= TimeSpan.FromHours(1);

        bool Handler(FileCache cache, string key, byte[] value, TimeSpan timespan)
        {
            var response = new HttpResponse();
            response.SetBegin(200);
            response.SetContentType(Path.GetExtension(key));
            response.SetHeader("Cache-Control", $"max-age={timespan.Seconds}");
            response.SetBody(value);
            return cache.Add(key, response.Cache.Data, timespan);
        }

        Cache.InsertPath(path, prefix, filter, timeout.Value, Handler);
    }
    /// <summary>
    /// Remove static content cache
    /// </summary>
    /// <param name="path">Static content path</param>
    public void RemoveStaticContent(string path) { Cache.RemovePath(path); }
    /// <summary>
    /// Clear static content cache
    /// </summary>
    public void ClearStaticContent() { Cache.Clear(); }

    protected override TcpSession CreateSession() { return new HttpSession(this); }

    #region IDisposable implementation

    // Disposed flag.
    private bool _disposed;

    protected override void Dispose(bool disposingManagedResources)
    {
        if (!_disposed)
        {
            if (disposingManagedResources) Cache.Dispose();
             
            _disposed = true;
        }

        // Call Dispose in the base class.
        base.Dispose(disposingManagedResources);
    }

    #endregion
}