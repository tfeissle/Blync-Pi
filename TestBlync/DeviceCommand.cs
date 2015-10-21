using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json.Converters;

namespace TestBlync
{
    public class DeviceCommand
    {
        public const string RESULT_PENDING = "Pending";
        public const string RESULT_SENT = "Sent";
        public const string RESULT_RECEIVED = "Received";
        public const string RESULT_SUCCESS = "Success";
        public const string RESULT_ERROR = "Error";

        /// <summary>
        /// The command to send to the device
        /// </summary>
        public string Name { get; set; }
        public string CorrelationId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string Result { get; set; }
        public string ErrorMessage { get; set; }

        private dynamic _parameters;
        [JsonConverter(typeof(ExpandoObjectConverter))]
        public dynamic Parameters
        {
            get
            {
                return _parameters;
            }

            set { _parameters = value; }

        }

        public DeviceCommand()
        {

        }

        public DeviceCommand(string command, dynamic parameters)
        {
            CorrelationId = Guid.NewGuid().ToString();
            Parameters = parameters;
            Name = command;
            CreatedTime = DateTime.UtcNow;
        }

        [JsonIgnore]
        public string ParametersAsJsonString
        {
            get { return JsonConvert.SerializeObject(Parameters); }
        }
    }
}
