using FanControl.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace FanControl.CurveReader
{
    public class CurveReaderPlugin : IPlugin2
    {
        public string Name => "Curve Reader";
        private static bool _isInitializing = false;
        private static bool _isInitialized = false;
        private static readonly object _initLock = new object();
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
            // Evita que dos InitialStartup se ejecuten a la vez y se pisen las listas estáticas.
            lock (_initLock)
            {
                if (_isInitializing) return;
                _isInitializing = true;
            }
            try
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

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                LogFileManager.WriteLog("ERROR en InitialStartup:\r\n" + ex + "\r\n");
            }
            finally
            {
                _isInitializing = false;
            }

        }
        public void Update()
        {
            if (_isInitializing) return;
            if (!_isInitialized) return;
            try
            {
                if (JSONConfigReader.GetCurrentConfigInfo().FileName != JSONConfigReader.GetFileName() ||
                    JSONConfigReader.GetCurrentConfigInfo().FilePath != JSONConfigReader.GetFilePath())
                {
                    Thread thread = new Thread(InitialStartup) { IsBackground = true };
                    thread.Start();
                }
                RPCReader.UpdateValues();
            }
            catch (Exception ex)
            {
                LogFileManager.WriteLog("ERROR en Update:\r\n" + ex + "\r\n");
            }
        }

        public void Close()
        {
            LogFileManager.WriteLog(LogFileManager.GetEndOfLogMarker());
        }

    }
}
