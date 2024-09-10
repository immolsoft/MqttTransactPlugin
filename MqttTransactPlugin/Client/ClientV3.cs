using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet;
using System;
using System.Threading.Tasks;
using System.Net;
using System.Reflection;
using VideoOS.Platform.Transact.Connector;
using MQTTnet.Server;
using MqttTransactPlugin.Helpers;

namespace MqttTransactPlugin.Client
{
    public delegate void MqttMessageReceived(MqttApplicationMessage message);

    public class ClientV3
    {
        private MqttFactory _mqttFactory;
        private IManagedMqttClient _managedMqttClient;
        private MqttTransactSubscriberInstanceSettings _settings;

        public event MqttMessageReceived MqttMessageReceived;

        public ClientV3()
        {
            _mqttFactory = new MqttFactory();
            _managedMqttClient = _mqttFactory.CreateManagedMqttClient();
        }

        public async Task ConnectAsync(MqttTransactSubscriberInstanceSettings settings)
        {
#if DEBUG
            Utils.Debug(false, $"{GetType().Name}:{MethodBase.GetCurrentMethod().Name}", $"Connecting to: {settings.BrokerUri}.");
#endif
            Uri broker;
            try
            {
                broker = new Uri(settings.BrokerUri);
            }
            catch (Exception ex)
            {
                Util.Log(true, MethodBase.GetCurrentMethod().Name, $": {ex.Message}");
                return;
            }

            _settings = settings;

            string password = new NetworkCredential("", settings.Password).Password;

            ManagedMqttClientOptions managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                .WithClientOptions(new MqttClientOptionsBuilder()
                                .WithCredentials(settings.UserName, password)
                                .WithTcpServer(broker.Host, broker.Port)
                                .WithClientId(Guid.NewGuid().ToString())
                                .WithCleanSession()
                                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V311)
                                .Build())
                .Build();

            try
            {
                _managedMqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
                _managedMqttClient.ConnectedAsync += _managedMqttClient_ConnectedAsync;
                _managedMqttClient.DisconnectedAsync += _managedMqttClient_DisconnectedAsync;
                await _managedMqttClient.StartAsync(managedMqttClientOptions); 
            }
            catch (Exception ex)
            {
                Util.Log(false, $"{GetType().Name}:{MethodBase.GetCurrentMethod().Name}", $" - Cannot connect to {settings.BrokerUri} : {ex.Message}");
            }
        }

        private Task _managedMqttClient_DisconnectedAsync(MqttClientDisconnectedEventArgs arg)
        {
#if DEBUG
            Utils.Debug(false, MethodBase.GetCurrentMethod().Name, $"Result Code: {arg.ConnectResult?.ResultCode}, Reason: {arg.ReasonString}, Client was connected: {arg.ClientWasConnected}");
#endif 
            return Task.CompletedTask;
        }

        private Task _managedMqttClient_ConnectedAsync(MqttClientConnectedEventArgs arg)
        {
#if DEBUG
            Utils.Debug(false, MethodBase.GetCurrentMethod().Name, $"Result Code: {arg.ConnectResult.ResultCode}");
#endif   
            Task.Run(() => SubscribeAsync(_settings.Topic));
            return Task.CompletedTask;
        }

        public async Task DisconnectAsync()
        {
            await _managedMqttClient.StopAsync();
            _managedMqttClient.Dispose();
            _managedMqttClient = null;

            MqttMessageReceived = null;
        }

        private Task OnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
        {
            MqttMessageReceived?.Invoke(args.ApplicationMessage);

            return Task.CompletedTask;
        }

        public async Task SubscribeAsync(string topic)
        {
#if DEBUG
            Utils.Debug(false, $"{GetType().Name}:{MethodBase.GetCurrentMethod().Name}", $"Subscribing to: {topic}.");
#endif
            if (_managedMqttClient == null) return; 

            if (!_managedMqttClient.IsConnected) return;

            try
            {
                await _managedMqttClient.SubscribeAsync(topic);
            }
            catch (OperationCanceledException)
            {
                Util.Log(true, MethodBase.GetCurrentMethod().Name, "Timeout while subscribing.");
            }
            catch (Exception ex)
            {
                Util.Log(true, MethodBase.GetCurrentMethod().Name, $": Exception: {ex.Message}");
            }
#if DEBUG
            Utils.Debug(false, $"{GetType().Name}:{MethodBase.GetCurrentMethod().Name}", $"Subscribed to: {topic}.");
#endif
        }
        public async Task UnsubscribeAsync(string topic)
        {
#if DEBUG
            Utils.Debug(false, $"{GetType().Name}:{MethodBase.GetCurrentMethod().Name}", $"Unsubscribing from: {topic}.");
#endif
            if (_managedMqttClient != null &&_managedMqttClient.IsConnected)
            {
                try
                {
                    await _managedMqttClient.UnsubscribeAsync(topic);
#if DEBUG
                    Utils.Debug(false, $"{GetType().Name}:{GetType().Name}", $"{MethodBase.GetCurrentMethod().Name}: Unsubscribed from: {topic}.");
#endif
                }
                catch (OperationCanceledException)
                {
                    Util.Log(true, MethodBase.GetCurrentMethod().Name, ": Timeout while unsubscribing.");
                }
                catch (Exception ex)
                {
                    Util.Log(true, MethodBase.GetCurrentMethod().Name, $": Exception: {ex}");
                }
            }
        }
    }
}
