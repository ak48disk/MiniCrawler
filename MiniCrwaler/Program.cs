using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCrwaler
{
    class Program
    {
        static void Main(string[] args)
        {
            /*http://stackoverflow.com/questions/56107/what-is-the-best-way-to-parse-html-in-c*/
            new SimpleHTMLParser("<html><head>aaa</head></html>").Parse();
            new Crawler(new HTMLDataAnalyzer(), new WebDataCrawler(), new DiskDataSaver()).
                Crwal("http://www.dgbrg.com/13P/index.html");
        }
    }
}
