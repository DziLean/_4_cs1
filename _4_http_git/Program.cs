using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;

namespace _4_http_git
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public WebServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            _responderMethod = method;
            _listener.Start();
        }

        public WebServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = _responderMethod(ctx.Request);
                                if(rstr != "H" && rstr != "P" && rstr !="G")
                                    ctx.Response.Headers["Content-Type"] = "text/html";
                                else
                                     ctx.Response.Headers["Content-Type"] = "text/plain";
                                Console.WriteLine("\r\n| Request Headers | ");
                                for (int i = 0; i < ctx.Request.Headers.AllKeys.Length; ++i)
                                    Console.WriteLine("+ " + ctx.Request.Headers.AllKeys[i] + ": " + ctx.Request.Headers[ctx.Request.Headers.AllKeys[i]]);
                                Console.WriteLine("\r\n| Response Headers | ");
                                ctx.Response.Headers["Host"] = ctx.Request.Headers["Host"];
                                ctx.Response.Headers["Status"] = "200";
                                ctx.Response.Headers["Status-Text"] = "OK";
                                ctx.Response.Headers["User-Agent"] = ctx.Request.Headers["User-Agent"];
                                for (int i = 0; i < ctx.Response.Headers.AllKeys.Length; ++i)
                                    Console.WriteLine("+ " + ctx.Response.Headers.AllKeys[i] + ": " + ctx.Response.Headers[ctx.Response.Headers.AllKeys[i]]);
                                byte[] buf;
                                if (rstr == "H")
                                    buf = null;
                                else
                                    buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
