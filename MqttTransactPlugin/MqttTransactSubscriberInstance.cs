using MqttTransactPlugin.Client;
using MqttTransactPlugin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoOS.Platform.Transact.Connector;
using VideoOS.Platform.Transact.Connector.Property;

namespace MqttTransactPlugin
{
    public class MqttTransactSubscriberInstance : ConnectorInstance
    {
        private ITransactionDataReceiver _transactionDataReceiver;
        private ClientV3 _mqttClient;
        private MqttTransactSubscriberInstanceSettings _settings;

        public override void Close()
        {
            if (_mqttClient == null) return; 

            _mqttClient.UnsubscribeAsync(_settings.Topic).Wait();
            _mqttClient.DisconnectAsync().Wait();
            _mqttClient = null;

        }

        public override void Init(ITransactionDataReceiver transactionDataReceiver, IEnumerable<ConnectorProperty> properties)
        {
            _transactionDataReceiver = transactionDataReceiver;
            UpdateProperties(properties);
        }

        public override void UpdateProperties(IEnumerable<ConnectorProperty> properties)
        {
            _settings = Utils.Parse(properties);
            StartInstance(_settings);
        }

        public override ConnectorPropertyValidationResult ValidateProperties(IEnumerable<ConnectorProperty> properties)
        {
            var settings = properties.ToList();

            var propServerUriString =
                settings.Single(s => s.Key == nameof(Resources.BrokerUri)) as ConnectorStringProperty;

            var propUser =
                settings.Single(s => s.Key == nameof(Resources.UserName)) as ConnectorStringProperty;

            var propPassword =
                settings.Single(s => s.Key == nameof(Resources.Password)) as ConnectorStringProperty;

            if (propServerUriString == null || String.IsNullOrEmpty(propServerUriString.Value))
            {
                return ConnectorPropertyValidationResult.CreateInvalidResult(
                nameof(Resources.BrokerUri),
                "Please specify the Broker URI");
            }

            try
            {
                Uri broker = new Uri(propServerUriString.Value);
            }
            catch (Exception ex)
            {
                return ConnectorPropertyValidationResult.CreateInvalidResult(
                nameof(Resources.BrokerUri),
                "Enter a valid Broker address");
            }

            var propTopic =
                settings.Single(s => s.Key == nameof(Resources.Topic)) as ConnectorStringProperty;

            if (propTopic == null || String.IsNullOrEmpty(propTopic.Value))
            {
                return ConnectorPropertyValidationResult.CreateInvalidResult(
                nameof(Resources.Topic),
                "Please specify valid Topic");
            }

            return ConnectorPropertyValidationResult.ValidResult;
        }
        private void StartInstance(MqttTransactSubscriberInstanceSettings settings)
        {
            if (_mqttClient != null)
            {
                _mqttClient.DisconnectAsync().Wait();
            }
            _mqttClient = new ClientV3();
            _mqttClient.MqttMessageReceived += (message) => _transactionDataReceiver.WriteRawData(message.PayloadSegment.Array); // Console.WriteLine(Encoding.ASCII.GetString(message.PayloadSegment.Array));
            _mqttClient.ConnectAsync(settings).Wait();
        }
    }
}
