using System;
using System.Collections.Generic;
using VideoOS.Platform.Transact.Connector;
using VideoOS.Platform.Transact.Connector.Property;

namespace MqttTransactPlugin
{
    public class MqttTransactSubscriberDefinition : ConnectorDefinition
    {
        public override Guid Id { get; } = new Guid(Resources.PluginId);

        public override string Name { get; } = Resources.ProductName;

        public override string DisplayName => Name;

        private static readonly Version ConnectorVersion = new Version(1, 0);
        public override string VersionText => ConnectorVersion.ToString();

        public override string Manufacturer { get; } = Resources.Manufacturer;

        public override void Init()
        {
            Util.Log(
                false, 
                $"{GetType().FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}", 
                $"Initialized {Name} connector definition");
        }

        public override ConnectorInstance CreateConnectorInstance()
        {
            Util.Log(
                false, 
                $"{GetType().FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}", 
                $"Creating a new {Name} connector instance");
            return new MqttTransactSubscriberInstance();
        }

        public override void Close()
        {
            Util.Log(
                false, 
                $"{GetType().FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}", 
                $"Closing {Name} connector definition");
        }

        public override IEnumerable<ConnectorPropertyDefinition> GetPropertyDefinitions()
        {
            return new List<ConnectorPropertyDefinition>
            {
                new ConnectorStringPropertyDefinition(nameof(Resources.BrokerUri), Resources.BrokerUri, "http://10.10.10.20:11883", Resources.BrokerUriToolTip),
                new ConnectorStringPropertyDefinition(nameof(Resources.UserName), Resources.UserName, string.Empty, Resources.UserNameTooltip),
                new ConnectorPasswordPropertyDefinition(nameof(Resources.Password), Resources.Password,  string.Empty, Resources.PasswordTooltip),
                new ConnectorStringPropertyDefinition(nameof(Resources.Topic), Resources.Topic, "#", Resources.TopicToolTip),
            };
        }
    }
}
