using System;
using System.Linq;
using System.Xml.Linq;

namespace LinkedInApplication.Core
{
    public static class CompanyInfoParser
    {
        public static CompanyInfo ParseCompanyInfo(string content)
        {
            try
            {
                var element = XDocument.Parse(content);

                return (from descedant in element.Descendants("company")
                    let id = descedant.Descendants("id").FirstOrDefault()
                    let name = descedant.Descendants("name").FirstOrDefault()
                    let url = descedant.Descendants("website-url").FirstOrDefault()
                    select new CompanyInfo
                           {
                               Id = id != null ? id.Value : string.Empty,
                               Name = name != null ? name.Value : string.Empty,
                               Url = url != null ? url.Value : string.Empty
                           }).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new XmlParsingException("Can't parse server response, message: " + e.Message);
            }
        }
    }
}
