using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using NUnit.Framework;
using LinkedInApplication.Core;

namespace LinkedInApplicationTests
{
    [TestFixture]
    public class Class1
    {
        private const string key =
    "AQVfV_0s3MadzEi57qg6Qx7iftX5t-LOEVlzYssooo7-VMqsLpDOHlnQ9vLQaXkvOT_I5Y-ZPsLCOABnsdDggF9FEQ8hQD8dWvNm0Wfl9zdPw1Y-nRozmjjR5yelkmOsNuPzpI5K_SqTOKqLVWVAhoQnrSslsaClONKV8k-svtRRq-JdTCI";

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
    }
}
