using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniCrwaler
{
    public enum StatusCode
    {
        Success,
        Error,
        Running,
        Terminated
    }
    interface DataCrawlerControl
    {
        StatusCode Status { get; }
        void Abort();
    }
    interface DataCrawler
    {
        DataCrawlerControl BeginCrawl(string URI, Action<StatusCode, byte[]> Callback);
    }
    class WebDataCrawler
        : DataCrawler
    {
        private class WebDataCrawlerControl
            : DataCrawlerControl
        {
            public WebDataCrawlerControl(Thread crawlingThread)
            {
                this.crawlingThread = crawlingThread;
                status = StatusCode.Terminated;
            }
            public void Abort()
            {
                if (crawlingThread.IsAlive)
                {
                    crawlingThread.Abort();
                    status = StatusCode.Terminated;
                }
            }
            public StatusCode Status { get { return status; } }
            private Thread crawlingThread;
            public StatusCode status;
        }

        public DataCrawlerControl BeginCrawl(string URI, Action<StatusCode, byte[]> Callback)
        {
            WebDataCrawlerControl control = null;
            var thread = new Thread(() =>
            {
                try
                {
                    var HTTPRequest = WebRequest.Create(new Uri(URI));
                    var HTTPResopnse = HTTPRequest.GetResponse();
                    byte[] data = new byte[HTTPResopnse.ContentLength];
                    int length = (int)HTTPResopnse.ContentLength;
                    var stream = HTTPResopnse.GetResponseStream();
                    int offset = 0;
                    while (length > 0)
                    {
                        var numRead = stream.Read(data, offset, length);
                        offset += numRead;
                        length -= numRead;
                        if (numRead == 0) break;
                    }
                    
                    Callback(StatusCode.Success, data);
                    control.status = StatusCode.Success;
                }
                catch (Exception e)
                {
                    Logger.Log(string.Format("Crawl {0} Error: {1}", URI, e.ToString()));
                    Callback(StatusCode.Error, null);
                    control.status = StatusCode.Error;

                }
            });
            control = new WebDataCrawlerControl(thread);
            thread.Start();
            control.status = StatusCode.Running;
            return control;
        }
    }
}
