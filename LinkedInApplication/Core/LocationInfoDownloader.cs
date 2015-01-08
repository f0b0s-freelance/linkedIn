using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LinkedInApplication.Core
{
  public class LocationInfoDownloader
  {
    private const string Key =
      "AQVfV_0s3MadzEi57qg6Qx7iftX5t-LOEVlzYssooo7-VMqsLpDOHlnQ9vLQaXkvOT_I5Y-ZPsLCOABnsdDggF9FEQ8hQD8dWvNm0Wfl9zdPw1Y-nRozmjjR5yelkmOsNuPzpI5K_SqTOKqLVWVAhoQnrSslsaClONKV8k-svtRRq-JdTCI";

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
