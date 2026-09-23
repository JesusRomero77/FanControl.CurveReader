using System.Collections.Generic;

namespace CurveReaderConfigurator
{
    public enum AppLanguage { English, Spanish }

    /// <summary>
    /// Un identificador por cada texto de la interfaz. Solo contiene el nombre del texto;
    /// el texto en sí está en los diccionarios de cada idioma.
    /// </summary>
    public enum TextId
    {
        File, Save, Exit,
        Settings, Language, FanControlPath,
        Help, ViewHelp, About,
        Profile, RefreshProfiles,
        Controls, Curves, Sensors,
        Log, Interval, ChangeThreshold,
        CurvesNotRecorded,
        AboutWindowTitle, AboutDescription, ViewOnGitHub, AboutJsonNotice, Version, Ok,
        HelpWindowTitle,HelpRequirementsTitle, HelpRequirementsText,HelpGettingStartedTitle, HelpGettingStartedText,HelpSelectProfileTitle, HelpSelectProfileText,HelpConfigureItemsTitle, HelpConfigureItemsText,HelpSaveTitle, HelpSaveText

    }

    /// <summary>
    /// Devuelve los textos de la interfaz en el idioma actual. Cada idioma tiene su diccionario
    /// y los textos se emparejan por identificador, no por posición.
    /// </summary>
    public static class Translator
    {
        /// <summary>Idioma actual de la interfaz. De momento arranca en español.</summary>
        public static AppLanguage CurrentLanguage { get; private set; } = AppLanguage.English;

        private static readonly Dictionary<TextId, string> s_englishTexts = new Dictionary<TextId, string>
        {
            { TextId.File, "File" },
            { TextId.Save, "Save" },
            { TextId.Exit, "Exit" },
            { TextId.Settings, "Settings" },
            { TextId.Language, "Language" },
            { TextId.FanControlPath, "Specify FanControl path" },
            { TextId.Help, "Help" },
            { TextId.ViewHelp, "View Help" },
            { TextId.About, "About..." },
            { TextId.Profile, "Profile:" },
            { TextId.RefreshProfiles, "Reload profiles" },
            { TextId.Controls, "Controls" },
            { TextId.Curves, "Curves" },
            { TextId.Sensors, "Sensors" },
            { TextId.Interval, "Interval (sec)" },
            { TextId.ChangeThreshold, "Minimum change" },
            { TextId.CurvesNotRecorded, "Curves are not recorded in the log yet." },
            { TextId.Log, "Log" },
            { TextId.AboutWindowTitle, "About CurveReaderConfigurator" },
            { TextId.AboutDescription, "CurveReaderConfigurator lets you configure the CurveReader logging plugin for FanControl:" +
                " choose which sensors and controls are logged, how often, and how big a change must be before it gets logged.\r\n\r\n" +
                "This application is portable: it can be run from anywhere on disk without installing it." },
            { TextId.ViewOnGitHub, "View project on GitHub" },
            { TextId.AboutJsonNotice, "Includes Newtonsoft.Json (Copyright \u00A9 James Newton-King), used under the MIT License." },
            { TextId.Version, "Version" },
            { TextId.Ok, "OK" },
            { TextId.HelpWindowTitle, "CurveReaderConfigurator Help" },
            { TextId.HelpRequirementsTitle, "Requirements" },
            { TextId.HelpRequirementsText, "Windows with .NET Framework 4.8 installed, and FanControl already set up with the profiles you want to log." },
            { TextId.HelpGettingStartedTitle, "Getting started" },
            { TextId.HelpGettingStartedText, "The first time you run the app, go to Settings > Specify FanControl " +
                "path and select FanControl.exe (or a shortcut to it). The app will locate your profiles automatically from then on." },
            { TextId.HelpSelectProfileTitle, "Selecting a profile" },
            { TextId.HelpSelectProfileText, "Choose the profile you want to configure from the Profile dropdown. " +
                "Its controls, curves and sensors will appear below, ready to edit." },
            { TextId.HelpConfigureItemsTitle, "Configuring sensors and controls" },
            { TextId.HelpConfigureItemsText, "For each item, the log checkbox decides whether it is logged at all. " +
                "\"Interval (sec)\" is the minimum time between two log entries for that item. " +
                "\"Minimum change\" (1 to 10) sets how much the value must change before it gets logged again." },
            { TextId.HelpSaveTitle, "Saving your changes" },
            { TextId.HelpSaveText, "Changes are kept in memory as you switch between profiles, but they are only written " +
                "to disk when you choose File > Save. The CurveReader plugin reads that file the next time it starts." }
        };

        private static readonly Dictionary<TextId, string> s_spanishTexts = new Dictionary<TextId, string>
        {
            { TextId.File, "Archivo" },
            { TextId.Save, "Guardar" },
            { TextId.Exit, "Salir" },
            { TextId.Settings, "Configuración" },
            { TextId.Language, "Idioma" },
            { TextId.FanControlPath, "Especificar ruta de FanControl" },
            { TextId.Help, "Ayuda" },
            { TextId.ViewHelp, "Ver la ayuda" },
            { TextId.About, "Acerca de..." },
            { TextId.Profile, "Perfil:" },
            { TextId.RefreshProfiles, "Recargar perfiles" },
            { TextId.Controls, "Controles" },
            { TextId.Curves, "Curvas" },
            { TextId.Sensors, "Sensores" },
            { TextId.Interval, "Intervalo (seg)" },
            { TextId.ChangeThreshold, "Umbral de cambio" },
            { TextId.CurvesNotRecorded, "Las curvas aún no se registran en el log." },
            { TextId.Log, "Registro" },
            { TextId.AboutWindowTitle, "Acerca de CurveReaderConfigurator" },
            { TextId.AboutDescription, "CurveReaderConfigurator permite configurar el plugin de registro CurveReader para FanControl: " +
                "elegir qué sensores y controles se registran, con qué frecuencia y qué magnitud de cambio debe producirse antes de registrarlo.\r\n\r\n" +
                "Este programa es portable: se puede ejecutar desde cualquier carpeta del disco sin necesidad de instalarlo." },
            { TextId.ViewOnGitHub, "Ver proyecto en GitHub" },
            { TextId.AboutJsonNotice, "Incluye Newtonsoft.Json (Copyright \u00A9 James Newton-King), bajo licencia MIT." },
            { TextId.Version, "Versión" },
            { TextId.Ok, "Aceptar" },
            { TextId.HelpWindowTitle, "Ayuda de CurveReaderConfigurator" },
            { TextId.HelpRequirementsTitle, "Requisitos" },
            { TextId.HelpRequirementsText, "Windows con .NET Framework 4.8 instalado, y FanControl ya configurado con los perfiles que quieras registrar." },
            { TextId.HelpGettingStartedTitle, "Primeros pasos" },
            { TextId.HelpGettingStartedText, "La primera vez que ejecutes la app, ve a Configuración > Especificar ruta de " +
                "FanControl y selecciona FanControl.exe (o un acceso directo). A partir de ahí la app localizará tus perfiles automáticamente." },
            { TextId.HelpSelectProfileTitle, "Elegir un perfil" },
            { TextId.HelpSelectProfileText, "Elige el perfil que quieras configurar en el desplegable Perfil. " +
                "Sus controles, curvas y sensores aparecerán debajo, listos para editar." },
            { TextId.HelpConfigureItemsTitle, "Configurar sensores y controles" },
            { TextId.HelpConfigureItemsText, "Para cada elemento, la casilla Registro decide si se registra o no. " +
                "\"Intervalo (seg)\" es el tiempo mínimo entre dos registros de ese elemento. " +
                "\"Umbral de cambio\" (1 a 10) indica cuánto debe cambiar el valor antes de volver a registrarse." },
            { TextId.HelpSaveTitle, "Guardar los cambios" },
            { TextId.HelpSaveText, "Los cambios se conservan en memoria al cambiar de perfil, " +
                "pero solo se escriben en disco al elegir Archivo > Guardar. El plugin CurveReader lee ese archivo la próxima vez que se inicie." }
        };

        public static void SetLanguage(AppLanguage language)
        {
            CurrentLanguage = language;
        }

        /// <summary>
        /// Devuelve el texto en el idioma actual. Si a ese idioma le falta el texto, devuelve el
        /// inglés, y si tampoco está, el nombre del identificador, para que el fallo se vea.
        /// </summary>
        public static string Translate(TextId id)
        {
            Dictionary<TextId, string> texts = CurrentLanguage == AppLanguage.Spanish ? s_spanishTexts : s_englishTexts;

            string text;
            if (texts.TryGetValue(id, out text)) return text;
            if (s_englishTexts.TryGetValue(id, out text)) return text;
            return id.ToString();
        }
    }
}