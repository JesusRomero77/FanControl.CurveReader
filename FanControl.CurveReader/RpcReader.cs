using FanControl.IPC;
using System;
using System.Collections.Generic;

namespace FanControl.CurveReader
{
    public static class RPCReader
    {
        // Datos en bruto obtenidos por RPC; ConfigMatcher los usa para emparejar con el JSON.
        private static readonly List<RPCSensor> _rpcSensors = new List<RPCSensor>();
        private static readonly List<RPCControl> _rpcControls = new List<RPCControl>();
        private static readonly List<SensorMessage> _rpcData = new List<SensorMessage>();

        // Objetos que han hecho correspondencia con el perfil activo; son los que se vigilan en cada lectura.
        private static readonly List<FilteredSensor> _filteredSensors = new List<FilteredSensor>();
        private static readonly List<FilteredControl> _filteredControls = new List<FilteredControl>();

        public class RPCSensor
        {
            public string Name { get; set; }
            public string Identifier { get; set; }
        }
        public class RPCControl
        {
            public string Name { get; set; }
            public string Identifier { get; set; }
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
            _rpcSensors.Clear();
            foreach (SensorMessage sensor in _rpcData)
            {
                if (string.IsNullOrWhiteSpace(sensor.Name) || string.IsNullOrWhiteSpace(sensor.Identifier)) continue;
                _rpcSensors.Add(new RPCSensor { Name = sensor.Name, Identifier = sensor.Identifier });
            }
        }
        private static void SetRPCControls()
        {
            _rpcControls.Clear();
            foreach (SensorMessage control in _rpcData)
            {
                if (control.Type != SensorMessageType.Control) continue;
                if (string.IsNullOrWhiteSpace(control.Name) || string.IsNullOrWhiteSpace(control.Identifier)) continue;
                _rpcControls.Add(new RPCControl { Name = control.Name, Identifier = control.Identifier });
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

        // Sustituye los objetos vigilados por los que ha creado ConfigMatcher para el perfil activo.
        // Al ser objetos nuevos, su estado de registro empieza de cero y la primera lectura se escribe siempre.
        public static void SetMatchedObjects()
        {
            _filteredSensors.Clear();
            _filteredControls.Clear();
            _filteredSensors.AddRange(ConfigMatcher.GetMatchedSensors());
            _filteredControls.AddRange(ConfigMatcher.GetMatchedControls());
        }
        public static List<RPCSensor> GetRPCSensors()
        {
            return _rpcSensors;
        }
        public static List<RPCControl> GetRPCControls()
        {
            return _rpcControls;
        }
        public static void UpdateValues()
        {
            SetRPCData();

            // Sensores: busca el valor actual de cada uno en los datos del RPC y lo escribe en el log
            // solo si su filtro lo permite (la decisión la toma el propio FilteredSensor).
            foreach (FilteredSensor sensor in _filteredSensors)
                foreach (SensorMessage rpcSensor in _rpcData)
                    if (sensor.Identifier == rpcSensor.Identifier)
                    {
                        if (sensor.Update((byte)Math.Round(rpcSensor.Value)))
                            LogFileManager.WriteLog("[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "] - SENSOR: Name = " + sensor.Name + " | Value = " + sensor.Value + "\r\n");
                        break;
                    }

            // Controles: misma lógica que los sensores.
            foreach (FilteredControl control in _filteredControls)
                foreach (SensorMessage rpcControl in _rpcData)
                    if (control.Identifier == rpcControl.Identifier)
                    {
                        if (control.Update((byte)Math.Round(rpcControl.Value)))
                            LogFileManager.WriteLog("[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "] - CONTROL: Name = " + control.Name + " | Value = " + control.Value + "\r\n");
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