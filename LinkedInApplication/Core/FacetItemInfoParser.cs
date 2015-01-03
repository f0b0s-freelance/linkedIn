using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinkedInApplication.Core
{
    public static class FacetItemInfoParser
    {
        public static IEnumerable<FacetItemInfo> Parse(string content)
        {
            try
            {
                var element = XDocument.Parse(content);

                return from descedant in element.Descendants("bucket")
                    let id = descedant.Descendants("code").FirstOrDefault()
                    let name = descedant.Descendants("name").FirstOrDefault()
                    select new FacetItemInfo
                           {
                               Id = id.Value,
                               Name = name.Value
                           };
            }
            catch (Exception e)
            {
                throw new XmlParsingException("Can't parse server response, message: " + e.Message);
            }
        }
    }
}