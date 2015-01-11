using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace LinkedInApplication.Core
{
    public static class PersonInfoDownloader
    {
        public static async Task<IEnumerable<PersonInfo>> Download(Uri url, string key)
        {
            var content = await DownloadContent(url, key);
            var requestResultInfo = RequestResultSizeParser.Parse(content);
            var peopleInfos = new List<PersonInfo>(requestResultInfo.Total);
            peopleInfos.AddRange(PersonInfoParser.Parse(content));

            for (var i = requestResultInfo.Count; i < requestResultInfo.Total; i += requestResultInfo.Count)
            {
                var uri = new Uri(url.AbsoluteUri + String.Format("&start={0}&count={1}", i, requestResultInfo.Count));
                content = await DownloadContent(uri, key);
                requestResultInfo = RequestResultSizeParser.Parse(content);
                peopleInfos.AddRange(PersonInfoParser.Parse(content));
            }

            return peopleInfos;
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

        public static void PostProcess(IEnumerable<PersonInfo> persons)
        {
            foreach (var person in persons)
            {
                if (string.IsNullOrEmpty(person.CompanyWebSite))
                    continue;

                try
                {
                    Uri uri;
                    if (Uri.TryCreate(person.CompanyWebSite, UriKind.RelativeOrAbsolute, out uri))
                    {
                        var host = uri.IsAbsoluteUri ? uri.Host : person.CompanyWebSite;

                        if (host.StartsWith("www."))
                        {
                            host = host.Substring(4, host.Length - 4);
                        }

                        person.Email = string.Format("{0}@{1}", person.FirstName, host);
                    }
                    else
                    {
                        person.Email = "n/a";
                    }
                }
                catch (UriFormatException)
                {

                }
            }
        }
    }
}
