namespace FanControl.CurveReader
{
    /// <summary>
    /// Control de FanControl con su informe de registro: su configuración de filtrado (Filter) y el
    /// estado necesario para saber si toca registrarlo. No decide nada: quien decide es RPCReader.
    /// </summary>
    public class FilteredControl : Control, ILoggableElement
    {
        // Configuración de filtrado de este control (leída del XML por LoggerConfigReader).
        public Filter Filter { get; }

        // Último valor escrito en el log, para medir cuánto ha cambiado desde entonces.
        public byte LastLoggedValue { get; set; }

        // Segundos transcurridos desde el último registro (el intervalo se cuenta desde ahí).
        public int SecondsSinceLastLog { get; set; }

        // Indica si el control ya se ha escrito alguna vez; la primera lectura siempre se escribe.
        public bool HasBeenLogged { get; set; }

        // Al crearse, pide su filtro a LoggerConfigReader: el filtro del XML si existe,
        // o uno con valores por defecto si no.
        public FilteredControl(string identifier, string name, string dataSource)
            : base(identifier, name, dataSource)
        {
            Filter = LoggerConfigReader.GetControlFilter(identifier);
        }
    }
}