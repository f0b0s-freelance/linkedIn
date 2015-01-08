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
          var result = new RequestResult
          {
            Count = peopleNode.Attribute("count") == null ? 0 : Int32.Parse(peopleNode.Attribute("count").Value),
            Total = peopleNode.Attribute("total") == null ? 0 : Int32.Parse(peopleNode.Attribute("total").Value),
            Start = peopleNode.Attribute("start") == null ? 0 : Int32.Parse(peopleNode.Attribute("start").Value)
          };

          if (result.Total != 0 && result.Count == 0)
          {
            result.Count = result.Total;
          }

          return result;
        }
      }
      catch (Exception e)
      {
        throw new XmlParsingException("Can't parse server response, message: " + e.Message);
      }
    }
  }
}
