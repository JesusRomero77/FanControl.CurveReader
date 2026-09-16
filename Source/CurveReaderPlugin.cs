using FanControl.Plugins;
using System;
using System.IO;
using System.Threading;

namespace FanControl.CurveReader
{
    public class CurveReaderPlugin : IPlugin2
    {
        public string Name => "Curve Reader";

        public void Initialize()
        {
        }
        public void Load(IPluginSensorsContainer container)
        {
            // 1. Crear el sensor para que Fan Control acepte el plugin
            container.TempSensors.Add(new LoggerSensor());

            // 2. LogFileManager: carga los resúmenes de perfil ya existentes en el log
            LogFileManager.LoadExistingProfileSummaries();

            // 3. Esperar a que Fan Control y RPC terminen de arrancar e iniciar el arranque del plugin en segundo plano
            Thread thread = new Thread(() => { Thread.Sleep(2000); InitialStartup(); }) { IsBackground = true };
            thread.Start();
        }
        private void InitialStartup()
        {
            // Ahora empieza realmente el trabajo del plugin...
            // 4. JSONConfigReader: establece y prepara los objetos de la configuración JSON:
            JSONConfigReader.SetJSONObjects();
            // 5. RPCReader: establece y prepara los objetos obtenidos de RPC:
            RPCReader.SetRPCObjects();
            // 6. ConfigMatcher: establece las correspondencias entre los sensores y controles de JSON y RPC:
            ConfigMatcher.SetMatchObjects();
            // 7. RPCReader: sustituye los objetos obtenidos inicialmente por RPC
            //    por aquellos que han hecho correspondencia con la configuración JSON activa.
            RPCReader.SetMatchedObjects();
        }
        public void Update()
        {
            var configInfo = JSONConfigReader.GetCurrentConfigInfo();
            if (configInfo.FileName != JSONConfigReader.GetFileName() || configInfo.FilePath != JSONConfigReader.GetFilePath())
            {
                Thread thread = new Thread(InitialStartup) { IsBackground = true };
                thread.Start();
            }
            RPCReader.UpdateValues();
        }
        public void Close()
        {
            LogFileManager.WriteLog(LogFileManager.GetEndOfLogMarker());
        }

    }
}