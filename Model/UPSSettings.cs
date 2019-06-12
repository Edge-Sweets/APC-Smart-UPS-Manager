using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace APCUPS
{
    public class UPSSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string _portName = "COM4";
        int _baudRate = 2400;
        Parity _parity = Parity.None;
        int _dataBits = 8;
        StopBits _stopBits = StopBits.One;
        int computerShutdownDelay = 20000;
        int shutdownTimeLeft = 20000;

        #region Properties
        /// <summary>
        /// The port to use (for example, COM1).
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set
            {
                if (!_portName.Equals(value))
                {
                    _portName = value;
                    SendPropertyChangedEvent("PortName");
                }
            }
        }
        /// <summary>
        /// The baud rate.
        /// </summary>
        public int BaudRate
        {
            get { return _baudRate; }
            set
            {
                if (_baudRate != value)
                {
                    _baudRate = value;
                    SendPropertyChangedEvent("BaudRate");
                }
            }
        }

        /// <summary>
        /// One of the Parity values.
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
            set
            {
                if (_parity != value)
                {
                    _parity = value;
                    SendPropertyChangedEvent("Parity");
                }
            }
        }
        /// <summary>
        /// The data bits value.
        /// </summary>
        public int DataBits
        {
            get { return _dataBits; }
            set
            {
                if (_dataBits != value)
                {
                    _dataBits = value;
                    SendPropertyChangedEvent("DataBits");
                }
            }
        }
        /// <summary>
        /// One of the StopBits values.
        /// </summary>
        public StopBits StopBits
        {
            get { return _stopBits; }
            set
            {
                if (_stopBits != value)
                {
                    _stopBits = value;
                    SendPropertyChangedEvent("StopBits");
                }
            }
        }

        public int ComputerShutdownDelay
        {
            get { return computerShutdownDelay / 1000; }
            set { computerShutdownDelay = value * 1000; SendPropertyChangedEvent("ComputerShutdownDelay"); }
        }

        public int ShutdownTimeLeft
        {
            get { return shutdownTimeLeft; }
            set { shutdownTimeLeft = value; SendPropertyChangedEvent("ShutdownTimeLeft"); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Send a PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of changed property</param>
        private void SendPropertyChangedEvent(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Serialize data to a settings file.
        /// </summary>
        /// <param name="info">Info to serialize</param>
        public static void Serialize(UPSSettings info)
        {
            string saveDirectory = @"C:\programdata\esco\";
            string fileName = "ups.xml";

            var serializer = new XmlSerializer(info.GetType());
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
            System.IO.File.Delete(saveDirectory + fileName);
            using (var writer = XmlWriter.Create(saveDirectory + fileName))
            {
                serializer.Serialize(writer, info);
            }
        }

        /// <summary>
        /// Deserialize data from settings file
        /// </summary>
        /// <returns>returns UPSSettings from settings file or default settings if no file found</returns>
        public static UPSSettings Deserialize()
        {
            string saveDirectory = @"C:\programdata\esco\";
            string fileName = "ups.xml";
            var serializer = new XmlSerializer(typeof(UPSSettings));
            UPSSettings settings = new UPSSettings();
            if (File.Exists(saveDirectory + fileName))
            {
                using (var reader = XmlReader.Create(saveDirectory + fileName))
                {
                    settings = (UPSSettings)serializer.Deserialize(reader);
                }
            }
            return settings;
        }

        #endregion
    }
}
