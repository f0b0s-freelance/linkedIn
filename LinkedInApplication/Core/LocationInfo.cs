using System.Runtime.Serialization;

namespace LinkedInApplication.Core
{
    [DataContract]
    public class Locations
    {
        [DataMember(Name = "resultList")]
        public LocationInfo[] LocationsInfos { get; set; }
    }

    [DataContract]
    public class LocationInfo
    {
        [DataMember(Name = "displayName")]
        public string Name { get; set; }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
