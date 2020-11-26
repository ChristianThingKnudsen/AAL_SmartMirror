/*
Copyright(c) 2020, Stefan Wagner, Aarhus University, Aarhus, Denmark
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of Aarhus University nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED.IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace OpenCare.EVODAY
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    
    /// <summary>
    /// BasicEvent is the basic model and DTO (data transfer object) for most direct sensor events and interpreted events,
    /// as well as for technical events occuring.
    /// Basic events represents the basic generic event type used by the HEUCOD standard and the OpenCare EVODAY architecture.
    /// BasicEvent containts most of the basic relevant (but optional) fields that are supported in HEUCOD. For simplicity, 
    /// we recommend not introducing too many fields in the subclasses of BasicEvent, in order to support a simple
    /// code base in the HEUCOD standard. However, it is naturally allowed to add additional attributes to the subclasses
    /// which should prefaribly contain very vendor specific code, which one would not expect third party service providerse 
    /// to use or support.
    /// </summary>
    public partial class BasicEvent
    {

        /// <summary>
        /// The unique ID of the event. Usually a GUID or UUID but one is free to choose
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// The timestamp of the event being created in  the UNIX Epoch time format.
        /// For units WITH real time clock - the event is set on the device
        /// For units WIHTOUT a real time clock - the event is set on the receiving server
        /// This is decided by the Sensor RTC clock paramter. Defult is true.
        /// Also, please check for SendingDelay parameter, e.g. if the sensor has delayed communicaitons,
        /// it can inform the server of this
        /// </summary>
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public long? Timestamp { get; set; }

        /// <summary>
        /// Does the sensor/gateway forwarding this message have an RTC clock.
        /// In other words, can we trust the time to be true, or will the server has to fill this
        /// </summary>
        [JsonProperty("sensorRtcClock", NullValueHandling = NullValueHandling.Ignore)]
        public bool? SensorRTCClock { get; set; }

        /// <summary>
        /// IF the sensor/gateway forwarding this message had to wait before sending, e.g. due to buffering
        /// what was the delay in milliseconds. The server may use this information to subtract the dealy
        /// </summary>
        [JsonProperty("sendingDelay", NullValueHandling = NullValueHandling.Ignore)]
        public long? SendingDelay { get; set; }

        /// <summary>
        /// ID pf the user or patient to whom this event belongs
        /// </summary>
        [JsonProperty("patientId", NullValueHandling = NullValueHandling.Ignore)]
        public string PatientId { get; set; }

        /// <summary>
        /// ID of the caregiver - e.g. one helping with a rehab or care task that is reported
        /// </summary>
        [JsonProperty("caregiverId", NullValueHandling = NullValueHandling.Ignore)]
        public string CaregiverId { get; set; }

        /// <summary>
        ///  The ID that can be used to monitor events of this person. This is mainly
        ///  used in databaseless applications, but can have many shapes. In some cases
        ///  the monitor ID can be the same as the patient ID - but this is a string.
        /// </summary>
        [JsonProperty("monitorId", NullValueHandling = NullValueHandling.Ignore)]
        public string MonitorId { get; set; }

        /// <summary>
        /// Location can be an addres or apartment ID. Care should be take to avoid
        /// GDPR complications by only storing mapping to actual locations on a secure
        /// device - e.g. on the CITIZEN MONITOR in the nursing office in a nursing 
        /// 
        /// setting - where the doors are locked - and data are only available to staff members.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        /// <summary>
        /// Could be the name or identifier of the care facility or care organization 
        /// responsible for the event.
        /// </summary>
        [JsonProperty("site", NullValueHandling = NullValueHandling.Ignore)]
        public string Site { get; set; }

        /// <summary>
        /// Name of the room where the event occured (if any).
        /// </summary>
        [JsonProperty("room", NullValueHandling = NullValueHandling.Ignore)]
        public string Room { get; set; }

        /// <summary>
        /// Optinal field for street adress and number
        /// GDPR complications by only storing mapping to actual street adresses on a secure
        /// device - e.g. on the CITIZEN MONITOR in the nursing office in a nursing home 
        /// setting - where the doors are locked - and data are only available to staff members.
        /// </summary>
        [JsonProperty("streetAdress", NullValueHandling = NullValueHandling.Ignore)]
        public string StreetAdress { get; set; }

        /// <summary>
        /// Optinal field for city 
        /// GDPR complications by only storing mapping to actual locations on a secure
        /// device - e.g. on the CITIZEN MONITOR in the nursing office in a nursing home 
        /// setting - where the doors are locked - and data are only available to staff members.
        /// </summary>
        [JsonProperty("city", NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; }

        /// <summary>
        /// Optinal field for post code
        /// GDPR complications by only storing mapping to actual locations on a secure
        /// device - e.g. on the CITIZEN MONITOR in the nursing office in a nursing home 
        /// setting - where the doors are locked - and data are only available to staff members.
        /// </summary>
        [JsonProperty("postalCode", NullValueHandling = NullValueHandling.Ignore)]
        public string postalcode { get; set; }

        /// <summary>
        /// All sensors should have a unique ID which they continue to use to identify themselves.
        /// However, the sensor ID is actually not mandatory to use.
        /// </summary>
        [JsonProperty("sensorId", NullValueHandling = NullValueHandling.Ignore)]
        public string SensorId { get; set; }


        private string eventType;

        /// <summary>
        /// The type of sensor used. This should prefaribly match the name of 
        // the "class" of the device following the HEUCOD ontology 
        /// </summary>
        [JsonProperty("eventType", NullValueHandling = NullValueHandling.Ignore)]
        public string EventType
        {
            get
            {
                if (eventType == null)
                {
                    eventType = this.GetType().ToString();
                    eventTypeEnum = Utils.HashFunction(eventType);
                }
                return eventType;
            }

            set
            {
                eventType = value;
                eventTypeEnum = Utils.HashFunction(eventType);
            }
        }

        private string sensorType;

        /// <summary>
        /// The type of sensor used. This should prefaribly match the name of 
        // the "class" of the device following the HEUCOD ontology 
        /// </summary>
        [JsonProperty("sensorType", NullValueHandling = NullValueHandling.Ignore)]
        public string SensorType
        { 
            get 
            {
                if (sensorType == null)
                {
                    sensorType = this.GetType().ToString();
                    eventTypeEnum = Utils.HashFunction(sensorType);
                }
                return sensorType; 
            }
            
            set
            {
                sensorType = value;
                eventTypeEnum = Utils.HashFunction(sensorType);
            }
        }

            /// <summary>
            /// The model of the device. While the type should be a standard type if possible
            /// the model nane or number can be used to differentiate
            /// </summary>
            [JsonProperty("deviceModel", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceModel { get; set; }

        /// <summary>
        /// The vendor of the device - e.g. who built the device. While the type should be a standard type if possible
        /// the vendor nane can be used to differentiate between different types
        /// </summary>
        [JsonProperty("deviceVendor", NullValueHandling = NullValueHandling.Ignore)]
        public string DeviceVendor { get; set; }

        private long? eventTypeEnum;

        /// <summary>
        /// The type of event used. This should prefaribly match the name of 
        /// the "class" of the device following the HEUCOD ontology naming. 
        /// </summary>
        [JsonProperty("evenTypeEnum", NullValueHandling = NullValueHandling.Ignore)]
        public long? EventTypeEnum 
        {
           get;  set;       
        }

        /// <summary>
        /// The ID of a gatway who is either relaying the event from a sensor 
        /// or if the event is generated by the gateway itself. Please remember,
        /// that some sensors have their own end-to-end communicaiton capabilities,
        /// and are thus not tied to a gateway.
        /// This value should be grabbed from the hardware platform used (i.e. CPU ID, unique MAC adress, etc).
        /// No IP adress or similar. 
        /// </summary>
        [JsonProperty("gatewayId", NullValueHandling = NullValueHandling.Ignore)]
        public string GatewayId { get; set; }

        /// <summary>
        /// THe ID of the service generating the event. A service can be a sensor monitoring service, 
        /// or it could be a higher level service - interpreting data from one or more sensors
        /// and even from several sensors, and maybe historical data
        /// </summary>
        [JsonProperty("serviceId", NullValueHandling = NullValueHandling.Ignore)]
        public string ServiceId { get; set; }

        /// <summary>
        /// Is this a direct event? This mean there is no gateway involved. 
        /// The field is non mandatory
        /// </summary>
        [JsonProperty("directEvent", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DirectEvent { get; set; }

        /// <summary>
        /// The primary value (as a long). If the value is not a number, use the 
        /// description field instead. 
        /// </summary>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public double? Value { get; set; }

        /// <summary>
        /// The secondary value. If there are more than 3 values, use the 
        /// advanced field to add data.
        /// </summary>
        [JsonProperty("value2", NullValueHandling = NullValueHandling.Ignore)]
        public double? Value2 { get; set; }

        /// <summary>
        /// The tertariy value. If there are more than 3 values, use the 
        /// advanced field to add data.
        /// </summary>
        [JsonProperty("value3", NullValueHandling = NullValueHandling.Ignore)]
        public long? Value3 { get; set; }

        /// <summary>
        /// The unit of the device. The unit can be a simple unit of the SI system, 
        /// e.g. meters, seconds, grams, or it could be based 
        /// There are many representations for units of measure and in many contexts, 
        /// particular representations are fixed and required, HL7 FHIR for instance
        /// speicifies mcg for micrograms.
        /// 
        /// </summary>
        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit { get; set; }

        /// <summary>
        /// The unit of the second value from the device. The unit can be a simple unit of the SI system, 
        /// e.g. meters, seconds, grams, or it could be based 
        /// </summary>
        [JsonProperty("unit2", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit2 { get; set; }

        /// <summary>
        /// The unit of the third value from the device. The unit can be a simple unit of the SI system, 
        /// e.g. meters, seconds, grams, or it could be based 
        /// </summary>
        [JsonProperty("unit3", NullValueHandling = NullValueHandling.Ignore)]
        public string Unit3 { get; set; }

        /// <summary>
        /// The length of the event period - in milliseconds. E.g. if a PIR sensor
        /// has detected movement - and it covers 90 seconds, it would be 90 000 ms
        /// </summary>
        [JsonProperty("length", NullValueHandling = NullValueHandling.Ignore)]
        public long? Length { get; set; }
        
        /// <summary>
        /// For how long is the sensor blind. E.g. a PIR sensor will detect movement - and then send
        /// After this - it will be "blind" typically between 10 and 120 seconds. This is important for 
        /// the classification services. The value is in seconds
        /// </summary>
        [JsonProperty("sensorBlindDuration", NullValueHandling = NullValueHandling.Ignore)]
        public int? SensorBlindDuration { get; set; }

        /// <summary>
        /// Start of the observed event
        /// </summary>
        [JsonProperty("startTime", NullValueHandling = NullValueHandling.Ignore)]
        public long? StartTime { get; set; }
        /// <summary>
        /// End of the observerd event
        /// </summary>
        [JsonProperty("endTime", NullValueHandling = NullValueHandling.Ignore)]
        public long? EndTime { get; set; }

        /// <summary>
        /// The aveage power consumption of the device in watts - use together with the length
        /// to get the kWh 
        /// </summary>
        [JsonProperty("power", NullValueHandling = NullValueHandling.Ignore)]
        public int? Power { get; set; }

        /// <summary>
        /// The battery level in percentage (0-100). A battery alert service may use this information to
        /// send alerts at 10% or 20 % battery life - and critical alerts at 0%
        /// </summary>
        [JsonProperty("battery", NullValueHandling = NullValueHandling.Ignore)]
        public int? Battery { get; set; }

        /// <summary>
        /// The self-reproted accuracy of the sensor or service event.
        /// E.g. a sensor may be 99% sure that it has detected a fall
        /// While a classification service may be 88% sure
        /// </summary>
        [JsonProperty("accuracy", NullValueHandling = NullValueHandling.Ignore)]
        public int? Accuracy { get; set; }

        /// <summary>
        /// RSSI stands for Received Signal Strength Indicator. It is often used with radio based networks, 
        /// including WiFi, BLE, Zigbee, Lora
        /// When e.g. used with a BLE beacon, tt is the strength of the beacon’s signal as seen on the receiving device, e.g. a smartphone. The signal strength depends on distance and Broadcasting Power value. At maximum Broadcasting Power (+4 dBm) the RSSI ranges from -26 (a few inches) to -100 (40-50 m distance).
        /// RSSI is used to approximate distance between the device and the beacon.
        /// At maximum Broadcasting Power (+4 dBm) a BLE RSSI usually ranges from -26 (a few inches) to -100 (40-50 m distance).
        /// Often it is used along with another value defined by the iBeacon standard: Measured (transmission) Power
        /// Due to external factors influencing radio waves—such as absorption, interference, or diffraction—RSSI tends to fluctuate.
        /// The further away the device is from the beacon, the more unstable the RSSI becomes.
        /// </summary>
        [JsonProperty("rssi", NullValueHandling = NullValueHandling.Ignore)]
        public double? RSSI { get; set; }

        /// <summary>
        /// Measured Power indicates what’s the expected RSSI at a distance of 1 meter to the beacon. 
        /// Combined with RSSI, it allows to estimate the distance between the device and the beacon. 
        /// </summary>
        [JsonProperty("measuredPower", NullValueHandling = NullValueHandling.Ignore)]
        public double? MeasuredPower{ get; set; }

        /// <summary>
        ///Signal-to-noise ratio(abbreviated SNR or S/N) is a measure used in science and engineering that compares the 
        ///level of a desired signal to the level of background noise.SNR is defined as the ratio of signal power to the noise power, 
        ///often expressed in decibels.        
        /// </summary>
        [JsonProperty("signalToNoiseRatio", NullValueHandling = NullValueHandling.Ignore)]
        public double? SignalToNoiseRatio{ get; set; }

        /// <summary>
        ///  Link Quality (LQ) is the quality of the real data received in a signal. 
        ///  This is a value from 0 to 255, being 255 the best quality.
        ///  Typically expect from 0 (bad) to 50-90 (good). It is related to RSSI and SNR values
        ///  as a quality indicator.
        ///  </summary>
        [JsonProperty("linkQuality", NullValueHandling = NullValueHandling.Ignore)]
        public double? LinkQuality { get; set; }

        /// <summary>
        /// This field can contain advanced or composite values which are well-known to the specialized vendors
        /// </summary>
        [JsonProperty("advanced", NullValueHandling = NullValueHandling.Ignore)]
        public string Advanced { get; set; }

        /// <summary>
        /// This field supports adding a prose description of the event - which can e.g. by used
        /// for audit and logging purposes.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }


        #region .NET Helper Properties
        public DateTime TimeStampNet 
        {
            get { return Utils.UnixTimeToDateTime(Timestamp); }
        }

        public DateTime StartTimeNet
        {
            get { return Utils.UnixTimeToDateTime(StartTime); }
        }

        public DateTime EndTimeNet
        {
            get { return Utils.UnixTimeToDateTime(EndTime); }
        }
        public BasicEvent()
        {
            this.SensorType = this.GetType().ToString();
            this.EventType = this.GetType().ToString();

            EventTypeEnum = Utils.HashFunction(this.GetType().ToString());
        }
        #endregion

    }

    #region Patient Domain Classes
    /// <summary>
    /// The Patient class is used to hold a list of events that belongs to the patient.
    /// </summary>
    public partial class Patient
    {
        /// <summary>
        /// ID pf the user or patient. The format is not fixed in the standard
        /// but the recommendation is to use either a time tick (DateTime.Now) or 
        /// a GUID - if it is a very large user population and not use any person refarrable 
        /// </summary>
        public string PatientID { get; set; }

                /// <summary>
        /// ID pf the user or patient. The format is not fixed in the standard
        /// but the recommendation is to use either a time tick (DateTime.Now) or 
        /// a GUID - if it is a very large user population and not use any person refarrable 
        /// </summary>
        public string Name{ get; set; }

        /// <summary>
        ///  The ID that can be used to monitor events of this person. This is mainly
        ///  used in databaseless applications, but can have many shapes. In some cases
        ///  the monitor ID can be the same as the patient ID - but as a string - but remember
        ///  that MonitorID may be changed more frequently than PatientID
        /// </summary>
        public string MonitorId { get; set; }

        /// <summary>
        /// The eventlist can be used to tentatively hold a list of events belonging to an individual person
        /// </summary>
        public List<BasicEvent> EventList { get; set; }

        /// <summary>
        /// The eventlist can be used to tentatively hold a list of events belonging to an individual person
        /// </summary>
        public List<BasicEvent> RecentEventsList { get; set; }

        /// <summary>
        /// This is when the last even was received on this patient
        /// </summary>
        public DateTime? LastEventReceived { get; set; }

        /// <summary>
        /// This is last heartbeat from the users home
        /// </summary>
        public DateTime LastAmbientHeartbeat { get; set; }

        /// <summary>
        /// This is last heartbeat from the usrs mobile device
        /// </summary>
        public DateTime LastMobileHeartbeat { get; set; }

        /// <summary>
        /// Last seen mobile location - e.g. a smart watch or phone
        /// reporting its location
        /// </summary>
        public string LastSeenMobileLocation { get; set; }


        /// <summary>
        /// If it is more than 2 minutes since we last saw a user at home 
        /// then HEUCOD suggest sending a notification saying user not home.
        /// When user is back again, HEUCOD suggest sending a notifcaiton saying user home
        /// </summary>
        public DateTime LastConfirmedHomeEvent { get; set; }


        /// <summary>
        /// A list of alerts
        /// </summary>
        public List<AlertNotification> AlertNotficationss { get; set; }



        public Patient()
        {
            EventList = new List<BasicEvent>();
            RecentEventsList = new List<BasicEvent>();
        }
    }

    /// <summary>
    /// Alert notificaiton is a helper class containing all alert data nessecary and relevant
    /// </summary>
    public class AlertNotification
    {

        /// <summary>
        /// Use this ID to identify an alert within a system. Thus, if an alert is 
        /// going to be cancelled, please use the same ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 20-40 characters headline
        /// </summary>
        public string AlertHeadline { get; set; }

        /// <summary>
        /// Full description
        /// </summary>
        public string AlertDescription { get; set; }

        /// <summary>
        /// Severity from 0 (not severe) to 100 (max severity)
        /// </summary>
        public int AlertSeverity { get; set; }

        /// <summary>
        /// The date time when the alert was recieved. Please not
        /// the time the alert was received is not nessecarily the time of
        /// the accident or condition leading to the alert. Most decision algorithms
        /// are slow to reach the decision to give an alert - in order to avoid false positives.        /// 
        /// </summary>
        public DateTime AlertReceived { get; set; }

        /// <summary>
        /// The primary user can cancel an alert. 
        /// </summary>
        public DateTime AlertCancelled { get; set; }

        /// <summary>
        /// Is used to mark than an alert has been handled
        /// </summary>
        public DateTime AlertHandled { get; set; }

        //The ID of the user who handled the alert
        public string AlertHandledBy { get; set; }
    }
    #endregion
    
    
    /*  The below list of event hashcodes and names have been generated by the Utils class
     *  Before adding new event types, please we suggest to check the uniques of the hashcde 
     *  82420 ClassificationResult 
        81325 BasicEvent 
        80542 EDL 
        82043 BedOccupancyEvent 
        82604 BathroomOccupancyEvent 
        81887 ToiletOccupancy 
        82263 ChairOccupancyEvent 
        82429 ToothBrushingSession 
        82099 RoomMovementEvent 
        82274 CouchOccupancyEvent 
        81943 HandWashingEvent 
        81776 SoapDispensnig 
        82202 TelevisionUseEvent 
        82053 ApplianceUseEvent 
        82028 FallDetectedEvent 
        81848 WalkingDetected 
        81746 EnterHomeEvent 
        81729 LeaveHomeEvent 
        80538 ADL 
        81152 Sleeping 
        81061 Resting 
        81272 Toileting 
        81030 Bathing 
        81042 Hygiene 
        81551 GroomingHair 
        81163 Brushing 
        80929 Eating 
        81160 Dressing 
        81598 Transferring 
        81477 Socializing 
        81320 WatchingTV 
        81475 Exericising 
        80611 IADL 
        80463 AE 
        81514 FallDetected 
        81540 BathroomFall 
        82686 LowGeneralActivityLevel 
        82085 LowHydrationLevel 
        81877 SparseToiletUse 
        82097 FrequentToiletUse 
        82334 MissedToothbrushing 
        81963 MissedMedication 
        81655 MissedHygiene 
        81750 TechnicalEvent 
        81209 HeartBeat 
        81827 BatteryPowerLOW 
        82539 DeviceNeedsMaintenance 
        81936 DeviceUsageEvent 
        82441 AppliancenDeviceUsage 
        81000 TVUsage 
        81325 RadioUsage 
        81677 ComputerUsage 
        81450 PhonerUsage 
        81969 SmartPhonerUsage 
        82761 RehabiliationDeviceUsage 
        82136 CookingDeviceUsage 
        81423 FridgeUsage 
        82295 MedicalObservations 
        81493 Observation 
        82840 BloodPressureMeasurement 
        82103 WeightMeasurement 
        82669 SaturationtMeasurement 
        82653 TemperatureMeasurement 
        81265 Serialize 
        81281 Converter 
        81477 SensorEvent 
        81900 SummarizedEvent 
        80858 Utils 
        82161 <>c__DisplayClass2_0 
        80550 <>c 
        82162 <>c__DisplayClass3_0 
        82163 <>c__DisplayClass4_0 
  *
 */


    public partial class BasicEvent
    {
        public static BasicEvent FromJson(string json) => JsonConvert.DeserializeObject<BasicEvent>(json, EVODAY.Converter.Settings);
    }

    #region SerilizationCode

    public static class Serialize
    {
        public static string ToJson(this BasicEvent self) => JsonConvert.SerializeObject(self, EVODAY.Converter.Settings);
    }


    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    #endregion
}


namespace OpenCare.EVODAY.EDL
{
    /// <summary>
    /// EDL events - are the events of daily living - which is collection of typical events which are naturally occuring
    /// in the home setting of users. EDL's are typically very basic in nature, and are usaully tightly intervowen with 
    /// ADL's (see below) and IADL's. From a collection of EDL's - a classification algortihm can typically infer 
    /// relevant ADL and IADL activites, as well as AE - or adverse events (see below) - for example a range of 
    /// EDL's in a given timeframe - e.g. during a 10 minute window - can be used to infer whether a fall has occured
    /// or whether the person was on the toilet and would perform correct handwashing operations. If not, an adverse event can be 
    /// signalled.
    /// </summary>
    public class EDL : BasicEvent { }

    #region EDL definition

    //EDL = Events of Daily Living - these are mostly direct sesnsor generated data that will provide an insigth into 
    //events occuring with the user
    public class BedOccupancyEvent : EDL { }
    public class BathroomOccupancyEvent : EDL { }
    public class ToiletOccupancy : EDL { }
    public class ChairOccupancyEvent : EDL { }
    public class ToothBrushingSession : EDL { }
    public class RoomMovementEvent : EDL { }
    public class CouchOccupancyEvent : EDL { }
    public class HandWashingEvent : EDL { }
    public class SoapDispensnig : EDL { }
    public class TelevisionUseEvent : EDL { }
    public class ApplianceUseEvent : EDL { }
    public class FallDetectedEvent : EDL { }
    public class WalkingDetected : EDL { }
    public class EnterHomeEvent : EDL { }
    public class LeaveHomeEvent : EDL { }
    public class LighingtChangedEvent : EDL { }
    public class LighingtSettingChangedEvent : EDL { }
    public class UserIsHomeEvent : EDL { }

    #endregion
}


namespace OpenCare.EVODAY.ADL
{

    /// <summary>
    /// Activities of daily living (ADLs) are routine activities people do every day without assistance. 
    /// ADL is are thus the basic tasks of daily life that most people are used to doing without assistance.
    /// The concept of ADLs was originally proposed in the 1950s by Sidney Katz at the Benjamin Rose Hospital in Cleveland, Ohio 
    /// and has been added to and refined by a variety of researchers since that time. 
    //  Assisted living facilities, in-home care providers, and nursing homes specialize in providing care and services 
    /// to those who can not perform ADLs for themselves.
    /// There are six basic ADLs: eating (self-feeding), bathing, personal hygiene (washing hands - brushing teeth) and grooming (including brushing/combing/styling hair), getting dressed, toileting, transferring (functonal mobiltiy).
    /// The performance of these ADLs is important in determining what type of long-term care and health coverage, 
    /// such as Medicare, Medicaid in the USA, public homecare or long-term care support in parts of Euorpe,  
    /// There is a hierarchy to where the early-loss function is hygiene, while the mid-loss functions are toilet use and transferring, 
    /// and the late loss function is eating. When there is only one remaining area in which the person is independent, 
    /// there is around 63% chance that it is eating and only a 4% chance that it is hygiene. Thus, tracking a lack of hygiene can
    /// be a powerfull indicator for lack of cognitive or physical function.
    /// The ADL definition is expanded to provide Sleeping and Resting, Brushing, Socializing, WatchingTV, and Exercising
    /// </summary>
    public class ADL : BasicEvent { }

    #region ADL definition 

    public class Sleeping : ADL { }

    public class Resting : ADL { }

    public class Toileting : ADL { }

    public class Bathing : ADL { }

    public class Hygiene : ADL { }

    public class GroomingHair : ADL { }

    public class Brushing : ADL { }

    public class Eating : ADL { }

    public class Dressing : ADL { }

    public class Transferring : ADL { }

    public class Socializing : ADL { }

    public class WatchingTV : ADL { }

    public class Exericising : ADL { }


    #endregion

}

namespace OpenCare.EVODAY.Notifications
{

    /// <summary>
    /// Notifications are used to inform of things that do not constitute an alert 
    /// but warrants the attention of a patient or a caregiver

    /// </summary>
    public class Notifications : BasicEvent { }

    #region Notificationsdefinition 

    public class HandwashingFollowingToiletingComplete : Notifications { }

    public class RememberSoapAndHandwashAfterToilet : Notifications { }

    public class RememberSoapBeforeHandwashAfterToilet : Notifications { }

    public class RememberWashingHandsAsLastTask : Notifications { }

    public class HandwashingAfterToiletForgotten : Notifications { }

    public class PossibleFallDetected : Notifications { }

    public class LowActivityWarning : Notifications { }

    public class ReplaceBatteries : Notifications { }

    public class SuboptimalSleep : Notifications { }

    public class SupoptimalBrushing : Notifications { }

    public class GeneralNotification : Notifications { }

    public class MovementInHomeWhileUserNotHome : Notifications { }

#endregion

}

namespace OpenCare.EVODAY.IADL
{

    /// <summary>
    /// Instrumental activities of daily living (IADL) are not necessary for fundamental functioning, 
    /// but they let an individual live independently in a community, and can thus be important to monitor
    /// as well 
    /// </summary>
    public class IADL : BasicEvent { }

    #region IADL


    public class Shopping : IADL { }
    public class Housekeeping : IADL { }
    public class ManagingMoney : IADL { }
    public class FoodPreparation : IADL { }
    public class Telephone : IADL { }
    public class Transportation : IADL { }

    #endregion
}

namespace OpenCare.EVODAY.AE
{


    /// <summary>
    /// An adverse event is an incident that results in harm to the patient or citizen. 
    /// Adverse events commonly experienced include falls, medication errors, malnutrition, 
    /// incontinence, and hospital-acquired pressure injuries and infections. 
    /// It can also include missed personal hygiene and brushing actions. 
    /// Physical and cognitive functional decline can have a significant impact on a person’s 
    /// ability to perform activities of daily living.  Older people are particularly vulnerable 
    /// to experiencing adverse events due to inherent complexity in managing their care and a decline 
    /// in physiological reserves. 
    /// </summary>
    public class AE : BasicEvent { } //Adverse event

    #region AE definition 

    public class FallDetected : AE { }

    public class BathroomFall : AE { }

    public class LowGeneralActivityLevel : AE { }

    public class LowHydrationLevel : AE { }

    public class SparseToiletUse : AE { }

    public class FrequentToiletUse : AE { }

    public class MissedToothbrushing : AE { }

    public class MissedMedication : AE { }

    public class MissedHygiene : AE { }

    #endregion

}

namespace OpenCare.EVODAY.TechnicalEvent
{

    /// <summary>
    /// Technial events are basic evens which supports the technical monitoring and reporting
    /// of sensors, gateways and infrastructures, e.g. keeping track of conectivity, and 
    /// battery status
    /// </summary>
    public class TechnicalEvent : BasicEvent { } //Technical event

    #region TechnicalEvent definition 

    public class HeartBeat : TechnicalEvent { }

    public class BatteryPowerLOW : TechnicalEvent { }

    public class DeviceNeedsMaintenance : TechnicalEvent { }

    #endregion

}
namespace OpenCare.EVODAY.DeviceUsageEvent
{
    /// <summary>
    /// Detice usage events are used to keep track of devices that are placed at 
    /// the home of users or patients or in nursing home facilities. Rather than report 
    /// on the users individual activities, they focus on whether the device is being used.
    /// </summary>
    public class DeviceUsageEvent : BasicEvent { } //Adverse event

    #region DeviceUsage definition 

    public class AppliancenDeviceUsage : DeviceUsageEvent { }

    public class TVUsage : DeviceUsageEvent { }

    public class RadioUsage : DeviceUsageEvent { }

    public class ComputerUsage : DeviceUsageEvent { }

    public class PhonerUsage : DeviceUsageEvent { }

    public class SmartPhonerUsage : DeviceUsageEvent { }

    public class RehabiliationDeviceUsage : DeviceUsageEvent { }

    public class CookingDeviceUsage : DeviceUsageEvent { }

    public class FridgeUsage : DeviceUsageEvent { }

    #endregion
}
namespace OpenCare.EVODAY.MedicalObservations

{ 
/// <summary>
/// Medical obseravtions are modelled based on the HL7 FHIR observation - and are realted to measurements concerning vital signs for instance
/// blood pressure, weight, oximeter (saturation), ECG, and more.
/// In HL7, observations are a considered central element in healthcare, used to support diagnosis, monitor progress, determine baselines 
/// and patterns and even capture demographic characteristics. 
/// Most observations are simple name/value pair, e.g. a weight observation, but some observations group other observations 
/// together logically, or even are multi-component observations. 
/// </summary>
public class MedicalObservations : BasicEvent { } //Adverse event

#region MedicalObservations definition 

public class Observation : MedicalObservations { }

public class BloodPressureMeasurement : Observation { }

public class WeightMeasurement : Observation { }

public class SaturationtMeasurement : Observation { }

public class TemperatureMeasurement : Observation { }

#endregion
}