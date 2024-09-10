# MqttTransactPlugin

MqttTransactPlugin is an extension (aka Plugin) for the Milestone Systems's XProtect VMS Open Platform, that targets specifically the Transact Add-On. For more information on it, please refer to the <a href="https://www.milestonesys.com/products/expand-your-solution/milestone-extensions/transact/" target="_blank">Milestone Systems' product information</a>

MqttTransactPlugin adds a MQTT subscriber source to XProtect's Transact, thus allowing Transact's users to receive data from any devices publishing information via MQTT.

## Features
* Autoreconnection in case the broker gets disconnected
* The MQTT client is implemented using <a href="https://github.com/dotnet/MQTTnet/tree/master Managed Client/" target="_blank">MQTTnet</a>
* Compatible with MQTT spec v.3.1.1
* QoS 0

## Possible Use Cases where Video evidence can be useful
* License Plate Recognition
* ATM Transactions
* Parcel tracking on a conveyor belt
* Parcel search in a warehouse, post office etc.
* Money transactions in a shop/store
* Product check at the end of Assembly Lines
* ...

## Special Thanks
To Cascadia Technology for the inspiration given by their  <a href="https://github.com/cascadia-technology/TxnServerPlugin/" target="_blank">TxnServerPlugin</a>
