
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;

namespace TestBlync
{
 
        public class DeserializableCommand
        {
            private readonly ISerialize _serializer;
            private readonly byte[] _messageBytes;
            private readonly DeviceCommand _command;
        private Message message;

        public string CommandName
            {
                get { return _command.Name; }
            }

        //public DeserializableCommand(Message message, ISerialize serializer)
        //{

        //    _messageBytes = message.GetBytes(); // this needs to be saved, becuase it can only be read once from the original Message
        //    _serializer = serializer;

        //    _command = serializer.DeserializeObject<DeviceCommand>(_messageBytes);
        //}


        public DeserializableCommand(byte[] message, ISerialize serializer)
        {

            _messageBytes = message; // this needs to be saved, becuase it can only be read once from the original Message
            _serializer = serializer;

            _command = serializer.DeserializeObject<DeviceCommand>(_messageBytes);
        }

        public DeserializableCommand(Message message, ISerialize _serializer)
        {
            this.message = message;
            this._serializer = _serializer;
        }


        /// <summary>
        /// Deserializes the internal byte array into the type specified by T
        /// </summary>
        /// <typeparam name="T">Type to deserialize the byte array into</typeparam>
        /// <returns>Returns a concerte type T that is deserialized from the byte array</returns>
        /// 





        public T Deserialize<T>() where T : class
            {
                return _serializer.DeserializeObject<T>(_messageBytes);
            }
        }
    }

