using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json;

namespace Website2
{
    public static class GlobalConfig
    {
        public static void CustomizeConfig(HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.JsonFormatter);

            // By default, properly camel case JSON members and add our custom serializer
            var json = new JsonMediaTypeFormatter();
            json.SerializerSettings = new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.None };
            //json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            json.SerializerSettings.Formatting = Formatting.Indented;
            config.Formatters.Add(json);


        }
    }
}
