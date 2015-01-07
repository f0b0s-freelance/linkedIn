using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInApplication.Core
{
    public static class CompanyInfoDownloader
    {
        private const string Key =
    "AQVfV_0s3MadzEi57qg6Qx7iftX5t-LOEVlzYssooo7-VMqsLpDOHlnQ9vLQaXkvOT_I5Y-ZPsLCOABnsdDggF9FEQ8hQD8dWvNm0Wfl9zdPw1Y-nRozmjjR5yelkmOsNuPzpI5K_SqTOKqLVWVAhoQnrSslsaClONKV8k-svtRRq-JdTCI";

      public static async Task<IEnumerable<PersonInfo>> Download(IEnumerable<PersonInfo> personsInfo)
      {
        foreach (var personInfo in personsInfo)
        {
          if (string.IsNullOrEmpty(personInfo.CompanyId))
          {
            continue;
          }

          var uri = new Uri(string.Format("https://api.linkedin.com/v1/companies/{0}:(id,name,website-url)", personInfo.CompanyId));
          var content = await DownloadContent(uri);
          var companyInfo = CompanyInfoParser.ParseCompanyInfo(content);
          if (companyInfo != null)
          {
            personInfo.CompanyName = companyInfo.Name;
            personInfo.CompanyWebSite = companyInfo.Url;
          }
        }

        return personsInfo;
      }

      private static async Task<string> DownloadContent(Uri url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Key);
                var result = await httpClient.GetAsync(url);
                var content = await result.Content.ReadAsStringAsync();
                return content;
            }
        }
    }
}
