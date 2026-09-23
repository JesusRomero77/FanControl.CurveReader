using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;

namespace FanControl.CurveReader
{
    public static class LoggerConfigReader
    {
        private const string ConfigFileName = "CurveReaderLoggerConfig.xml";
        private const int MinIntervalSeconds = 1;
        private const int MaxIntervalSeconds = 3600;
        private const int MinThreshold = 1;
        private const int MaxThreshold = 10;

        private static readonly Dictionary<string, ElementSettings> _controlSettings = new Dictionary<string, ElementSettings>();
        private static readonly Dictionary<string, ElementSettings> _sensorSettings = new Dictionary<string, ElementSettings>();

        // Ajustes de un control o sensor. Los valores iniciales son los valores por defecto
        // que se usan cuando no hay archivo de configuración o falta el elemento.
        public class ElementSettings
        {
            public string Identifier { get; set; }
            public bool IncludedInLog { get; set; } = true;
            public int IntervalSeconds { get; set; } = 1;
            public int Threshold { get; set; } = 1;
        }

        // Carga los ajustes del perfil indicado (nombre de archivo con extensión, ej. "verano.json").
        // Si el archivo no existe, no contiene ese perfil o está mal formado, las listas quedan
        // vacías y todos los elementos usarán los valores por defecto.
        public static void Load(string profileFileName)
        {
            _controlSettings.Clear();
            _sensorSettings.Clear();

            if (string.IsNullOrWhiteSpace(profileFileName)) return;

            try
            {
                string configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "FanControl",
                    ConfigFileName);
                if (!File.Exists(configPath)) return;

                XDocument document = XDocument.Load(configPath);
                XElement profile = null;
                foreach (XElement candidate in document.Root.Elements("Profile"))
                {
                    string candidateName = (string)candidate.Attribute("FileName");
                    if (string.Equals(candidateName, profileFileName, StringComparison.OrdinalIgnoreCase))
                    {
                        profile = candidate;
                        break;
                    }
                }
                if (profile == null) return;

                ReadElements(profile.Elements("Control"), _controlSettings);
                ReadElements(profile.Elements("Sensor"), _sensorSettings);
            }
            catch (Exception ex)
            {
                _controlSettings.Clear();
                _sensorSettings.Clear();
                LogFileManager.WriteLog("ERROR leyendo configuración del logger:\r\n" + ex + "\r\n");
            }
        }

        // Devuelve los ajustes de un control por su Identifier, o los valores por defecto si no está.
        public static ElementSettings GetControlSettings(string identifier)
        {
            return GetSettings(_controlSettings, identifier);
        }

        // Devuelve los ajustes de un sensor por su Identifier, o los valores por defecto si no está.
        public static ElementSettings GetSensorSettings(string identifier)
        {
            return GetSettings(_sensorSettings, identifier);
        }

        private static ElementSettings GetSettings(Dictionary<string, ElementSettings> settings, string identifier)
        {
            if (identifier != null && settings.TryGetValue(identifier, out ElementSettings found)) return found;
            return new ElementSettings { Identifier = identifier };
        }

        // Recorre los elementos XML de un tipo y guarda sus ajustes en el diccionario, usando
        // el Identifier como clave. Ignora los elementos sin Identifier.
        private static void ReadElements(IEnumerable<XElement> elements, Dictionary<string, ElementSettings> destination)
        {
            foreach (XElement element in elements)
            {
                string identifier = (string)element.Attribute("Identifier");
                if (string.IsNullOrWhiteSpace(identifier)) continue;

                destination[identifier] = new ElementSettings
                {
                    Identifier = identifier,
                    IncludedInLog = ParseBool((string)element.Attribute("IncludedInLog"), true),
                    IntervalSeconds = ParseInt((string)element.Attribute("IntervalSeconds"), 1, MinIntervalSeconds, MaxIntervalSeconds),
                    Threshold = ParseInt((string)element.Attribute("Threshold"), 1, MinThreshold, MaxThreshold)
                };
            }
        }

        // Convierte un texto a bool; si no es válido devuelve el valor por defecto.
        private static bool ParseBool(string text, bool defaultValue)
        {
            return bool.TryParse(text, out bool value) ? value : defaultValue;
        }

        // Convierte un texto a entero y lo limita al rango permitido; si no es válido
        // devuelve el valor por defecto.
        private static int ParseInt(string text, int defaultValue, int min, int max)
        {
            if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)) return defaultValue;
            return Math.Max(min, Math.Min(max, value));
        }
    }
}