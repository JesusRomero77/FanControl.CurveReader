using System.Collections.Generic;

namespace FanControl.CurveReader
{
    public static class ConfigMatcher
    {
        private static readonly List<FilteredSensor> _matchedSensors = new List<FilteredSensor>();
        private static readonly List<FilteredControl> _matchedControls = new List<FilteredControl>();

        public static void SetMatchObjects()
        {
            SetMatchSensors();
            SetMatchControls();
        }

        // Empareja cada sensor del JSON con su sensor del RPC (mismo Name e Identifier) y, por cada
        // pareja encontrada, crea un FilteredSensor con los datos del JSON.
        private static void SetMatchSensors()
        {
            _matchedSensors.Clear();

            List<JSONConfigReader.Sensor> jsonSensors = JSONConfigReader.GetJSONSensors();

            List<RPCReader.RPCSensor> rpcSensors = RPCReader.GetRPCSensors();

            foreach (JSONConfigReader.Sensor jsonSensor in jsonSensors)
            {
                if (string.IsNullOrWhiteSpace(jsonSensor.Name) || string.IsNullOrWhiteSpace(jsonSensor.Identifier)) continue;
                foreach (RPCReader.RPCSensor rpcSensor in rpcSensors)
                {
                    if (string.IsNullOrWhiteSpace(rpcSensor.Name) || string.IsNullOrWhiteSpace(rpcSensor.Identifier)) continue;
                    if (jsonSensor.Name == rpcSensor.Name && jsonSensor.Identifier == rpcSensor.Identifier)
                    {
                        _matchedSensors.Add(new FilteredSensor(jsonSensor.Identifier, jsonSensor.Name, jsonSensor.DataSource));
                        break;
                    }
                }
            }
        }

        // Empareja cada control del JSON con su control del RPC (mismo Identifier) y, por cada
        // pareja encontrada, crea un FilteredControl con los datos del JSON (el Name es el NickName).
        private static void SetMatchControls()
        {
            _matchedControls.Clear();
            List<JSONConfigReader.Control> jsonControls = JSONConfigReader.GetJSONControls();
            List<RPCReader.RPCControl> rpcControls = RPCReader.GetRPCControls();
            foreach (JSONConfigReader.Control jsonControl in jsonControls)
                foreach (RPCReader.RPCControl rpcControl in rpcControls)
                    if (jsonControl.Identifier == rpcControl.Identifier)
                    {
                        _matchedControls.Add(new FilteredControl(jsonControl.Identifier, jsonControl.Name, jsonControl.DataSource));
                        break;
                    }
        }

        public static List<FilteredSensor> GetMatchedSensors()
        {
            return _matchedSensors;
        }
        public static List<FilteredControl> GetMatchedControls()
        {
            return _matchedControls;
        }
    }
}