using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;

namespace APCUPS
{
    public class UPSPortManager : IDisposable
    {

        public bool PortActive { get; private set; }

        private Timer heartbeatTimer;

        public UPSPortManager(UPSSettings settings)
        {
            _currentSerialSettings = settings;
            _currentSerialSettings.PropertyChanged += _currentSerialSettings_PropertyChanged;
            FindUPSSerialPort();

            heartbeatTimer = new Timer();
            heartbeatTimer.Interval = 20000;
            heartbeatTimer.Elapsed += heartbeatTimer_Elapsed;
            heartbeatTimer.Start();
        }

        //this method will likely never do anything. It required the USB
        //to be unplugged, which causes an exception in windows.
        void heartbeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (!PortActive)
            {
                FindUPSSerialPort();
            }
        }

        ~UPSPortManager()
        {
            Dispose(false);
        }

        #region Fields
        private SerialPort _serialPort;
        private UPSSettings _currentSerialSettings;
        private string _latestRecieved = String.Empty;
        public event EventHandler<SerialDataEventArgs> NewSerialDataRecieved;

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the current serial port settings
        /// </summary>
        public UPSSettings CurrentSerialSettings
        {
            get { return _currentSerialSettings; }
            set { _currentSerialSettings = value; }
        }

        #endregion

        #region Event handlers

        void _currentSerialSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UPSSettings.Serialize(_currentSerialSettings);
        }


        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int dataLength = _serialPort.BytesToRead;
                byte[] data = new byte[dataLength];
                int nbrDataRead = _serialPort.Read(data, 0, dataLength);
                if (nbrDataRead == 0)
                    return;

                // Send data to whom ever interested
                if (NewSerialDataRecieved != null)
                    NewSerialDataRecieved(this, new SerialDataEventArgs(data));
            }
            catch (Exception)
            {

            }
        }

        public void WriteSerial(string write)
        {
            lock (lockObject)
            {
                try
                {
                    _serialPort.Write(write);
                }
                catch (Exception)
                {
                    PortActive = false;
                }
            }
        }

        /// <summary>
        /// Used to send a command and wait for a response.
        /// </summary>
        /// <param name="write">command to send</param>
        /// <param name="delay">delay in milliseconds to wait for response. Use larger number if expecting more data back</param>
        /// <returns>response from serial port</returns>
        public string WriteAndWaitForResponse(string write, int delay)
        {
            string response = null;
            lock (lockObject)
            {

                try
                {
                    _serialPort.DataReceived -= _serialPort_DataReceived;
                    _serialPort.Write(write);
                    System.Threading.Thread.Sleep(delay);
                    int dataLength = _serialPort.BytesToRead;
                    byte[] data = new byte[dataLength];
                    int nbrDataRead = _serialPort.Read(data, 0, dataLength);
                    if (nbrDataRead != 0)
                    {
                        response = System.Text.Encoding.Default.GetString(data).Replace("\r\n", "");
                    }
                    _serialPort.DataReceived += _serialPort_DataReceived;
                }
                catch (Exception)
                {
                    PortActive = false;
                    //the write failed for some reason. mark the port inactive.
                }
            }
            return response;
        }

        #endregion

        #region Methods

        public object lockObject = new object();

        public void FindUPSSerialPort()
        {
            lock (lockObject)
            {
                PortActive = false;
                // Finding installed serial ports on hardware
                string[] portNameCollection = SerialPort.GetPortNames();

                for (int i = 0; i < portNameCollection.Length; i++)
                {
                    portNameCollection[i] = "COM" + Regex.Replace(portNameCollection[i], @"\D*(\d+)\D*", @"$1");
                }
                // If serial ports are found, we select the first found
                if (portNameCollection.Length > 0)
                {
                    _serialPort = new SerialPort(
                    _currentSerialSettings.PortName,
                    _currentSerialSettings.BaudRate,
                    _currentSerialSettings.Parity,
                    _currentSerialSettings.DataBits,
                    _currentSerialSettings.StopBits);
                    _serialPort.WriteTimeout = 1000;
                    _serialPort.ReadTimeout = 1000;

                    if (_serialPort.PortName != "COM00")
                    {
                        //this is likely the last saved COM port. See if it's still
                        //the correct one for the UPS
                        try
                        {
                            _serialPort.Open();
                            WriteAndWaitForResponse("Y", 100);
                            string model = WriteAndWaitForResponse(((char)1).ToString(), 200);
                            if (model.ToLower().Contains("smart-ups"))
                            {
                                //this is our serial port.
                                PortActive = true;
                                return;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }

                    foreach (string portName in portNameCollection)
                    {
                        try
                        {
                            if (_serialPort.IsOpen)
                            {
                                _serialPort.Close();
                            }
                            _serialPort.PortName = portName;
                            //    _currentSerialSettings.PortName = portName;
                            _serialPort.Open();
                            //try to enter smart mode
                            WriteAndWaitForResponse("Y", 100);
                            string model = WriteAndWaitForResponse(((char)1).ToString(), 200);
                            if (model.ToLower().Contains("smart-ups"))
                            {
                                //this is our serial port.
                                PortActive = true;
                                _currentSerialSettings.PortName = portName;
                                //save the settings once we've found the right port
                                UPSSettings.Serialize(_currentSerialSettings);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                _serialPort.Close();
                            }
                            catch (Exception) { }
                        }
                    }
                }
            }
        }

        // Call to release serial port
        public void Dispose()
        {
            Dispose(true);
        }

        // Part of basic design pattern for implementing Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serialPort.DataReceived -= new SerialDataReceivedEventHandler(_serialPort_DataReceived);
            }
            // Releasing serial port (and other unmanaged objects)
            if (_serialPort != null)
            {
                try
                {
                    if (_serialPort.IsOpen)
                        _serialPort.Close();

                    _serialPort.Dispose();
                }
                catch (Exception) { }
            }
        }


        #endregion

    }

    /// <summary>
    /// EventArgs used to send bytes recieved on serial port
    /// </summary>
    public class SerialDataEventArgs : EventArgs
    {
        public SerialDataEventArgs(byte[] dataInByteArray)
        {
            Data = dataInByteArray;
        }

        /// <summary>
        /// Byte array containing data from serial port
        /// </summary>
        public byte[] Data;
    }
}
