namespace FanControl.CurveReader
{
    /// <summary>
    /// Sensor de FanControl: además del Identifier y el Name que hereda, guarda el sensor físico
    /// del que toma su temperatura (DataSource) y su valor actual (Value).
    /// </summary>
    public class Sensor : FanControlElement
    {
        // Identifier del sensor físico del que este sensor toma su valor
        // (SelectedTempSource.Identifier en el JSON de FanControl).
        public string DataSource { get; }

        // Valor actual del sensor, redondeado; se actualiza en cada lectura de FanControl.
        public byte Value { get; internal set; }

        public Sensor(string identifier, string name, string dataSource) : base(identifier, name)
        {
            DataSource = dataSource;
        }
    }
}