using FanControl.IPC;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static FanControl.CurveReader.JSONConfigReader;


namespace FanControl.CurveReader
{
    public static class RPCReader
    {
        private static readonly List<RPCSensor> _sensors = new List<RPCSensor>();
        private static readonly List<RPCControl> _controls = new List<RPCControl>();
        private static readonly List<SensorMessage> _rpcData = new List<SensorMessage>();
        public class RPCSensor
        {
            public string Name { get; set; }
            public string RPCName { get; set; }
            public string Identifier { get; set; }
            public string PhysicalIdentifier { get; set; }
            public byte Value { get; set; }
            public byte PreviousValue { get; set; }
            public LoggerConfigReader.ElementSettings Settings { get; set; } = new LoggerConfigReader.ElementSettings();
            public int SecondsSinceLastLog { get; set; }
            public bool HasBeenLogged { get; set; }
        }
        public class RPCControl
        {
            public string Name { get; set; }
            public string RPCName { get; set; }
            public string Identifier { get; set; }
            public byte Value { get; set; }
            public byte PreviousValue { get; set; }
            public LoggerConfigReader.ElementSettings Settings { get; set; } = new LoggerConfigReader.ElementSettings();
            public int SecondsSinceLastLog { get; set; }
            public bool HasBeenLogged { get; set; }
        }
        public class RPCCurves
        {
            public string Name { get; set; }
            public string RPCName { get; set; }
            public List<string> DataSource { get; set; }
            public byte Value { get; set; }
            public byte PreviousValue { get; set; }
        }
        private static void SetRPCData()
        {
            _rpcData.Clear();
            var client = IPCFactory.GetSensorClient();
            var reply = client.GetAllSensors(new GetAllSensorsRequest());
            foreach (SensorMessage sensor in reply.Sensors)
            {
                _rpcData.Add(sensor);
            }
        }
        private static void SetRPCSensors()
        {
            _sensors.Clear();
            foreach (SensorMessage sensor in _rpcData)
            {
                if (string.IsNullOrWhiteSpace(sensor.Name) || string.IsNullOrWhiteSpace(sensor.Identifier)) continue;
                _sensors.Add(new RPCSensor { Name = sensor.Name, RPCName = sensor.Name, Identifier = sensor.Identifier, PhysicalIdentifier = null });
            }
        }
        private static void SetRPCControls()
        {
            _controls.Clear();
            foreach (SensorMessage control in _rpcData)
            {
                if (control.Type != SensorMessageType.Control) continue;
                if (string.IsNullOrWhiteSpace(control.Name) || string.IsNullOrWhiteSpace(control.Identifier)) continue;
                _controls.Add(new RPCControl { Name = control.Name, RPCName = control.Name, Identifier = control.Identifier });
            }
        }
        private static void SetRPCCurves()
        {//Quizá en un futuro se pueda crear este método con datos de FanControl.IPC.dll
        }
        public static void SetRPCObjects()
        {
            SetRPCData();
            SetRPCSensors();
            SetRPCControls();
            SetRPCCurves();
        }
        public static void SetMatchedObjects()
        {
            _sensors.Clear();
            _controls.Clear();
            _sensors.AddRange(ConfigMatcher.GetMatchedSensors());
            _controls.AddRange(ConfigMatcher.GetMatchedControls());

            // Asigna a cada sensor y control sus ajustes del logger (leídos del XML) y reinicia su estado
            // de registro, para que la primera lectura del perfil se escriba siempre.
            foreach (RPCSensor sensor in _sensors)
            {
                sensor.Settings = LoggerConfigReader.GetSensorSettings(sensor.Identifier);
                sensor.SecondsSinceLastLog = 0;
                sensor.HasBeenLogged = false;
            }
            foreach (RPCControl control in _controls)
            {
                control.Settings = LoggerConfigReader.GetControlSettings(control.Identifier);
                control.SecondsSinceLastLog = 0;
                control.HasBeenLogged = false;
            }
        }
        public static List<RPCSensor> GetRPCSensors()
        {
            return _sensors;
        }
        public static List<RPCControl> GetRPCControls()
        {
            return _controls;
        }
        public static void UpdateValues()
        {
            SetRPCData();

            // Sensores: lee el valor actual de cada uno y lo escribe en el log solo si sus ajustes lo permiten.
            foreach (RPCSensor sensor in _sensors)
                foreach (SensorMessage rpcSensor in _rpcData)
                    if (sensor.Identifier == rpcSensor.Identifier)
                    {
                        sensor.Value = (byte)Math.Round(rpcSensor.Value);
                        sensor.SecondsSinceLastLog++;
                        if (MustLog(sensor.Settings, sensor.HasBeenLogged, sensor.SecondsSinceLastLog, sensor.Value, sensor.PreviousValue))
                        {
                            LogFileManager.WriteLog("[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "] - SENSOR: Name = " + sensor.Name + " | Value = " + sensor.Value + "\r\n");
                            sensor.PreviousValue = sensor.Value;
                            sensor.HasBeenLogged = true;
                            sensor.SecondsSinceLastLog = 0;
                        }
                        break;
                    }

            // Controles: misma lógica que los sensores.
            foreach (RPCControl control in _controls)
                foreach (SensorMessage rpcControl in _rpcData)
                    if (control.Identifier == rpcControl.Identifier)
                    {
                        control.Value = (byte)Math.Round(rpcControl.Value);
                        control.SecondsSinceLastLog++;
                        if (MustLog(control.Settings, control.HasBeenLogged, control.SecondsSinceLastLog, control.Value, control.PreviousValue))
                        {
                            LogFileManager.WriteLog("[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "] - CONTROL: Name = " + control.Name + " | Value = " + control.Value + "\r\n");
                            control.PreviousValue = control.Value;
                            control.HasBeenLogged = true;
                            control.SecondsSinceLastLog = 0;
                        }
                        break;
                    }
        }

        // Decide si un sensor o control debe escribirse en el log en esta lectura, según sus ajustes:
        // - Si no está incluido en el log, nunca se escribe.
        // - La primera lectura siempre se escribe.
        // - Tras escribir, "descansa" IntervalSeconds segundos (contados desde el último registro).
        // - Pasado ese tiempo, se escribe cuando el valor difiere del último escrito en Threshold o más.
        private static bool MustLog(LoggerConfigReader.ElementSettings settings, bool hasBeenLogged, int secondsSinceLastLog, byte value, byte previousValue)
        {
            if (!settings.IncludedInLog) return false;
            if (!hasBeenLogged) return true;
            if (secondsSinceLastLog < settings.IntervalSeconds) return false;
            return Math.Abs(value - previousValue) >= settings.Threshold;
        }
        private class CurveCalculator
        {
            //En alguna versión posterior se calcularán las curvas aquí. Como son cálculos complejos
            //entre las simples y las mixtas, y según qué clase de mixta, etc... vamos a dejar una clase entera sólo para ello.
        }
    }
}