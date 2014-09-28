using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCrwaler
{
    interface DataAnalyzer
    {
        HashSet<string> AnalyzeForNewURIs(string URI, string content);
    }
    class HTMLDataAnalyzer
        : DataAnalyzer
    {
        private string NormalizeURI(string srcURI, string targetURI)
        {
            if (targetURI.Length >= 2)
            {
                char ch = targetURI[0];
                if (ch == '\'' || ch == '"')
                {
                    targetURI = targetURI.Substring(1, targetURI[targetURI.Length - 1] == ch ?
                        targetURI.Length - 2 : targetURI.Length - 1);
                }
            }
            if (targetURI.Contains("#"))
            {
                targetURI = targetURI.Substring(0, targetURI.IndexOf("#"));
                if (targetURI == "") return srcURI;
            }
            if (!targetURI.Contains("://"))
            {
                var pos = targetURI.IndexOf('/');
                if (pos != -1)
                {
                    string firstSegment = targetURI.Substring(0, pos);
                    if (!firstSegment.Contains("."))
                    {
                        if (pos == 0)
                            targetURI = srcURI.Substring(0, srcURI.IndexOf("/")) + targetURI;
                        else
                            targetURI =
                        srcURI.Substring(0, srcURI.LastIndexOf("/") + 1) + targetURI;
                    }
                }
                else
                {
                     targetURI = 
                        srcURI.Substring(0, srcURI.LastIndexOf("/") + 1) + targetURI;
                }
                if (!srcURI.Contains("://"))
                    return "http://" + targetURI;
                else
                    if (!targetURI.Contains("://"))
                        return srcURI.Substring(0, srcURI.IndexOf("://") + 3) + targetURI;
                    else
                        return targetURI;
            }
            return targetURI;
        }
        public HashSet<string> AnalyzeForNewURIs(string URI, string content)
        {
            var parser = new SimpleHTMLParser(content);
            var result = parser.Parse();
            HashSet<string> newURIs = new HashSet<string>();
            foreach (var node1 in result)
            {
                foreach (var node in
                    node1.ThisAndAllChildrens())
                {
                    if (node.Attributes.ContainsKey("src"))
                    {
                        var newURI = NormalizeURI(URI, node.Attributes["src"]);
                        if (!newURIs.Contains(newURI))
                            newURIs.Add(newURI);
                    }
                    if (node.Attributes.ContainsKey("href"))
                    {
                        var newURI = NormalizeURI(URI, node.Attributes["href"]);
                        if (!newURIs.Contains(newURI))
                            newURIs.Add(newURI);
                    }
                }
                    
            }
            return newURIs;
        }
    }
}
