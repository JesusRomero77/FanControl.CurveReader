namespace FanControl.CurveReader
{
    /// <summary>
    /// Configuración de filtrado de un elemento de FanControl. Es inmutable: se crea una sola vez
    /// con todos sus valores (normalmente por LoggerConfigReader, a partir del XML) y no cambia.
    /// Si solo se indica el Identifier, el resto toma los valores por defecto.
    /// Solo guarda datos; la decisión de registrar la toma la clase filtrada que lo posee.
    /// </summary>
    public class Filter
    {
        // Identifier del elemento al que se aplica (la misma clave que usa el XML de configuración).
        public string Identifier { get; }

        // Indica si el control o sensor se registra en el log.
        public bool IncludedInLog{ get; }

        // Segundos que el elemento deja de vigilarse tras un registro (1-3600).
        public int IntervalSeconds { get; }

        // Diferencia mínima (>=) respecto al último valor escrito para registrar (1-10).
        public int Threshold { get; }

        // Valores por defecto para un elemento concreto: registrándolo, comprobando
        // cada segundo y con umbral 1 (cualquier cambio).
        public Filter(string identifier)
        {
            Identifier = identifier;
            IncludedInLog = true;
            IntervalSeconds = 1;
            Threshold = 1;
        }
        public Filter(string identifier, bool includedInLog, int intervalSeconds, int threshold)
        {
            Identifier = identifier;
            IncludedInLog = includedInLog;
            IntervalSeconds = intervalSeconds;
            Threshold = threshold;
        }

    }
}