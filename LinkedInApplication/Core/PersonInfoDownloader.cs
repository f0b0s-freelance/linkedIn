using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LinkedInApplication.Core
{
    public static class PersonInfoDownloader
    {
        private const string Key =
    "AQVfV_0s3MadzEi57qg6Qx7iftX5t-LOEVlzYssooo7-VMqsLpDOHlnQ9vLQaXkvOT_I5Y-ZPsLCOABnsdDggF9FEQ8hQD8dWvNm0Wfl9zdPw1Y-nRozmjjR5yelkmOsNuPzpI5K_SqTOKqLVWVAhoQnrSslsaClONKV8k-svtRRq-JdTCI";

        public static async Task<IEnumerable<PersonInfo>> Download(Uri url)
        {
            var content = await DownloadContent(url);
            var requestResultInfo = RequestResultSizeParser.Parse(content);
            var peopleInfos = new List<PersonInfo>(requestResultInfo.Total);
            peopleInfos.AddRange(PersonInfoParser.Parse(content));

            for (int i = requestResultInfo.Count; i < requestResultInfo.Total; i += requestResultInfo.Count)
            {
                content = await DownloadContent(new Uri(url.AbsoluteUri + String.Format("&start={0}&count={1}", i, requestResultInfo.Count)));
                requestResultInfo = RequestResultSizeParser.Parse(content);
                peopleInfos.AddRange(PersonInfoParser.Parse(content));
            }

            return peopleInfos;
        }

        private static async Task<string> DownloadContent(Uri url)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Key);
            var result = await httpClient.GetAsync(url);
            var content = await result.Content.ReadAsStringAsync();
            return content;
        }
    }
}
