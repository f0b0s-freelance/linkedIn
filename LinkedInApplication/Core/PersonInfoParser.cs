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
                       let companyName = descedant.Descendants("company").Descendants("name").FirstOrDefault()
                       let position = descedant.Descendants("position").FirstOrDefault(x => x.Descendants("is-current").First().Value == "true")
                       let companyId = position != null ? position.Descendants("company").Descendants("id").FirstOrDefault() : null
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
