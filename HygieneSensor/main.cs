using System;
using System.Threading;
using System.Threading.Tasks;
using Phidget22;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using OpenCare.EVODAY;
using Phidget_Example;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Text;

namespace ConsoleApplication
{
	class Program
	{
		public static MqttClient MQTTClient { get; set; }
		private static string ClientID;
		private static string CLIENT_IDENTIFIER;
        private static DateTime? lastHandWashEventSent = null;
        private static DateTime? lastSoapDispensingEventSent;
		private static bool isWashingHands = false;
		private static int toiletNotOccupiedCounter = 0;
		private static DateTime? toiletActionStarted = null;
		private static DateTime? lastToiletOccupancyReportedTime = null;

		public byte MQTTState { get; private set; }
		private static void DistanceSensor0_DistanceChange(object sender, Phidget22.Events.DistanceSensorDistanceChangeEventArgs e)
		{
			Console.WriteLine("Distance: " + e.Distance);
		}

		private static void DistanceSensor0_Attach(object sender, Phidget22.Events.AttachEventArgs e)
		{
			Console.WriteLine("Attach!");
		}

		private static void DistanceSensor0_Detach(object sender, Phidget22.Events.DetachEventArgs e)
		{
			Console.WriteLine("Detach!");
		}

		private static void DistanceSensor0_Error(object sender, Phidget22.Events.ErrorEventArgs e)
		{
			Console.WriteLine("Code: " + e.Code);
			Console.WriteLine("Description: " + e.Description);
			Console.WriteLine("----------");
		}

		static void Main(string[] args)
		{
			Log.Enable(LogLevel.Info, "phidgetlog.log");
			DistanceSensor distanceSensor0 = new DistanceSensor();
			distanceSensor0.HubPort = 3;
			//distanceSensor0.DistanceChange += DistanceSensor0_DistanceChange;
			distanceSensor0.Attach += DistanceSensor0_Attach;
			distanceSensor0.Detach += DistanceSensor0_Detach;
			distanceSensor0.Error += DistanceSensor0_Error;

			DistanceSensor distanceSensor1 = new DistanceSensor();
			distanceSensor1.HubPort = 4;
			//distanceSensor0.DistanceChange += DistanceSensor0_DistanceChange;
			distanceSensor1.Attach += DistanceSensor0_Attach;
			distanceSensor1.Detach += DistanceSensor0_Detach;
			distanceSensor1.Error += DistanceSensor0_Error;

			//VoltageRatioSensorType voltageRatioSensor = new VoltageRatioSensorType();
			VoltageRatioInput voltageRatioInput = new VoltageRatioInput();
			voltageRatioInput.HubPort = 0;
			voltageRatioInput.IsHubPortDevice = true;
			voltageRatioInput.Channel = 0;

			voltageRatioInput.VoltageRatioChange += VoltageRatioInput_VoltageRatioChange;

			try
			{
				InitizlizeMqttClient();

				distanceSensor0.Open(5000);

				distanceSensor0.DataInterval = 2000;
				distanceSensor0.SonarQuietMode = false;

				distanceSensor1.Open(5000);
				distanceSensor1.DataInterval = 2000;
				distanceSensor1.SonarQuietMode = false;

				voltageRatioInput.Open(5000);
				//voltageRatioInput.SensorType = VoltageRatioSensorType.PN_1129;

				voltageRatioInput.DataInterval = 100;

				//Console.ReadLine();

				Task.Run(() =>
				{
					
					while (true)
					{
						Thread.Sleep(2000);

						checkHandWash(distanceSensor1);
						checkToiletVisit(distanceSensor0);

					}
				});

				//Wait until Enter has been pressed before exiting
				Console.ReadLine();

				distanceSensor0.Close();
			}
			catch (PhidgetException ex)
			{
				Console.WriteLine(ex.ToString());
				Console.WriteLine("");
				Console.WriteLine("PhidgetException " + ex.ErrorCode + " (" + ex.Description + "): " + ex.Detail);
				Console.ReadLine();
			}
		}

		private static void VoltageRatioInput_VoltageRatioChange(object sender, Phidget22.Events.VoltageRatioInputVoltageRatioChangeEventArgs e)
		{
			if (lastSoapDispensingEventSent == null || (DateTime.Now - (DateTime)lastSoapDispensingEventSent).TotalSeconds > 30)

			if (e.VoltageRatio > 0.9)
			{
				Console.WriteLine("Soap dispensed");
				lastSoapDispensingEventSent = DateTime.Now;
				clearHandWash();
				SendSoapDispensnig();
				Console.WriteLine("Soap event sent");
			}
		}

		private static void clearSoap()
        {
			lastSoapDispensingEventSent = null;

		}


		/// <summary>
		/// Prepare to listen for events via MMQT
		/// </summary>
		private static void InitizlizeMqttClient()
		{
			try
			{

				if (MQTTClient != null)
					CleanUpMQTT();

				if (MQTTClient == null)
				{

					MQTTClient = new MqttClient(Setting.Default.MQTTServer, Setting.Default.MQTTPort, true, null, null, MqttSslProtocols.TLSv1_2, CallBack);
					//c = new MqttClient , Int32.Parse(Port.Text), false, MqttSslProtocols.None, null, null);

					// register to message received
					MQTTClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
					MQTTClient.MqttMsgSubscribed += Client_MqttMsgSubscribed;
					MQTTClient.MqttMsgUnsubscribed += Client_MqttMsgUnsubscribed;
					MQTTClient.MqttMsgPublished += Client_MqttMsgPublished;
					MQTTClient.ConnectionClosed += MQTTClient_ConnectionClosed;

					// subscribe to the topics PIRSensor and BEDSensor .... and then Stefans own Test message
					MQTTClient.Subscribe(new string[] { "home0", "home0", "home0" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

					ClientID = Guid.NewGuid().ToString();
					CLIENT_IDENTIFIER = ClientID;// + Guid.NewGuid(); - consider giving it something else then a GUID - but then again - need to persist it per client.

					DoMqttConnect();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

		}
		private static void checkHandWash(DistanceSensor distanceSensorEpic)
        {
			
			var handwashDistance = distanceSensorEpic.Distance;

			if (handwashDistance < Setting.Default.HandwashDistance)
			{
				if (!isWashingHands && (lastHandWashEventSent == null || (DateTime.Now - (DateTime)lastHandWashEventSent).TotalSeconds > 10))
				{
					Console.WriteLine("Handwashing being performed");
					lastHandWashEventSent = DateTime.Now;
					isWashingHands = true;
				}

			}
			else if (isWashingHands)
			{
				var timePassed = (DateTime.Now - (DateTime)lastHandWashEventSent).TotalSeconds;
				isWashingHands = false;
				Console.WriteLine("Handwash stopped! Lasted "+ (int)(timePassed) +" seconds");
				if (timePassed > 5) { 
					SendHandwashingEvent();
					Console.WriteLine("Hand wash event sent");
				}
			}

		}

		private static void clearHandWash()
        {
			isWashingHands = false;
			lastHandWashEventSent = null;
        }

		private static void checkToiletVisit(DistanceSensor distanceSensor0)
		{

			if (lastToiletOccupancyReportedTime != null)
			{
				var result = (DateTime.Now - lastToiletOccupancyReportedTime).Value.TotalSeconds;
				//We will not accept more than one toilet visit pr five minutes - should be half an hour - - if this is the case - we should log the incident and continue
				if (result < 60)
					return;
			}

			var distance = distanceSensor0.Distance;
			//Console.WriteLine("Distance: "+distance);
			double? duration = null;

			if (distance < Setting.Default.ToiletDistance)
			{
				if (toiletActionStarted == null)
				{
					toiletNotOccupiedCounter = 0;
					//If a new occupancy has started - and this is the first time - sound the alarm
					Console.WriteLine("Toilet visit started");
					toiletActionStarted = DateTime.Now.AddSeconds(-2);
					System.Media.SystemSounds.Hand.Play();
				}

				// we are on the toilet (maybe just sat down)
				duration = (int)(DateTime.Now- toiletActionStarted).Value.TotalSeconds-2;

				if (duration > 10)
				{
					Console.WriteLine("Toilet visit more than {0} seconds in total", duration);

				}

            }
            else
            {
				if (toiletActionStarted == null)
				{ return; }

					toiletNotOccupiedCounter = toiletNotOccupiedCounter + 1;
					Console.WriteLine("Left toilet for the {0}. time", toiletNotOccupiedCounter);

				if (toiletNotOccupiedCounter == 2)
				{
					var toiletActionEnded = DateTime.Now.AddSeconds(-6);
					Console.WriteLine("");
					Console.WriteLine("Toilet visit lasted from {0} to {1} ", ((DateTime)toiletActionStarted).ToLongTimeString(), ((DateTime)toiletActionEnded).ToLongTimeString());
					//now send the toilet occupancy event 
					if((toiletActionEnded-toiletActionStarted).Value.TotalSeconds > 10)
                    {
						Console.WriteLine("Toilet event sent!");
						SendToiletOccupancyEvent((DateTime)toiletActionStarted, (DateTime)toiletActionEnded, (int)((DateTime)toiletActionEnded - (DateTime)toiletActionStarted).TotalSeconds);
					}
					toiletNotOccupiedCounter = 0;
					toiletActionStarted = null;
					lastToiletOccupancyReportedTime = DateTime.Now;
				}
				
			}
		}

		private static void DoMqttConnect()
		{
			try
			{
				if (MQTTClient != null && !MQTTClient.IsConnected)
				{
					Console.WriteLine("Connecting to MQTT server");
					var state = MQTTClient.Connect(CLIENT_IDENTIFIER, Setting.Default.MQTTUser, Setting.Default.MQTTPass, true, Setting.Default.KeepAlive);
					Console.WriteLine("Connect called ");
				}
				else
				{
					Console.WriteLine("Tried connecting to MQTT server - but was already connected");
				}
			}
			catch (Exception e)
			{
			}

		}

		//Cleans up after MQTT connections. First, it disconnects if it is still connected. Next it unsubsribes all events
		private static void CleanUpMQTT()
		{
			try
			{
				// deregister event listeners
				if (MQTTClient != null)
				{

					if (MQTTClient.IsConnected) MQTTClient.Disconnect();

					MQTTClient.MqttMsgPublishReceived -= Client_MqttMsgPublishReceived;
					MQTTClient.MqttMsgSubscribed -= Client_MqttMsgSubscribed;
					MQTTClient.MqttMsgUnsubscribed -= Client_MqttMsgUnsubscribed;
					MQTTClient.MqttMsgPublished -= Client_MqttMsgPublished;
					MQTTClient.ConnectionClosed -= MQTTClient_ConnectionClosed;
					MQTTClient = null;
				}
			}
			catch (Exception e)
			{
				//Log.Logger.Error("Error during clean up: " + e);
				Console.WriteLine("Error during clean up: " + e);
			}
		}

		private static void MQTTClient_ConnectionClosed(object sender, EventArgs e)
		{
			try
			{
			}
			catch (Exception ez)
			{
				Console.WriteLine("Error while initializing: " + ez);
			}

		}

		/// <summary><
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void Client_MqttMsgSubscribed(object sender, MqttMsgSubscribedEventArgs e)
		{
			//Console.WriteLine("Test");
		}


		private static void Client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
		{
			//Console.WriteLine("Test");
		}

		private static void Client_MqttMsgUnsubscribed(object sender, MqttMsgUnsubscribedEventArgs e)
		{
			//Console.WriteLine("Test");
		}

		/// <summary>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
		{
			try
			{
			}
			catch (Exception ex)
			{
				//TODO: Add logging in case of errors - and also implement a back-off and wait and retry - in case 
				//the ID is already used by the database.
				Console.WriteLine("Error in service: " + ex);
			}
		}

		private static bool CallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}


		private static void SendToiletOccupancyEvent(DateTime start, DateTime end, int length)
		{

			var sensorData = new OpenCare.EVODAY.EDL.ToiletOccupancy();
			sensorData.SensorId = "PressalitToilet1_" + Setting.Default.PatientID;
			sensorData.Value = 1;
			sensorData.PatientId = Setting.Default.PatientID;
			sensorData.MonitorId = Setting.Default.PatientID;
			sensorData.Length = length;
			sensorData.DeviceModel = "ToiletDetect";
			sensorData.DeviceVendor = "Pressalit";
			sensorData.Room = "Bathroom";
			sensorData.Timestamp = Utils.ConvertToUnixTime(DateTime.UtcNow);
			sensorData.StartTime = Utils.ConvertToUnixTime(start.ToUniversalTime());
			sensorData.EndTime = Utils.ConvertToUnixTime(end.ToUniversalTime());

			var message = OpenCare.EVODAY.Serialize.ToJson((BasicEvent)sensorData);

			//Reset these
			clearSoap();
			clearHandWash();

			SendMQTT(message);
		}

        private static void SendMQTT(string message)
        {
			System.Media.SystemSounds.Asterisk.Play();
			Task.Run(() =>
			{

				//Send the message with exactly once semantics
				MQTTClient.Publish(Setting.Default.Topic, Encoding.UTF8.GetBytes(message), // message body
								MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, // QoS level
								true);
			});

		}

		private static void SendSoapDispensnig()
		{

			var sensorData = new OpenCare.EVODAY.EDL.SoapDispensnig();
			sensorData.SensorId = "PressalitHandWash1" + Setting.Default.PatientID;
			sensorData.PatientId = Setting.Default.PatientID;
			sensorData.MonitorId = Setting.Default.PatientID;
			sensorData.Value = 1;
			sensorData.Description = "Pressalit hand wash tracker";
			sensorData.DeviceModel = "SoapDetect";
			sensorData.DeviceVendor = "Pressalit";
			sensorData.Room = "Bathroom";
			sensorData.Timestamp = Utils.ConvertToUnixTime(DateTime.UtcNow);
			sensorData.StartTime = Utils.ConvertToUnixTime(DateTime.UtcNow);
			sensorData.EndTime = Utils.ConvertToUnixTime(DateTime.UtcNow);

			var message = OpenCare.EVODAY.Serialize.ToJson((BasicEvent)sensorData);

			SendMQTT(message);
		}


		private static void SendHandwashingEvent()
		{
			var sensorData = new OpenCare.EVODAY.EDL.HandWashingEvent();
			sensorData.SensorId = "PressalitHandWash1" + Setting.Default.PatientID;
			sensorData.Value = 1;
			sensorData.PatientId = Setting.Default.PatientID;
			sensorData.MonitorId = Setting.Default.PatientID;
			sensorData.Description = "Pressalit hand wash tracker";
			sensorData.DeviceModel = "SoapDetect";
			sensorData.DeviceVendor = "Pressalit";
			sensorData.Room = "Bathroom";
			sensorData.Timestamp = Utils.ConvertToUnixTime(DateTime.UtcNow);
			sensorData.StartTime = Utils.ConvertToUnixTime(DateTime.UtcNow);
			sensorData.EndTime = Utils.ConvertToUnixTime(DateTime.UtcNow);

			var message = OpenCare.EVODAY.Serialize.ToJson((BasicEvent)sensorData);

			SendMQTT(message);
		}

	}

}
