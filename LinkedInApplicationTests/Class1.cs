using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using NUnit.Framework;
using LinkedInApplication.Core;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;

namespace LinkedInApplicationTests
{
    [TestFixture]
    public class Class1
    {
        //67fc07db-3147-4c05-8e9b-3ce336fe40a2
        //private const string key = "AQVfV_0s3MadzEi57qg6Qx7iftX5t-LOEVlzYssooo7-VMqsLpDOHlnQ9vLQaXkvOT_I5Y-ZPsLCOABnsdDggF9FEQ8hQD8dWvNm0Wfl9zdPw1Y-nRozmjjR5yelkmOsNuPzpI5K_SqTOKqLVWVAhoQnrSslsaClONKV8k-svtRRq-JdTCI";
        private const string key = "67fc07db-3147-4c05-8e9b-3ce336fe40a2";

        [Test]
        public void Test()
        {
            var url = new Uri("https://www.linkedin.com/uas/oauth2/authorization?response_type=code" +
                                           "&client_id=npq2v0qgrl9y" +
                                           "&scope=r_fullprofile%20r_emailaddress%20r_network" +
                                           "&state=DCEEFWF45453sdffef424" +
                                           "&redirect_uri=http://skilleo.herokuapp.com");
            Console.WriteLine(url);
            var httpClient = new HttpClient();
            var result = httpClient.GetAsync(url).Result;
            Console.WriteLine(result);
        }

        [Test]
        public void Test1()
        {
            var url = new Uri("https://www.linkedin.com/uas/oauth2/accessToken?grant_type=authorization_code" +
                              "&code=AQTH4piKPjgAvf7JaDLSfe-Y58QyzoM44vO4DVTrMfSYtTbzFtgulyohFwhaLY-po7m3ZIOwl9mVEXBjOH6mSQPCqU1NT2chmaODVRUmh5NiNbpohis" +
                              "&redirect_uri=http://skilleo.herokuapp.com" +
                              "&client_id=npq2v0qgrl9y" +
                              "&client_secret=50DUgVK6Cl6RwMRy");
            Console.WriteLine(url);
            var httpClient = new HttpClient();
            var result = httpClient.GetAsync(url).Result;
            Console.WriteLine(result);
        }

        [Test]
        public void Test2()
        {
            var url = new Uri("https://api.linkedin.com/v1/people-search?company-name=DeltaPay");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            var result = httpClient.GetAsync(url).Result;
            Console.WriteLine(result);
        }

        [Test]
        public void GetUser()
        {
            var url = new Uri("https://api.linkedin.com/v1/people/id=0Wck-O1laV:(first-name,last-name,positions)");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            var result = httpClient.GetAsync(url).Result;
            Console.WriteLine(result);
        }

        [Test]
        public void GetUserProfile()
        {
            var url = new Uri(String.Format("https://api.linkedin.com/v1/people/url=https://www.linkedin.com/profile/view?id=339415843&amp;authType=name&amp;authToken=5n4V&amp;trk=api*a288830*s296410*"));
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            var result = httpClient.GetAsync(url).Result;
            Console.WriteLine(result);
        }

        [Test]
        public void GetCompanyProfile()
        {
            var url = new Uri(String.Format("https://api.linkedin.com/v1/companies/2219005:(id,name,website-url,locations:(address:(state,city)))"));
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            var result = httpClient.GetAsync(url).Result;
            Console.WriteLine(result);
        }

        [Test]
        public void SearchByFacet1()
        {
            var url =
                new Uri(
                    string.Format(
                        "https://api.linkedin.com/v1/people-search:(people:(first-name,headline,positions,location:(name)))?facets={0}&facet={1}&facet={2}&title=CEO&current-title=true",
                        Uri.EscapeDataString("location,industry"),
                        Uri.EscapeDataString("location,ru:7487,ru:7481"),
                        Uri.EscapeDataString("industry,6")));
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            var result = httpClient.GetAsync(url).Result;
            var content = result.Content.ReadAsStringAsync().Result;
            
            var element = XDocument.Parse(content);
            var peoples = from descedant in element.Descendants("person")
                let firstName = descedant.Descendants("first-name").FirstOrDefault()
                let lastName = descedant.Descendants("last-name").FirstOrDefault()
                let company = descedant.Descendants("company").FirstOrDefault()
                let headline = descedant.Descendants("headline").FirstOrDefault()
                select new PersonInfo
                       {
                           FirstName = firstName != null ? firstName.Value : string.Empty,
                           LastName = lastName != null ? lastName.Value : string.Empty,
                           Headline = headline != null ? headline.Value : string.Empty,
                           CompanyName = company != null ? company.Value : string.Empty,
                       };

            foreach (var people in peoples)
            {
                Console.WriteLine(people.FirstName);
                Console.WriteLine(people.LastName);
                Console.WriteLine(people.Headline);
                Console.WriteLine(people.CompanyName);
                Console.WriteLine("=======================");
            }

//            Console.WriteLine(result);
//            Console.WriteLine(content);
        }

        [Test]
        public void SearchByFacet()
        {
            var url =
                new Uri(
                    string.Format(
                        "https://api.linkedin.com/v1/people-search?facets={0}&facet={1}&facet={2}&title=javascript&current-title=true",
                        Uri.EscapeDataString("location,industry"), 
                        Uri.EscapeDataString("location,ru:7487,ru:7481"),
                        Uri.EscapeDataString("industry,6")));
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            var result = httpClient.GetAsync(url).Result;
            Console.WriteLine(result);
        }

        [Test]
        public void GetLocations()
        {
            var url = new Uri("https://www.linkedin.com/ta/region?query=v");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            var result = httpClient.GetAsync(url).Result;

          var response = result.Content.ReadAsStringAsync().Result;
            byte[] byteArray = Encoding.UTF8.GetBytes(response);
            var stream = new MemoryStream(byteArray);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Locations));
            Locations locationInfos = (Locations)serializer.ReadObject(stream);


            Console.WriteLine(result);
        }

      [Test]
      public void TestUrl()
      {
        var uri = new Uri("http://www.rambler.ru");
        Console.WriteLine(uri.Host.TrimStart(new char[]{'w'}));
      }

        [Test]
        public void Test234()
        {
            var uri = "www.google.com";
            //var url = new Uri(uri);
            Uri u;
            if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out u))
            {
                Console.WriteLine("Yes");
                Console.WriteLine(u);
            }
            var res = Uri.CheckSchemeName(uri);
            var res1 = Uri.CheckHostName(uri);
            Console.WriteLine(res);
            Console.WriteLine(res1);
        }

        [Test]
        public void Testtre()
        {
            const string t = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>" +
"<people-search>" +
  @"<people total=""1"">" +
    "<person>" +
      @"<positions total=""2"">" +
        "<position>" +
          "<id>599222310</id>" +
          "<title>CEO</title>" +
          "<summary>We live in a connected world. To thrive and progress, we need to understand and influence the web of connections that surrounds us.</summary>" +
          "<start-date>" +
            "<year>2013</year>" +
            "<month>12</month>" +
          "</start-date>" +
          "<is-current>true</is-current>" +
          "<company>" +
            "<name>DLS</name>" +
          "</company>" +
        "</position>" +
        "<position>" +
          "<id>517993058</id>" +
          "<title>Head of Media Development</title>" +
          "<is-current>true</is-current>" +
          "<company>" +
            "<id>2715562</id>" +
            "<name>MIT Labs</name>" +
          "</company>" +
        "</position>" +
      "</positions>" +
    "</person>"  +
  "</people>" +
"</people-search>";


            var result = PersonInfoParser.Parse(t);
        }
    }
}
