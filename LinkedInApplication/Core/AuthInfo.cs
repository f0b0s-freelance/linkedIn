using System.Runtime.Serialization;

namespace LinkedInApplication.Core
{
    [DataContract]
    public class AuthInfo
    {
        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }
    }
}