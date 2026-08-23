using System.Collections.Generic;

namespace FanControl.CurveReader
{
    public static class ConfigMatcher
    {
        private static readonly List<RPCReader.RPCSensor> _matchedSensors = new List<RPCReader.RPCSensor>();
        private static readonly List<RPCReader.RPCControl> _matchedControls = new List<RPCReader.RPCControl>();

        public static void SetMatchObjects()
        {
            SetMatchSensors();
            SetMatchControls();
        }
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
                        _matchedSensors.Add(rpcSensor);
                        break;
                    }
                }
            }
        }
        private static void SetMatchControls()
        {
            _matchedControls.Clear();
            List<JSONConfigReader.Control> jsonControls = JSONConfigReader.GetJSONControls();
            List<RPCReader.RPCControl> rpcControls = RPCReader.GetRPCControls();
            foreach (JSONConfigReader.Control jsonControl in jsonControls)
                foreach (RPCReader.RPCControl rpcControl in rpcControls)
                    if (jsonControl.Identifier == rpcControl.Identifier)
                    {
                        rpcControl.RPCName = rpcControl.Name;
                        rpcControl.Name = jsonControl.Name;
                        _matchedControls.Add(rpcControl);
                        break;
                    }
        }

        public static List<RPCReader.RPCSensor> GetMatchedSensors()
        {
            return _matchedSensors;
        }
        public static List<RPCReader.RPCControl> GetMatchedControls()
        {
            return _matchedControls;
        }
    }
}