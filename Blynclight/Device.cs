using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.UI;

namespace BlyncLight
{
    public class Device : IDisposable
    {      
        private Color CURRENT_COLOR = Colors.Black;
        private readonly Color DEFAULT_COLOR = Colors.Black;
        private byte toggleSwitch;
        private byte _red;     //byte1
        private byte _blue;    //byte2
        private byte _green;   //byte3
        private byte _blink;   //byte4
        private byte _music;   //byte5
        private byte _byte6;
        private byte _byte7;
        private byte _byte8;
        private readonly Tuple<DeviceInformation, HidDevice> _store;
        private Color _color;

        public Device(DeviceInformation info, HidDevice device)
        {
            _store = new Tuple<DeviceInformation, HidDevice>(info, device);
            _color = DEFAULT_COLOR;
        }

        public DeviceInformation DeviceInformation
        {
            get { return _store.Item1; }
        }

        public HidDevice HIDDevice
        {
            get { return _store.Item2; }
        }

        public Color StatusColor
        {
            get { return _color; }
            set
            {
                if (_color != value)
                {
                    _color = value;
                    SendColorCommand(_color);
                }
            }
        }
        public byte Blink
        {
            get { return _blink; }
            set
            {
                if (_blink != value)
                {
                    _blink = value;
                    SendCommand();
                }
            }
        }
        public byte Music
        {
            get { return _music; }
            set
            {
                if (_music != value)
                {
                    _music = value;
                    SendCommand();
                }
            }
        }
        public byte Byte6
        {
            get { return _byte6; }
            set
            {
                if (_byte6 != value)
                {
                    _byte6 = value;
                    SendCommand();
                }
            }
        }
        public byte Byte7
        {
            get { return _byte7; }
            set
            {
                if (_byte7 != value)
                {
                    _byte7 = value;
                    SendCommand();
                }
            }
        }
        public byte Byte8
        {
            get { return _byte8; }
            set
            {
                if (_byte8 != value)
                {
                    _byte8 = value;
                    SendCommand();
                }
            }
        }
        public byte ToggleSwitch
        {
            get { return toggleSwitch; }
            set
            {
                if (toggleSwitch != value)
                {
                    toggleSwitch = value;
                    SendColorCommand(_color);
                }
            }
        }
        private void SendColorCommand(Color color)
        {
            Byte red = color.R;
            Byte green = color.G;
            Byte blue = color.B;

            _red = red;
            _blue = blue;
            _green = green;

            var commandBuffer = new Byte[9];
            commandBuffer[0] = 0x00;
            commandBuffer[1] = _red;
            commandBuffer[2] = _blue;
            commandBuffer[3] = _green;
            commandBuffer[4] = _blink;     // 0 is stable, 70 is fast blink, 100 is medium blink ?
            commandBuffer[5] = _music;
            commandBuffer[6] = 0x40;
            commandBuffer[7] = 0x02;
            commandBuffer[8] = ToggleSwitch; // Did this turn it off? controlCode & 0xFF 

            WriteOutputReport(commandBuffer);
        }

        //Set the public properties (first)
        public void SendCommand()
        {
            var commandBuffer = new Byte[9];
            commandBuffer[0] = 0x00;
            commandBuffer[1] = _red;
            commandBuffer[2] = _blue;
            commandBuffer[3] = _green;
            commandBuffer[4] = _blink;     // 0 is stable, 70 is fast blink, 100 is medium blink ?
            commandBuffer[5] = _music;   // byte5;
            commandBuffer[6] = _byte6;
            commandBuffer[7] = _byte7;
            commandBuffer[8] = _byte8; ; // Did this turn it off? controlCode & 0xFF 

            WriteOutputReport(commandBuffer);
        }

        private async void WriteOutputReport(Byte[] data)
        {
            var report = _store.Item2.CreateOutputReport();

            // Only grab the byte we need            
            Byte[] bytesToModify = data;

            WindowsRuntimeBufferExtensions.CopyTo(bytesToModify, 0, report.Data, 0, bytesToModify.Length);

            try
            {
                await _store.Item2.SendOutputReportAsync(report);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        public void Dispose()
        {
            _color = DEFAULT_COLOR;
        }
    }
}