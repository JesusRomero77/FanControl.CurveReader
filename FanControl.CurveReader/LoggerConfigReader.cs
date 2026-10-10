using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using static FanControl.CurveReader.ErrorLogger.Severity;

namespace FanControl.CurveReader
{
    public static class LoggerConfigReader
    {
        private const string LanguageAttributeName = "Language";
        private const string ConfigFileName = "CurveReaderLoggerConfig.xml";
        private const int MinIntervalSeconds = 1;
        private const int MaxIntervalSeconds = 3600;
        private const int MinThreshold = 1;
        private const int MaxThreshold = 10;

        private static readonly Dictionary<string, Filter> _controlFilters = new Dictionary<string, Filter>();
        private static readonly Dictionary<string, Filter> _sensorFilters = new Dictionary<string, Filter>();

        // Carga los filtros del perfil indicado (nombre de archivo con extensión, ej. "verano.json").
        // Si el archivo no existe, no contiene ese perfil o está mal formado, las listas quedan
        // vacías y todos los elementos usarán los valores por defecto.
        public static void Load(string profileFileName)
        {
            _controlFilters.Clear();
            _sensorFilters.Clear();

            // Mientras no se lea otro idioma del XML, el log se escribe en el de por defecto (inglés).
            Translator.SetLanguage(null);

            try
            {
                string configPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "FanControl",
                    ConfigFileName);
                if (!File.Exists(configPath)) return;

                XDocument document = XDocument.Load(configPath);

                // El idioma del log lo elige el usuario en el configurador y se guarda en la raíz del XML.
                // Si falta o no existe en el plugin, se mantiene el de por defecto.
                Translator.SetLanguage((string)document.Root.Attribute(LanguageAttributeName));

                if (string.IsNullOrWhiteSpace(profileFileName)) return;

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

                ReadElements(profile.Elements("Control"), _controlFilters);
                ReadElements(profile.Elements("Sensor"), _sensorFilters);
            }
            catch (Exception ex)
            {
                _controlFilters.Clear();
                _sensorFilters.Clear();
                ErrorLogger.Write(Error, ex);
            }
        }
        // Devuelve el filtro de un control por su Identifier, o uno con valores por defecto si no está.
        public static Filter GetControlFilter(string identifier)
        {
            return GetFilter(_controlFilters, identifier);
        }

        // Devuelve el filtro de un sensor por su Identifier, o uno con valores por defecto si no está.
        public static Filter GetSensorFilter(string identifier)
        {
            return GetFilter(_sensorFilters, identifier);
        }

        private static Filter GetFilter(Dictionary<string, Filter> filters, string identifier)
        {
            if (identifier != null && filters.TryGetValue(identifier, out Filter found)) return found;
            return new Filter(identifier);
        }

        // Recorre los elementos XML de un tipo y guarda sus filtros en el diccionario, usando
        // el Identifier como clave. Ignora los elementos sin Identifier.
        private static void ReadElements(IEnumerable<XElement> elements, Dictionary<string, Filter> destination)
        {
            foreach (XElement element in elements)
            {
                string identifier = (string)element.Attribute("Identifier");
                if (string.IsNullOrWhiteSpace(identifier)) continue;

                destination[identifier] = new Filter(
                    identifier,
                    ParseBool((string)element.Attribute("IncludedInLog"), true),
                    ParseInt((string)element.Attribute("IntervalSeconds"), 1, MinIntervalSeconds, MaxIntervalSeconds),
                    ParseInt((string)element.Attribute("Threshold"), 1, MinThreshold, MaxThreshold));
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