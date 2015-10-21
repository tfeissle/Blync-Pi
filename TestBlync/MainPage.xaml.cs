using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using BlyncLight;
using Windows.UI;
using System.Diagnostics;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using Windows.System.Threading;

//byte 5 is for playing the mp3 files
//18-30 play a tune single time
//49-59 plays never ending versions of the tunes

// Preliminary idea of what the command buffer does
//            commandBuffer[0] = 0x00;
//            commandBuffer[1] = red;
//            commandBuffer[2] = blue;
//            commandBuffer[3] = green;
//            commandBuffer[4] = Blink;     // 0 is stable, 70 is fast blink, 100 is medium blink ?
//            commandBuffer[5] = Music; 
//            commandBuffer[6] = unk; // 0x40;
//            commandBuffer[7] = unk; // 0x02;
//            commandBuffer[8] = unk?

//Blync Colors (from the SDK)
//Red
//Yellow
//Green
//Purple
//Blue
//No Color




namespace TestBlync
{
    public sealed partial class MainPage : Page
    {
        private Manager blyncManager;
        private static DeviceClient deviceClient;


        //check for a command every 5 seconds

        private static void ExampleTimerElapsedHandler(ThreadPoolTimer timer)
        {
            System.Diagnostics.Debug.WriteLine("Checking for Command");
            try
            {
                DeviceCommunication.ReceiveCommands(deviceClient).Wait();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            InitializeDevice();

            ThreadPoolTimer PeriodicTimer =
                ThreadPoolTimer.CreatePeriodicTimer(ExampleTimerElapsedHandler, TimeSpan.FromMilliseconds(5000));

        }
        private  void btnRed_Click(object sender, RoutedEventArgs e)
        {
            SetColor(Colors.Red, blyncManager);      
        }
        private void btnGreen_Click(object sender, RoutedEventArgs e)
        {
            SetColor(Colors.Green, blyncManager);
        }

        private void btnYellow_Click(object sender, RoutedEventArgs e)
        {
            SetColor(Colors.Yellow, blyncManager);
        }
        private void btnPurple_Click(object sender, RoutedEventArgs e)
        {
            SetColor(Colors.Purple, blyncManager);
        }
        private void btnBlue_Click(object sender, RoutedEventArgs e)
        {
            SetColor(Colors.Blue, blyncManager);
        }

        private async void InitializeDevice()
        {
            try
            {
                var manager = new BlyncLight.Manager();
                await manager.Init();
                blyncManager = manager;

                deviceClient = DeviceCommunication.InitializeDeviceCommunication();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private  void SetColor(Color newColor, Manager manager)
        {
            try
            {               
                manager.BlyncLight.StatusColor = newColor;
                DeviceCommunication.SendEvent(deviceClient, "Color=" + newColor.ToString());

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }
       
        private void btnTurnOff_Click(object sender, RoutedEventArgs e)
        {
            SetColor(Colors.Black, blyncManager);        
        }

        private void btnBlinkOn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                blyncManager.BlyncLight.Blink = 100;   //turns on the blink            
                blyncManager.BlyncLight.SendCommand();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private void btnBlinkOff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                blyncManager.BlyncLight.Blink = 0;   //turns off the blink            
                blyncManager.BlyncLight.SendCommand();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private void sldByte_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            try
            {
                string sliderValues = Convert.ToString(sldByte0.Value + "," + sldByte1.Value + "," + sldByte2.Value + "," + sldByte3.Value + "," + sldByte4.Value + "," + sldByte5.Value + "," + sldByte6.Value + "," + sldByte7.Value + "," + sldByte8.Value);
                blyncManager.BlyncLight.Music = Convert.ToByte(sldByte0.Value);
                System.Diagnostics.Debug.WriteLine("Slider Value: " + sliderValues);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e)
        {
            Button myButton = (Button)sender;
            string sliderValues = Convert.ToString(sldByte0.Value + "," + sldByte1.Value + "," + sldByte2.Value + "," + sldByte3.Value + "," + sldByte4.Value + "," + sldByte5.Value + "," + sldByte6.Value + "," + sldByte7.Value + "," + sldByte8.Value);

            try
            {

                if (myButton.Content.ToString() == "SET5")
                {
                    blyncManager.BlyncLight.Music = Convert.ToByte(sldByte5.Value);
                    blyncManager.BlyncLight.SendCommand();   //play music
                }
                if (myButton.Content.ToString() == "SET6")
                {
                    blyncManager.BlyncLight.Byte6 = Convert.ToByte(sldByte6.Value);
                    blyncManager.BlyncLight.SendCommand();   
                }
                if (myButton.Content.ToString() == "SET7")
                {
                    blyncManager.BlyncLight.Byte7 = Convert.ToByte(sldByte7.Value);
                    blyncManager.BlyncLight.SendCommand();
                }
                if (myButton.Content.ToString() == "SET8")
                {
                    blyncManager.BlyncLight.Byte8 = Convert.ToByte(sldByte8.Value);
                    blyncManager.BlyncLight.SendCommand();
                }

                System.Diagnostics.Debug.WriteLine("Slider Value: " + sliderValues);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Button Pressed: " + myButton.Content + " Value: " + sliderValues + " Error:" + ex.Message);
            }
        }

        private void btnMusicOff_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                blyncManager.BlyncLight.Music  = 0;   //turns off the music            
                blyncManager.BlyncLight.SendCommand();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error: " + ex.Message);
            }
        }
    }
}

