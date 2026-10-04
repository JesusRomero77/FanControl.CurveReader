using System;

namespace FanControl.CurveReader
{
    /// <summary>
    /// Control de FanControl con filtrado de registro. Guarda su configuración (Filter) y el
    /// estado necesario para decidir, en cada lectura, si el valor debe escribirse en el log.
    /// </summary>
    public class FilteredControl : Control
    {
        // Último valor escrito en el log, para medir cuánto ha cambiado desde entonces.
        private byte _lastLoggedValue;

        // Segundos transcurridos desde el último registro (el intervalo se cuenta desde ahí).
        private int _secondsSinceLastLog;

        // Indica si el control ya se ha escrito alguna vez; la primera lectura siempre se escribe.
        private bool _hasBeenLogged;

        // Configuración de filtrado de este control (leída del XML por LoggerConfigReader).
        private readonly Filter _filter;

        // Al crearse, pide su filtro a LoggerConfigReader: el filtro del XML si existe,
        // o uno con valores por defecto si no.
        public FilteredControl(string identifier, string name, string dataSource)
            : base(identifier, name, dataSource)
        {
            _filter = LoggerConfigReader.GetControlFilter(identifier);
        }

        // Actualiza el valor con la nueva lectura y devuelve true si debe escribirse en el log.
        // Si devuelve true, el control ya queda marcado como registrado (último valor,
        // contador de segundos a cero), así que quien lo llame solo tiene que escribir la línea.
        public bool Update(byte newValue)
        {
            Value = newValue;
            _secondsSinceLastLog++;

            if (!MustLog()) return false;

            _lastLoggedValue = Value;
            _hasBeenLogged = true;
            _secondsSinceLastLog = 0;
            return true;
        }

        // Decide si la lectura actual debe escribirse, con la misma lógica que el MustLog actual:
        // - Si no está incluido en el log, nunca.
        // - La primera lectura siempre.
        // - Tras escribir, descansa IntervalSeconds segundos.
        // - Pasado ese tiempo, escribe si el valor difiere del último escrito en Threshold o más.
        private bool MustLog()
        {
            if (!_filter.IncludedInLog) return false;
            if (!_hasBeenLogged) return true;
            if (_secondsSinceLastLog < _filter.IntervalSeconds) return false;
            return Math.Abs(Value - _lastLoggedValue) >= _filter.Threshold;
        }
    }
}