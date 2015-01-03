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
            stringBuilder.AppendFormat("First Name;Last Name;Headline;Company" + Environment.NewLine);

            foreach (var personInfo in personInfos)
            {
                stringBuilder.AppendFormat("{0};{1};{2};{3}{4}", personInfo.FirstName, personInfo.LastName,
                    personInfo.Headline, personInfo.Company, Environment.NewLine);
            }

            return stringBuilder.ToString();
        }
    }
}
