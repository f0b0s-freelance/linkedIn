using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace LinkedInApplication.Core
{
  public static class LocationInfoParser
  {
    public static IEnumerable<LocationInfo> Parse(string content)
    {
      try
      {
        var byteArray = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(byteArray);
        var serializer = new DataContractJsonSerializer(typeof (Locations));
        var locationInfos = (Locations) serializer.ReadObject(stream);
        return locationInfos.LocationsInfos;
      }
      catch (Exception e)
      {
        throw new XmlParsingException("Can't parse server response, message: " + e.Message);
      }
    }
  }
}
