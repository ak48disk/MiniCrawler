using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCrwaler
{
    class HTMLNode
    {
        public List<HTMLNode> Children { get { return children; } }
        public Dictionary<string, string> Attributes { get { return attributes; } }
        public string Type { get; set; }
        private List<HTMLNode> children = new List<HTMLNode>();
        private Dictionary<string, string> attributes = new Dictionary<string, string>();
        public IEnumerable<HTMLNode> ThisAndAllChildrens()
        {
            foreach (var child in Children)
            {
                foreach (var childchild in child.ThisAndAllChildrens())
                {
                    yield return childchild;
                }
            }
            yield return this;
        }
    }
    class SimpleHTMLParser
    {
        class Lexer
        {
            public Lexer(string HTMLText)
            {
                text = HTMLText;
                textLen = text.Length;
                pos = 0;
            }

            public void Push()
            {
                stack.Push(pos);
            }

            public void Pop()
            {
                pos = stack.Pop();
            }

            public string Next()
            {
                string curr = "";
                if (pos < textLen)
                {
                    char ch = text[pos++];
                    if (ch == '<' || ch == '>' || ch == '/' || ch == '=')
                        return ch.ToString();
                    if (ch == '\'' || ch == '"')
                    {
                        curr += ch;
                        while (pos < textLen && text[pos] != ch)
                        {
                            curr += text[pos++];
                        }
                        if (pos < textLen && text[pos] == ch)
                        {
                            curr += text[pos++];
                        }
                        return curr;
                    }
                    pos--;
                    while ((ch != ' ' && ch != '\t' &&
                        ch != '<' && ch != '>' && ch != '/' && ch != '\'' && ch != '"' && ch != '='))
                    {
                        curr += ch; pos++;
                        if (pos >= textLen) return curr;
                        ch = text[pos];
                    }
                    if (curr != "") return curr;
                    while (ch == ' ' || ch == '\t' || ch =='\r' || ch == '\n')
                    {
                        curr += ch; pos++;
                        if (pos >= textLen) return curr;
                        ch = text[pos];
                    }
                    return curr;
                }
                return "";
            }
            private string text;
            private int textLen;
            private int pos;
            private Stack<int> stack = new Stack<int>();
        }
        public SimpleHTMLParser(string HTMLText)
        {
            l = new Lexer(HTMLText);
            lookAhead = l.Next();
        }
        public List<HTMLNode> Parse()
        {
            List<HTMLNode> nodes = new List<HTMLNode>();
            while (lookAhead != "")
            {
                var newNode = Node();
                if (newNode != null)
                    nodes.Add(newNode);
            }
            return nodes;
        }
        public void SkipSpaces()
        {
            while (lookAhead.Trim() == "" && lookAhead != "") Accept();
        }
        public string End(bool recover)
        {
            string oldLookAhead = lookAhead;
            l.Push();
            SkipSpaces();
            if (lookAhead == "<")
            {
                SkipSpaces(); Accept();
                if (lookAhead == "/")
                {
                    Accept();
                    SkipSpaces();
                    string labelName = Accept();
                    if (recover) 
                    {
                        l.Pop();
                        lookAhead = oldLookAhead;
                        return labelName;
                    }
                    while (lookAhead != ">" && lookAhead != "") Accept();
                    Accept();
                   
                    return labelName;
                }
            }
            l.Pop();
            lookAhead = oldLookAhead;
            return "";
        }
        public HTMLNode Node()
        {
            HTMLNode node = new HTMLNode();
            if (lookAhead == "<")
            {
                bool endFound = false;
                Accept(); SkipSpaces();
                node.Type = Accept().Trim();
                while (lookAhead != ">" && lookAhead != "")
                {
                    SkipSpaces();
                    if (lookAhead == "/")
                    {
                        Accept();
                        endFound = true;
                    }
                    if (endFound)
                    {
                        SkipSpaces();
                        if (lookAhead == ">") Accept();
                        return node;
                    }
                    string attribName = Accept().Trim();
                    string attribValue = "";
                    
                    if (lookAhead == "=")
                    {
                        Accept(); SkipSpaces();
                        attribValue = Accept().Trim();
                    }
                    node.Attributes[attribName] = attribValue;
                }
                if (lookAhead == ">") Accept();
                while (lookAhead != "")
                {
                    string type = End(true);
                    if (type != "")
                    {
                        if (type.Trim() == node.Type)
                            End(false);
                        break;
                    }
                    var newNode = Node();
                    if (newNode != null)
                        node.Children.Add(newNode);
                }
                return node;
            }
            else
            {
                node.Type = Accept();
                if (node.Type.Trim().Replace("\n","").Replace("\r","") == "")
                    return null;
            }
            return node;
        }
        private string Accept()
        {
            string r = lookAhead;
            lookAhead = l.Next();
            return r;
        }
        private Lexer l;
        private string lookAhead;
    }
}
