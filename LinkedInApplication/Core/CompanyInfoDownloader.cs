using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LinkedInApplication.Core
{
    public static class CompanyInfoDownloader
    {
        public static async Task<IEnumerable<PersonInfo>> Download(IEnumerable<PersonInfo> personsInfo, string key)
        {
            foreach (var personInfo in personsInfo)
            {
                if (string.IsNullOrEmpty(personInfo.CompanyId))
                {
                    continue;
                }

                var uri = new Uri(string.Format("https://api.linkedin.com/v1/companies/{0}:(id,name,website-url)", personInfo.CompanyId));
                var content = await DownloadContent(uri, key);
                var companyInfo = CompanyInfoParser.ParseCompanyInfo(content);
                
                if (companyInfo == null) 
                    continue;

                personInfo.CompanyName = companyInfo.Name;
                personInfo.CompanyWebSite = companyInfo.Url;
            }

            return personsInfo;
        }

        private static async Task<string> DownloadContent(Uri url, string key)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
                var result = await httpClient.GetAsync(url);
                var content = await result.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}
