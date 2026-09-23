using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace CurveReaderConfigurator
{
    /// <summary>
    /// Lee y escribe la configuración del logger en el archivo XML que usará el plugin,
    /// dentro de %LocalAppData%\FanControl.
    /// </summary>
    public static class LoggerConfigFile
    {
        private const string FolderName = "FanControl";
        private const string FileName = "CurveReaderLoggerConfig.xml";
        private const string IntervalSecondsAttributeName = "IntervalSeconds";

        // Nombres de los elementos y atributos del XML, iguales a los que usa el plugin.
        private const string RootElementName = "LoggerConfig";
        private const string ProfileElementName = "Profile";
        private const string ControlElementName = "Control";
        private const string SensorElementName = "Sensor";
        private const string VersionAttributeName = "Version";
        private const string FileNameAttributeName = "FileName";
        private const string IdentifierAttributeName = "Identifier";
        private const string NameAttributeName = "Name";
        private const string IncludedInLogAttributeName = "IncludedInLog";
        private const string ThresholdAttributeName = "Threshold";

        // Ruta completa del archivo. Se calcula con la carpeta AppData del usuario actual.
        public static string FilePath
        {
            get
            {
                string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(appDataFolder, FolderName, FileName);
            }
        }

        /// <summary>
        /// Lee la configuración. Si el archivo no existe devuelve una configuración vacía (sin
        /// perfiles). Devuelve null, con el motivo en 'error', si existe pero no se puede leer.
        /// </summary>
        public static LoggerConfig Load(out string error)
        {
            error = null;
            LoggerConfig config = new LoggerConfig();
            if (!File.Exists(FilePath)) return config;

            try
            {
                XDocument document = XDocument.Load(FilePath);
                if (document.Root == null || document.Root.Name != RootElementName)
                {
                    error = "El archivo de configuración no tiene el formato esperado.";
                    return null;
                }

                foreach (XElement profileElement in document.Root.Elements(ProfileElementName))
                {
                    string fileName = (string)profileElement.Attribute(FileNameAttributeName);
                    if (string.IsNullOrWhiteSpace(fileName)) continue;

                    ProfileConfig profile = new ProfileConfig { FileName = fileName };
                    ReadItems(profileElement, ControlElementName, profile.Controls);
                    ReadItems(profileElement, SensorElementName, profile.Sensors);
                    config.Profiles.Add(profile);
                }

                return config;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// Guarda la configuración. Escribe primero en un archivo temporal y después sustituye
        /// al definitivo, para que el archivo nunca quede a medias. Devuelve false, con el
        /// motivo en 'error', si no se ha podido guardar.
        /// </summary>
        public static bool Save(LoggerConfig config, out string error)
        {
            error = null;
            try
            {
                XElement root = new XElement(RootElementName,
                    new XAttribute(VersionAttributeName, LoggerConfig.CurrentVersion));

                foreach (ProfileConfig profile in config.Profiles)
                {
                    XElement profileElement = new XElement(ProfileElementName,
                        new XAttribute(FileNameAttributeName, profile.FileName));
                    AddItems(profileElement, ControlElementName, profile.Controls);
                    AddItems(profileElement, SensorElementName, profile.Sensors);
                    root.Add(profileElement);
                }

                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

                string tempPath = FilePath + ".tmp";
                new XDocument(root).Save(tempPath);

                if (File.Exists(FilePath)) File.Replace(tempPath, FilePath, null);
                else File.Move(tempPath, FilePath);

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        // Lee los controles o los sensores de un perfil. Los valores que falten o no sean
        // válidos se sustituyen por los valores por defecto.
        private static void ReadItems(XElement profileElement, string elementName, List<ItemConfig> items)
        {
            foreach (XElement element in profileElement.Elements(elementName))
            {
                string identifier = (string)element.Attribute(IdentifierAttributeName);
                if (string.IsNullOrWhiteSpace(identifier)) continue;

                items.Add(new ItemConfig
                {
                    Identifier = identifier,
                    Name = (string)element.Attribute(NameAttributeName),
                    IncludedInLog = ParseIncludedInLog((string)element.Attribute(IncludedInLogAttributeName)),
                    IntervalSeconds = ParseIntervalSeconds((string)element.Attribute(IntervalSecondsAttributeName)),
                    Threshold = ParseThreshold((string)element.Attribute(ThresholdAttributeName))
                });
            }
        }

        // Escribe los controles o los sensores de un perfil como elementos del XML.
        private static void AddItems(XElement profileElement, string elementName, List<ItemConfig> items)
        {
            foreach (ItemConfig item in items)
            {
                profileElement.Add(new XElement(elementName,
                    new XAttribute(IdentifierAttributeName, item.Identifier),
                    new XAttribute(NameAttributeName, item.Name ?? string.Empty),
                    new XAttribute(IncludedInLogAttributeName, item.IncludedInLog),
                    new XAttribute(IntervalSecondsAttributeName, item.IntervalSeconds),
                    new XAttribute(ThresholdAttributeName, item.Threshold)));
            }
        }

        private static bool ParseIncludedInLog(string text)
        {
            bool value;
            return bool.TryParse(text, out value) ? value : ItemConfig.DefaultIncludedInLog;
        }

        // El umbral debe ser un entero entre el mínimo y el máximo; si no, se usa el valor por defecto.
        private static int ParseThreshold(string text)
        {
            int value;
            if (!int.TryParse(text, out value)) return ItemConfig.DefaultThreshold;
            if (value < ItemConfig.MinThreshold || value > ItemConfig.MaxThreshold) return ItemConfig.DefaultThreshold;
            return value;
        }

        // El intervalo debe ser un entero entre el mínimo y el máximo; si no, se usa el valor por defecto.
        private static int ParseIntervalSeconds(string text)
        {
            int value;
            if (!int.TryParse(text, out value)) return ItemConfig.DefaultIntervalSeconds;
            if (value < ItemConfig.MinIntervalSeconds || value > ItemConfig.MaxIntervalSeconds) return ItemConfig.DefaultIntervalSeconds;
            return value;
        }
    }
}