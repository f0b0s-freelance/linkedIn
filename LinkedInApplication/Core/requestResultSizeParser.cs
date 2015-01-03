using System;
using System.Linq;
using System.Xml.Linq;

namespace LinkedInApplication.Core
{
    public static class RequestResultSizeParser
    {
        public static RequestResult Parse(string content)
        {
            try
            {
                var element = XDocument.Parse(content);
                var peopleNode = element.Descendants("people").FirstOrDefault();
                if (peopleNode == null)
                {
                    return new RequestResult();
                }
                else
                {
                    return new RequestResult
                           {
                               Count = Int32.Parse(peopleNode.Attribute("count").Value),
                               Total = Int32.Parse(peopleNode.Attribute("total").Value),
                               Start = Int32.Parse(peopleNode.Attribute("start").Value)
                           };
                }

            }
            catch (Exception e)
            {
                throw new XmlParsingException("Can't parse server response, message: " + e.Message);
            }
        }
    }
}
