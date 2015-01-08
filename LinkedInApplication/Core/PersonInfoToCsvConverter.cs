using System;
using System.Collections.Generic;
using System.Text;

namespace LinkedInApplication.Core
{
    public static class PersonInfoToCsvConverter
    {
      public static string Convert(IEnumerable<PersonInfo> personInfos)
      {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("Company;First Name;Last Name;Headline;Url;Location;Email" + Environment.NewLine);

        foreach (var personInfo in personInfos)
        {
          stringBuilder.AppendFormat("{0};{1};{2};{3};{4};{5};{6};{7}", personInfo.CompanyName, personInfo.FirstName,
            personInfo.LastName,
            personInfo.Headline, personInfo.CompanyWebSite, personInfo.CompanyLocation, personInfo.Email,
            Environment.NewLine);
        }

        return stringBuilder.ToString();
      }
    }
}
