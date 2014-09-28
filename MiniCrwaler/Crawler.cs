using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiniCrwaler
{
    class Crawler
    {
        public Crawler(DataAnalyzer a, DataCrawler c, DataSaver s)
        {
            this.s = s;
            this.c = c;
            this.a = a;
        }
        public void Crwal(string start)
        {
            All.Add(start);
            URIs.Enqueue(start);
            while (URIs.Count != 0)
            {
                string URI = URIs.Dequeue();
                var ct = c.BeginCrawl(URI, (StatusCode s, byte[] e) =>
                    {
                        if (s == StatusCode.Success)
                        {
                            string str = System.Text.Encoding.GetEncoding("GB2312").GetString(e);
                            var l = a.AnalyzeForNewURIs(URI, str);
                            foreach (var t in l)
                                if (!All.Contains(t))
                                {
                                    All.Add(t);
                                    URIs.Enqueue(t);
                                }
                            this.s.SaveData(URI, e);
                        }
                    });
                while (ct.Status == StatusCode.Running)
                    Thread.Sleep(1);
            }
        }
        public Queue<string> URIs = new Queue<string>();
        public HashSet<string> All = new HashSet<string>();
        private DataSaver s;
        private DataCrawler c;
        private DataAnalyzer a;
    }
}
