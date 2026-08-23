using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FanControl.Plugins;

namespace FanControl.CurveReader
{
    internal class LoggerSensor : IPluginSensor
    {
        public string Id => "Logger";

        public string Name => "Logger";

        public float? Value => 0;

        public void Update()
        {
        }
    }

}
