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
                       let company = descedant.Descendants("company").Descendants("name").FirstOrDefault()
                       let headline = descedant.Descendants("headline").FirstOrDefault()
                       select new PersonInfo
                       {
                           FirstName = firstName != null ? firstName.Value : string.Empty,
                           LastName = lastName != null ? lastName.Value : string.Empty,
                           Headline = headline != null ? headline.Value : string.Empty,
                           Company = company != null ? company.Value : string.Empty,
                       };
            }
            catch (Exception e)
            {
                throw new XmlParsingException("Can't parse server response, message: " + e.Message);
            }
        }
    }
}
