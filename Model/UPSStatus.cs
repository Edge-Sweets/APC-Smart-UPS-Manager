using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;

namespace APCUPS
{
    public class UPSStatus : INotifyPropertyChanged
    {

        #region Properties

        public enum PowerTypeEnum
        {
            Line,
            Battery
        }

        public enum AlarmDelayEnum
        {
            [Description("Five Seconds")]
            FiveSeconds,
            [Description("Thirty Seconds")]
            ThirtySeconds,
            [Description("Battery Only")]
            BatteryOnly,
            [Description("Disabled")]
            Disabled
        }

        public enum GracefulDelay
        {
            [Description("1 Minute")]
            OneMinute = 60,
            [Description("3 Minutes")]
            ThreeMinute = 180,
            [Description("5 Minutes")]
            FiveMinute = 300,
            [Description("10 Minutes")]
            TenMinute = 600
        }

        private PowerTypeEnum powerType;
        public PowerTypeEnum PowerType
        {
            get { return powerType; }
            set
            {
                if (powerType != value)
                {
                    powerType = value; OnPropertyChanged(new PropertyChangedEventArgs("PowerType"));
                }
            }
        }

        private string batteryVoltage;
        public string BatteryVoltage
        {
            get { return batteryVoltage; }
            set
            {
                if (batteryVoltage != value)
                {
                    batteryVoltage = value; OnPropertyChanged(new PropertyChangedEventArgs("BatteryVoltage"));
                }
            }
        }

        private string inputVoltage;
        public string InputVoltage
        {
            get { return inputVoltage; }
            set {
                if (inputVoltage != value)
                {
                    inputVoltage = value; OnPropertyChanged(new PropertyChangedEventArgs("InputVoltage"));
                }
            }
        }

        private string outputVoltage;
        public string OutputVoltage
        {
            get { return outputVoltage; }
            set
            {
                if (outputVoltage != value)
                {
                    outputVoltage = value; OnPropertyChanged(new PropertyChangedEventArgs("OutputVoltage"));
                }
            }
        }

        private string batteryLevel;
        public string BatteryLevel
        {
            get { return batteryLevel; }
            set
            {
                if (batteryLevel != value)
                {
                    batteryLevel = value; OnPropertyChanged(new PropertyChangedEventArgs("BatteryLevel"));
                }
            }
        }

        private string model;
        public string Model
        {
            get { return model; }
            set
            {
                if (model != value) 
                { 
                    model = value; OnPropertyChanged(new PropertyChangedEventArgs("Model")); 
                }
            }
        }

        private string shutdownDelay;
        public string ShutdownDelay
        {
            get { return shutdownDelay; }
            set
            {
                if (shutdownDelay != value)
                {
                    shutdownDelay = value; OnPropertyChanged(new PropertyChangedEventArgs("ShutdownDelay"));
                }
            }
        }

        private string alarmDelay;
        public string AlarmDelay
        {
            get { return alarmDelay; }
            set
            {
                if (alarmDelay != value)
                {
                    alarmDelay = value; OnPropertyChanged(new PropertyChangedEventArgs("AlarmDelay"));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion

        private UPSPortManager _manager;

        private Timer timer;

        public UPSStatus(UPSPortManager manager)
        {
            _manager = manager;
            timer = new Timer();
            timer.Interval = 2000;
            timer.Elapsed += t_Elapsed;
            timer.Start();
        }

        public void PauseTimer()
        {
            timer.Stop();
        }

        public void StartTimer()
        {
            timer.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_manager.PortActive)
            {
                _manager.WriteAndWaitForResponse("Y", 100); //only waiting to make sure it switched over
                BatteryVoltage = _manager.WriteAndWaitForResponse("B", 100);
                string pType = _manager.WriteAndWaitForResponse("Q", 100);
                switch (pType)
                {
                    case "08": PowerType = PowerTypeEnum.Line; break;
                    case "10": PowerType = PowerTypeEnum.Battery; break;
                }
                InputVoltage = _manager.WriteAndWaitForResponse("L", 100);
                OutputVoltage = _manager.WriteAndWaitForResponse("O", 100);
                BatteryLevel = _manager.WriteAndWaitForResponse("f", 100);
                UPSStatus.GracefulDelay shutdownDelayEnum = (UPSStatus.GracefulDelay)Enum.Parse(typeof(UPSStatus.GracefulDelay), _manager.WriteAndWaitForResponse("p", 100));
                ShutdownDelay = UPSStatus.GetEnumDescription(shutdownDelayEnum);
                AlarmDelay = GetEnumDescription(SetAlarmDelayEnum(_manager.WriteAndWaitForResponse("k", 100)));
                Model = _manager.WriteAndWaitForResponse(((char)1).ToString(), 175);
            }
        }

        private static AlarmDelayEnum SetAlarmDelayEnum(string currentDelay)
        {
            AlarmDelayEnum delayEnum = AlarmDelayEnum.Disabled;
            switch (currentDelay)
            {
                case "0": delayEnum = AlarmDelayEnum.FiveSeconds; break;
                case "T": delayEnum = AlarmDelayEnum.ThirtySeconds; break;
                case "L": delayEnum = AlarmDelayEnum.BatteryOnly; break;
                case "N": delayEnum = AlarmDelayEnum.Disabled; break;
            }
            return delayEnum;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
