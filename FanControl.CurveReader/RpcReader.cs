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

        // Lee los datos del RPC y recorre los sensores y controles vigilados para decidir
        // si cada uno debe registrarse.
        public static void UpdateValues()
        {
            SetRPCData();

            foreach (FilteredSensor filteredSensor in _filteredSensors)
                UpdateElement(filteredSensor);

            foreach (FilteredControl filteredControl in _filteredControls)
                UpdateElement(filteredControl);
        }

        // Decide si un sensor o control debe escribirse en el log y, si es así, lo escribe. Se descartan
        // en orden los que no toca registrar (no incluidos, o dentro de su intervalo de descanso) sin tocar
        // su valor. Solo a los que llegan hasta el final se les actualiza el valor, y entonces se comprueba
        // el umbral de cambio. La decisión la toma este método; el elemento solo aporta su filtro y su estado.
        private static void UpdateElement(ILoggableElement element)
        {
            if (!element.Filter.IncludedInLog) return;

            element.SecondsSinceLastLog++;
            if (element.HasBeenLogged && element.SecondsSinceLastLog < element.Filter.IntervalSeconds) return;

            if (!GetRPCValue(element.Identifier, out byte value)) return;
            element.Value = value;

            if (element.HasBeenLogged && Math.Abs(element.Value - element.LastLoggedValue) < element.Filter.Threshold) return;

            element.LastLoggedValue = element.Value;
            element.HasBeenLogged = true;
            element.SecondsSinceLastLog = 0;
            LogFileManager.WriteLog("[" + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + "] - " + GetElementTypeName(element)
                + ": " + Translator.Translate(TextId.RecordNameLabel) + " = " + element.Name
                + " | " + Translator.Translate(TextId.RecordValueLabel) + " = " + element.Value + "\r\n");
        }

        // Devuelve el tipo de elemento para el log, en el idioma actual: sensor o control.
        private static string GetElementTypeName(ILoggableElement element)
        {
            return Translator.Translate(element is FilteredSensor ? TextId.SensorType : TextId.ControlType);
        }
        // Busca en los datos del RPC el elemento con ese Identifier y devuelve su valor redondeado.
        // Devuelve false si el RPC no lo incluye en esta lectura.
        private static bool GetRPCValue(string identifier, out byte value)
        {
            foreach (SensorMessage rpcElement in _rpcData)
                if (rpcElement.Identifier == identifier)
                {
                    value = (byte)Math.Round(rpcElement.Value);
                    return true;
                }

            value = 0;
            return false;
        }
        private class CurveCalculator
        {
            //En alguna versión posterior se calcularán las curvas aquí. Como son cálculos complejos
            //entre las simples y las mixtas, y según qué clase de mixta, etc... vamos a dejar una clase entera sólo para ello.
        }
    }
}