using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VideoOS.Platform.Transact.Connector.Property;

namespace MqttTransactPlugin.Helpers
{
    public static class Utils
    {
        //private static readonly string pattern = "^(?:(?'scheme'[a-z0-9]+?):\\/\\/)?(?'host'[a-z0-9_\\-.]+)(?:\\:(?'port'[0-9]+))?(?'path'\\/[^\\?\\s]*)?(?:\\?(?'query'\\S+))?";

        //public static UriParts ExtractUriParts(string uri)
        //{
        //    var matches = Regex.Matches(uri, pattern, RegexOptions.IgnoreCase);

        //    Int32.TryParse(matches[0].Groups["port"].Value, out int port);

        //    return new UriParts()
        //    {
        //        Schema = matches[0].Groups["scheme"].Value,
        //        Host = matches[0].Groups["host"].Value,
        //        Port = port != 0 ? port : 1883,
        //        Path = matches[0].Groups["path"].Value,
        //        Query = matches[0].Groups["query"].Value
        //    };
        //}

        //public static bool ValidateMqttTopic(string topic)
        //{
        //    return true;
        //    var regex = new Regex("^(?:(?:[a-z0-9_-]+|[a-z0-9_-]+\\*|\\*)\\/)*([a-z0-9_-]+|[a-z0-9_-]+\\*|\\*|>)$", RegexOptions.IgnoreCase);
        //    bool x = regex.IsMatch(topic);
        //    return x;
        //}


        public static MqttTransactSubscriberInstanceSettings Parse(IEnumerable<ConnectorProperty> properties)
        {
            var instanceProperties = new MqttTransactSubscriberInstanceSettings();
            foreach (var property in properties)
            {
                switch (property.Key)
                {
                    case nameof(Resources.BrokerUri):
                        {
                            instanceProperties.BrokerUri = ((ConnectorStringProperty)property).Value;
                            break;
                        }

                    case nameof(Resources.UserName):
                        {
                            instanceProperties.UserName = ((ConnectorStringProperty)property).Value;
                            break;
                        }

                    case nameof(Resources.Password):
                        {
                            instanceProperties.Password =
                                string.IsNullOrEmpty(((ConnectorPasswordProperty)property).Value) ?
                                null :
                                new NetworkCredential("", ((ConnectorPasswordProperty)property)?.Value).SecurePassword;
                            break;
                        }

                    case nameof(Resources.Topic):
                        {
                            instanceProperties.Topic = ((ConnectorStringProperty)property).Value;
                            break;
                        }
                }
            }
            return instanceProperties;
        }
        public static void Debug(bool error, string where, string text)
        {
            System.Diagnostics.Debug.WriteLine($"{(error ? "ERROR" : "INFO")}: {where}, {text}");
        }

    }
}