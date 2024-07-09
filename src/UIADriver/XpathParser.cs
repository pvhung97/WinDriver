using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace UIADriver
{
    public enum ResolveType
    {
        FULL_SOURCE,
        PARTIAL_SOURCE,
        NAVIGATE
    }

    public class XpathSegment
    {
        public string xpath;
        public ResolveType type;
        public Dictionary<string, object> meta;

        public XpathSegment(string xpath, ResolveType type)
        {
            this.xpath = xpath;
            this.type = type;
            this.meta = [];
        }

        public T getMeta<T>(string key)
        {
            return (T) meta[key];
        }
    }

    public class XpathParser
    {
        private static Regex Regex = new Regex(@"^\/\w+\[\d+\]$");

        public static List<XpathSegment> Parse(string xpath)
        {
            try
            {
                XPathExpression.Compile(xpath);
            }
            catch
            {
                return [new XpathSegment(xpath, ResolveType.FULL_SOURCE)];
            }

            List<XpathSegment> rs = [];
            int stage = 0;
            int i = 0;
            var sb = new StringBuilder();
            int squareBracketCount = 0;

            while (i < xpath.Length)
            {
                char c = xpath[i];

                switch (stage)
                {
                    case 0:
                        switch (c)
                        {
                            case '/':
                                if (sb.Length > 0)
                                {
                                    rs.Add(new XpathSegment(sb.ToString(), ResolveType.PARTIAL_SOURCE));
                                    sb.Clear();
                                }

                                if (i < xpath.Length - 1 && xpath[i + 1] == '/')
                                {
                                    rs.Add(new XpathSegment(xpath.Substring(i), ResolveType.FULL_SOURCE));
                                    return rs;
                                }

                                sb.Append(c);
                                i += 1;
                                break;
                            case '*':
                                rs.Add(new XpathSegment(xpath.Substring(i - sb.Length), ResolveType.FULL_SOURCE));
                                return rs;
                            case '[':
                                sb.Append(c);
                                i += 1;
                                squareBracketCount += 1;
                                stage = 1;
                                break;
                            default:
                                if (char.IsLetter(c) || char.IsDigit(c))
                                {
                                    sb.Append(c);
                                    i += 1;
                                }
                                else
                                {
                                    return [new XpathSegment(xpath, ResolveType.FULL_SOURCE)];
                                }
                                break;
                        }
                        break;
                    case 1:
                        switch (c)
                        {
                            case ']':
                                sb.Append(c);
                                i += 1;
                                squareBracketCount -= 1;
                                if (squareBracketCount == 0)
                                {
                                    stage = 0;
                                }
                                break;
                            case '[':
                                sb.Append(c);
                                i += 1;
                                squareBracketCount += 1;
                                break;
                            case '\'':
                                sb.Append(c);
                                i += 1;
                                stage = 2;
                                break;
                            case '"':
                                sb.Append(c);
                                i += 1;
                                stage = 3;
                                break;
                            default:
                                sb.Append(c);
                                i += 1;
                                break;
                        }
                        break;
                    case 2:
                        switch (c)
                        {
                            case '\\':
                                sb.Append(c);
                                i += 1;
                                if (i < xpath.Length - 1)
                                {
                                    sb.Append(xpath[i]);
                                    i += 1;
                                }
                                break;
                            case '\'':
                                sb.Append(c);
                                i += 1;
                                stage = 1;
                                break;
                            default:
                                sb.Append(c);
                                i += 1;
                                break;
                        }
                        break;
                    case 3:
                        switch (c)
                        {
                            case '\\':
                                sb.Append(c);
                                i += 1;
                                if (i < xpath.Length - 1)
                                {
                                    sb.Append(xpath[i]);
                                    i += 1;
                                }
                                break;
                            case '"':
                                sb.Append(c);
                                i += 1;
                                stage = 1;
                                break;
                            default:
                                sb.Append(c);
                                i += 1;
                                break;
                        }
                        break;
                }
            }

            if (sb.Length > 0)
            {
                rs.Add(new XpathSegment(sb.ToString(), ResolveType.PARTIAL_SOURCE));
                sb.Clear();
            }

            foreach (var item in rs)
            {
                if (item.type == ResolveType.PARTIAL_SOURCE && Regex.IsMatch(item.xpath))
                {
                    int idxOfOpenBracket = item.xpath.IndexOf('[');
                    int idxOfEndBracket = item.xpath.IndexOf("]");
                    string tagName = item.xpath.Substring(1, idxOfOpenBracket - 1);
                    int idx = int.Parse(item.xpath.Substring(idxOfOpenBracket + 1, idxOfEndBracket - idxOfOpenBracket - 1));
                    if (idx > 0)
                    {
                        item.type = ResolveType.NAVIGATE;
                        item.meta["tagName"] = tagName;
                        item.meta["index"] = idx;
                    }
                }
            }

            return rs;
        }
    }
}
