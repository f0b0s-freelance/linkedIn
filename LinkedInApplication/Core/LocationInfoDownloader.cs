using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace LinkedInApplication.Core
{
  public class LocationInfoDownloader
  {
    public static async Task<IEnumerable<LocationInfo>> Download(string query)
    {
      var uri = new Uri(string.Format("https://www.linkedin.com/ta/region?query={0}", query));
      var content = await DownloadContent(uri);
      var locationInfo = LocationInfoParser.Parse(content);

      return locationInfo;
    }

    private static async Task<string> DownloadContent(Uri url)
    {
      using (var httpClient = new HttpClient())
      {
        var result = await httpClient.GetAsync(url);
        var content = await result.Content.ReadAsStringAsync();
        return content;
      }
    }
  }
}
