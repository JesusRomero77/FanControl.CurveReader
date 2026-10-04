namespace FanControl.CurveReader
{
    /// <summary>
    /// Control de FanControl: además del Identifier y el Name que hereda, guarda la curva que
    /// tiene asignada (DataSource) y su valor actual (Value).
    /// </summary>
    public class Control : FanControlElement
    {
        // Nombre de la curva asignada al control (SelectedFanCurve.Name en el JSON de FanControl).
        public string DataSource { get; }

        // Valor actual del control (PWM), redondeado; se actualiza en cada lectura de FanControl.
        public byte Value { get; internal set; }

        public Control(string identifier, string name, string dataSource) : base(identifier, name)
        {
            DataSource = dataSource;
        }
    }
}