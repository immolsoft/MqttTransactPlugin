using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using VideoOS.Platform.Transact.Connector.Property;

namespace MqttTransactPlugin
{
    public struct MqttTransactSubscriberInstanceSettings
    {
        public string BrokerUri { get; set; }
        public string UserName { get; set; }
        public SecureString Password { get; set; }
        public string Topic { get; set; }
    }
}
