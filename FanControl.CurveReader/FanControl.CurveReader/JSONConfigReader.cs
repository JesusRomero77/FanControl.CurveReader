using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace FanControl.CurveReader
{
    public static class JSONConfigReader
    {
        // Última información leída del CACHE y fecha de modificación del archivo en ese momento.
        // Mientras la fecha no cambie, se devuelve la información guardada sin volver a leer el archivo.
        private static readonly object _cacheLock = new object();
        private static DateTime _cacheLastWriteTimeUtc = DateTime.MinValue;
        private static (string FileName, string FilePath) _cachedConfigInfo = (null, null);
        private static string _fileName;
        private static string _filePath;
        private static readonly List<Control> _controls = new List<Control>();
        private static readonly List<Curve> _curves = new List<Curve>();
        private static readonly List<Sensor> _sensors = new List<Sensor>();
        // Ancho de las líneas de "=" del resumen del perfil activo.
        private const int SummaryWidth = 35;

        public class Control
        {
            public string Name { get; set; }
            public string Identifier { get; set; }
            public string DataSource { get; set; }
        }
        public class Curve
        {
            public string Name { get; set; }
            public List<string> DataSource { get; set; }
        }
        public class Sensor
        {
            public string Name { get; set; }
            public string Identifier { get; set; }
            public string DataSource { get; set; }
        }


        public static (string FileName, string FilePath) GetCurrentConfigInfo()
        {
            // Se bloquea porque lo llaman a la vez Update (hilo de FanControl) e InitialStartup (hilo propio),
            // y la fecha y la información guardadas deben cambiar siempre juntas.
            lock (_cacheLock)
            {
                try
                {
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string configurationsFolder = Path.Combine(baseDirectory, "Configurations");
                    string cachePath = Path.Combine(configurationsFolder, "CACHE");
                    if (!File.Exists(cachePath)) return (null, null);

                    // La fecha se lee ANTES del contenido: si FanControl modifica el archivo entre las dos
                    // operaciones, se guarda la fecha antigua y la siguiente llamada volverá a leerlo.
                    DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(cachePath);
                    if (lastWriteTimeUtc == _cacheLastWriteTimeUtc) return _cachedConfigInfo;

                    JObject cache = JObject.Parse(File.ReadAllText(cachePath));
                    string fileName = cache["CurrentConfigFileName"]?.ToString();
                    string customConfigFolder = cache["CustomConfigFolder"]?.ToString();
                    if (string.IsNullOrWhiteSpace(fileName)) return (null, null);
                    string configFolder = !string.IsNullOrWhiteSpace(customConfigFolder) ? customConfigFolder : configurationsFolder;
                    string filePath = Path.Combine(configFolder, fileName);

                    // Solo se guarda el resultado si la lectura ha ido bien, para que un fallo se reintente.
                    _cachedConfigInfo = (fileName, filePath);
                    _cacheLastWriteTimeUtc = lastWriteTimeUtc;
                    return _cachedConfigInfo;
                }
                catch (Exception ex)
                {
                    ErrorLogger.Write(ErrorLogger.Severity.Error, ex);
                    return (null, null);
                }
            }
        }
        public static void SetJSONObjects()
        {
            try
            {
                var configInfo = GetCurrentConfigInfo();
                _fileName = configInfo.FileName;
                _filePath = configInfo.FilePath;
                _controls.Clear();
                _curves.Clear();
                _sensors.Clear();

                LoggerConfigReader.Load(_fileName);

                if (string.IsNullOrWhiteSpace(_fileName) || string.IsNullOrWhiteSpace(_filePath) || !File.Exists(_filePath)) return;

                JObject configuration = JObject.Parse(File.ReadAllText(_filePath));

                JObject fanControl = configuration["FanControl"] as JObject;

                if (fanControl == null) return;

                JArray controls = fanControl["Controls"] as JArray;

                if (controls == null) return;

                foreach (JToken token in controls)
                {
                    JObject control = token as JObject;
                    if (control == null) continue;
                    if (control["IsHidden"]?.Value<bool?>() == true || control["Enable"]?.Value<bool?>() == false) continue;
                    string name = control["NickName"]?.ToString();
                    string identifier = control["Identifier"]?.ToString();
                    string dataSource = control["SelectedFanCurve"]?["Name"]?.ToString();
                    Control newControl = new Control { Name = name, Identifier = identifier, DataSource = dataSource};
                    _controls.Add(newControl);
                }
                JArray fanCurves = fanControl["FanCurves"] as JArray;
                if (fanCurves == null) return;
                foreach (JToken token in fanCurves)
                {
                    JObject curve = token as JObject;
                    if (curve == null) continue;
                    if (curve["IsHidden"]?.Value<bool?>() == true) continue;
                    string name = curve["Name"]?.ToString();
                    var dataSource = new List<string>();
                    JObject selectedTempSource = curve["SelectedTempSource"] as JObject;
                    if (selectedTempSource != null)
                    {
                        string identifier = selectedTempSource["Identifier"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(identifier)) dataSource.Add(identifier);
                    }
                    JArray selectedFanCurves = curve["SelectedFanCurves"] as JArray;
                    if (selectedFanCurves != null)
                        foreach (JToken selectedCurve in selectedFanCurves)
                        {
                            string curveName = selectedCurve["Name"]?.ToString();
                            if (!string.IsNullOrWhiteSpace(curveName)) dataSource.Add(curveName);
                        }
                    
                    Curve newCurve = new Curve {Name = name, DataSource = dataSource};
                    _curves.Add(newCurve);
                }
                JArray customSensors = fanControl["CustomSensors"] as JArray;
                if (customSensors == null) return;
                foreach (JToken token in customSensors)
                {
                    JObject sensor = token as JObject;
                    if (sensor == null) continue;
                    if (sensor["IsHidden"]?.Value<bool?>() == true) continue;
                    string name = sensor["NickName"]?.ToString();
                    string identifier = sensor["Identifier"]?.ToString();
                    string dataSource = sensor["SelectedTempSource"]?["Identifier"]?.ToString();
                    Sensor newSensor = new Sensor{Name = name, Identifier = identifier, DataSource = dataSource};
                    _sensors.Add(newSensor);
                }
                WriteCurrentConfiguration();//Imprimimos un resumen de la configuración actual en el archivo log.
            }
            catch (Exception ex)
            {
                ErrorLogger.Write(ErrorLogger.Severity.Error, ex);
            }
        }
        private static void WriteCurrentConfiguration()
        {
            string text = "\r\n\r\n" + LogFileManager.BuildHeader(Translator.Translate(TextId.ActiveProfile), SummaryWidth) + "\r\n";

            text += Translator.Translate(TextId.ProfileNameLabel) + " " + Path.GetFileNameWithoutExtension(GetFileName()) + "\r\n";
            text += Translator.Translate(TextId.ProfilePathLabel) + " " + GetFilePath() + "\r\n";
            text += new string('=', SummaryWidth) + "\r\n\r\n";
            text += Translator.Translate(TextId.ControlsHeading) + "\r\n";
            foreach (Control control in GetJSONControls()) text += "- " + control.Name + "\r\n";

            text += "\r\n" + Translator.Translate(TextId.CurvesHeading) + "\r\n";
            foreach (Curve curve in GetJSONCurves()) text += "- " + curve.Name + "\r\n";

            text += "\r\n" + Translator.Translate(TextId.SensorsHeading) + "\r\n";
            foreach (Sensor sensor in GetJSONSensors()) text += "- " + sensor.Name + "\r\n";

            text += new string('=', SummaryWidth) + "\r\n\r\n";

            LogFileManager.AddProfileSummary(text);
            LogFileManager.WriteLog(text);
        }
        public static string GetFileName()
        {
            return _fileName;
        }
        public static string GetFilePath()
        {
            return _filePath;
        }
        public static List<Control> GetJSONControls()
        {
            return _controls;
        }
        public static List<Curve> GetJSONCurves()
        {
            return _curves;
        }
        public static List<Sensor> GetJSONSensors()
        {
            return _sensors;
        }
    }
}