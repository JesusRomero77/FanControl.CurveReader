using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace FanControl.CurveReader
{
    /// <summary>
    /// Base de todos los idiomas del log del plugin. Cada idioma es una clase que hereda de esta y
    /// contiene únicamente su nombre y su diccionario. El plugin descubre solo las clases que heredan
    /// de Language, así que para añadir un idioma basta con añadir su clase en este archivo
    /// (y en Language.cs del configurador, con el mismo Name).
    /// </summary>
    public abstract class Language
    {
        /// <summary>Nombre del idioma escrito en ese mismo idioma. Debe coincidir con el del configurador.</summary>
        public abstract string Name { get; }

        /// <summary>Un texto por cada TextId. Si falta alguno, se usará el del inglés.</summary>
        public abstract Dictionary<TextId, string> Texts { get; }
    }

    /// <summary>Idioma por defecto y de reserva: debe tener todos los textos.</summary>
    public class English : Language
    {
        public override string Name => "English";

        public override Dictionary<TextId, string> Texts { get; } = new Dictionary<TextId, string>
        {
            { TextId.EndOfLog, "END OF LOG" },
            { TextId.ActiveProfile, "ACTIVE PROFILE" },
            { TextId.ProfileNameLabel, "Name:" },
            { TextId.ProfilePathLabel, "Path:" },
            { TextId.ControlsHeading, "CONTROLS:" },
            { TextId.CurvesHeading, "CURVES:" },
            { TextId.SensorsHeading, "SENSORS:" },
            { TextId.ControlType, "CONTROL" },
            { TextId.SensorType, "SENSOR" },
            { TextId.RecordNameLabel, "Name" },
            { TextId.RecordValueLabel, "Value" },
            { TextId.UnknownOrigin, "Unknown origin" }
        };
    }

    public class Spanish : Language
    {
        public override string Name => "Español";

        public override Dictionary<TextId, string> Texts { get; } = new Dictionary<TextId, string>
        {
            { TextId.EndOfLog, "FIN DEL LOG" },
            { TextId.ActiveProfile, "PERFIL ACTIVO" },
            { TextId.ProfileNameLabel, "Nombre:" },
            { TextId.ProfilePathLabel, "Ruta:" },
            { TextId.ControlsHeading, "CONTROLES:" },
            { TextId.CurvesHeading, "CURVAS:" },
            { TextId.SensorsHeading, "SENSORES:" },
            { TextId.ControlType, "CONTROL" },
            { TextId.SensorType, "SENSOR" },
            { TextId.RecordNameLabel, "Nombre" },
            { TextId.RecordValueLabel, "Valor" },
            { TextId.UnknownOrigin, "Origen desconocido" }
        };
    }
}