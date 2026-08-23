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
        }
        public class RPCControl
        {
            public string Name { get; set; }
            public string RPCName { get; set; }
            public string Identifier { get; set; }
            public byte Value { get; set; }
            public byte PreviousValue { get; set; }
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

            foreach (RPCSensor sensor in _sensors)
                foreach (SensorMessage rpcSensor in _rpcData)
                    if (sensor.Identifier == rpcSensor.Identifier)
                    {
                        sensor.Value = (byte)Math.Round(rpcSensor.Value);
                        if (sensor.Value != sensor.PreviousValue)
                        {
                            CurveReaderPlugin.WriteLog("[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "] - SENSOR: Name = " + sensor.Name + " | Value = " + sensor.Value + "\r\n");
                            sensor.PreviousValue = sensor.Value;
                        }
                        break;
                    }

            foreach (RPCControl control in _controls)
                foreach (SensorMessage rpcControl in _rpcData)
                    if (control.Identifier == rpcControl.Identifier)
                    {
                        control.Value = (byte)Math.Round(rpcControl.Value);
                        if (control.Value != control.PreviousValue)
                        {
                            CurveReaderPlugin.WriteLog("[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "] - CONTROL: Name = " + control.Name + " | Value = " + control.Value + "\r\n");
                            control.PreviousValue = control.Value;
                        }
                        break;
                    }
        }
        private class CurveCalculator
        {
            //En alguna versión posterior se calcularán las curvas aquí. Como son cálculos complejos
            //entre las simples y las mixtas, y según qué clase de mixta, etc... vamos a dejar una clase entera sólo para ello.
        }
    }
}