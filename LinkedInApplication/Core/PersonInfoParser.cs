using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LinkedInApplication.Core
{
    public static class PersonInfoParser
    {
        public static IEnumerable<PersonInfo> Parse(string content)
        {
            try
            {
                var element = XDocument.Parse(content);
                
                return from descedant in element.Descendants("person")
                       let firstName = descedant.Descendants("first-name").FirstOrDefault()
                       let lastName = descedant.Descendants("last-name").FirstOrDefault()
                       let positions = descedant.Descendants("position").Where(x => x.Descendants("is-current").First().Value == "true")
                       let companyIdNode = positions.Any() ? positions.Descendants("company").FirstOrDefault(x => x.Descendants("id").Any()) : null
                       let companyNameNode = positions.Any() ? positions.Descendants("company").FirstOrDefault(x => x.Descendants("name").Any()) : null
                       let companyId = companyIdNode != null ? companyIdNode.Descendants("id").FirstOrDefault() : null
                       let companyName = companyNameNode != null ? companyNameNode.Descendants("name").FirstOrDefault() : null
                       let headline = descedant.Descendants("headline").FirstOrDefault()
                       let location = descedant.Descendants("location").Descendants("name").FirstOrDefault()
                       select new PersonInfo
                       {
                           FirstName = firstName != null ? firstName.Value : string.Empty,
                           LastName = lastName != null ? lastName.Value : string.Empty,
                           Headline = headline != null ? headline.Value : string.Empty,
                           CompanyName = companyName != null ? companyName.Value : string.Empty,
                           CompanyId = companyId != null ? companyId.Value : string.Empty,
                           CompanyLocation = location != null ? location.Value : string.Empty
                       };
            }
            catch (Exception e)
            {
                throw new XmlParsingException("Can't parse server response, message: " + e.Message);
            }
        }
    }
}
