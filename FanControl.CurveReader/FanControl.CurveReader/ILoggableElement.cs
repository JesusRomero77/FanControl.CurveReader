namespace FanControl.CurveReader
{
    /// <summary>
    /// Contrato común de los elementos con informe de registro (FilteredSensor y FilteredControl).
    /// Permite que RPCReader decida si registrar un sensor o un control con el mismo código.
    /// </summary>
    public interface ILoggableElement
    {
        string Identifier { get; }
        string Name { get; }
        byte Value { get; set; }
        Filter Filter { get; }
        byte LastLoggedValue { get; set; }
        int SecondsSinceLastLog { get; set; }
        bool HasBeenLogged { get; set; }
    }
}