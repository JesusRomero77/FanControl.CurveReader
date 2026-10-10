using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FanControl.CurveReader
{
    /// <summary>
    /// Un identificador por cada texto fijo que el plugin escribe en los logs. Solo contiene el
    /// nombre del texto; el texto en sí está en los diccionarios de cada idioma (Language.cs).
    /// </summary>
    public enum TextId
    {
        EndOfLog, ActiveProfile,
        ProfileNameLabel, ProfilePathLabel,
        ControlsHeading, CurvesHeading, SensorsHeading,
        ControlType, SensorType, RecordNameLabel, RecordValueLabel,
        UnknownOrigin
    }

    /// <summary>
    /// Devuelve los textos fijos del log en el idioma actual, que es inglés hasta que
    /// LoggerConfigReader lee el idioma elegido en el configurador. Los idiomas disponibles son las
    /// clases que heredan de Language; aquí no hay que tocar nada para añadir uno.
    /// </summary>
    public static class Translator
    {
        // Idioma de arranque y de reserva. Debe existir una clase de idioma con este Name.
        private const string DefaultLanguageName = "English";

        // El orden de estas tres líneas importa: cada una usa la anterior. El idioma actual es
        // volatile porque lo lee el hilo de Update y lo cambia el de InitialStartup.
        private static readonly List<Language> _languages = DiscoverLanguages();
        private static readonly Language _defaultLanguage = FindLanguage(DefaultLanguageName);
        private static volatile Language _currentLanguage = _defaultLanguage;

        /// <summary>
        /// Cambia el idioma actual. Si no hay ningún idioma con ese nombre (o es null), se usa el
        /// de por defecto.
        /// </summary>
        public static void SetLanguage(string languageName)
        {
            _currentLanguage = FindLanguage(languageName) ?? _defaultLanguage;
        }

        /// <summary>
        /// Devuelve el texto en el idioma actual. Si a ese idioma le falta, devuelve el inglés, y si
        /// tampoco está, el nombre del identificador, para que el fallo se vea.
        /// </summary>
        public static string Translate(TextId id)
        {
            string text;
            Language current = _currentLanguage;
            if (current != null && current.Texts.TryGetValue(id, out text)) return text;
            if (_defaultLanguage != null && _defaultLanguage.Texts.TryGetValue(id, out text)) return text;
            return id.ToString();
        }

        /// <summary>
        /// Devuelve el texto de ese identificador en cada idioma disponible. Sirve para reconocer
        /// líneas que se escribieron en un idioma distinto del actual.
        /// </summary>
        public static IEnumerable<string> GetTextInAllLanguages(TextId id)
        {
            foreach (Language language in _languages)
            {
                string text;
                if (language.Texts.TryGetValue(id, out text)) yield return text;
            }
        }

        // Busca por reflexión todas las clases que heredan de Language y crea una instancia de cada
        // una. Si algo falla, el plugin no debe caerse: se queda al menos con el inglés.
        private static List<Language> DiscoverLanguages()
        {
            try
            {
                return GetLoadableTypes(typeof(Language).Assembly)
                    .Where(type => type.IsSubclassOf(typeof(Language)) && !type.IsAbstract)
                    .Select(type => (Language)Activator.CreateInstance(type))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<Language> { new English() };
            }
        }

        // Devuelve los tipos del ensamblado que se han podido cargar. Si alguno no se puede
        // cargar (por una dependencia que no esté disponible), se ignora en vez de fallar.
        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(type => type != null);
            }
        }

        // Devuelve el idioma con ese nombre, o null si no existe.
        private static Language FindLanguage(string languageName)
        {
            return _languages.FirstOrDefault(language =>
                string.Equals(language.Name, languageName, StringComparison.OrdinalIgnoreCase));
        }
    }
}