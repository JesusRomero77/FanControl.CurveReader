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

            // 2. Esperar a que Fan Control y RPC terminen de arrancar e iniciar el arranque del plugin en segundo plano
            Thread thread = new Thread(() => { Thread.Sleep(2000); InitialStartup(); }) { IsBackground = true };
            thread.Start();
        }
        private void InitialStartup()
        {
            // Ahora empieza realmente el trabajo del plugin...
            // 3. JSONConfigReader: establece y prepara los objetos de la configuración JSON:
            JSONConfigReader.SetJSONObjects();
            // 4. RPCReader: establece y prepara los objetos obtenidos de RPC:
            RPCReader.SetRPCObjects();
            // 5. ConfigMatcher: establece las correspondencias entre los sensores y controles de JSON y RPC:
            ConfigMatcher.SetMatchObjects();
            // 6. RPCReader: sustituye los objetos obtenidos inicialmente por RPC
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
            WriteLog("============ FIN DEL LOG ============");
        }

        public static void WriteLog(string text)
        {
            const long maxLogSize = 50L * 1024L; // 50 KB

            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"FanControl");

            Directory.CreateDirectory(folder);

            string logPath = Path.Combine(folder, "CurveReader.log");

            if (!File.Exists(logPath))
            {
                File.WriteAllText(logPath, text);
                return;
            }

            long newTextSize = System.Text.Encoding.UTF8.GetByteCount(text);
            FileInfo fileInfo = new FileInfo(logPath);

            // Si no se supera el límite, escribimos directamente.
            if (fileInfo.Length + newTextSize <= maxLogSize)
            {
                File.AppendAllText(logPath, text);
                return;
            }

            string[] lines = File.ReadAllLines(logPath);

            long currentSize = fileInfo.Length;

            // Eliminar registros antiguos hasta que haya espacio suficiente.
            while (currentSize + newTextSize > maxLogSize)
            {
                bool removedLine = false;

                // Buscamos desde el principio el registro continuo más antiguo.
                for (int i = 0; i < lines.Length; i++) if (lines[i] != null && lines[i].StartsWith("["))
                {
                    long lineSize = System.Text.Encoding.UTF8.GetByteCount(lines[i] + Environment.NewLine);
                    lines[i] = null;
                    currentSize -= lineSize;
                    removedLine = true;
                    break;
                }
                

                // Si hemos eliminado un registro, comprobamos si el bloque
                // de registros del resumen más antiguo ya se ha agotado.
                if (removedLine)
                {
                    int firstContentLine = -1;

                    // Primera línea que todavía existe.
                    for (int i = 0; i < lines.Length; i++) if (lines[i] != null && !string.IsNullOrWhiteSpace(lines[i]))
                    {
                        firstContentLine = i;
                        break;
                    }
                    

                    if (firstContentLine >= 0)
                    {
                        int equalsCount = 0;
                        int thirdEqualsLine = -1;

                        // Buscamos la tercera línea que empieza por "=",
                        // que marca el final del primer resumen.
                        for (int i = firstContentLine; i < lines.Length; i++) if (lines[i] != null && lines[i].StartsWith("="))
                        {
                            equalsCount++;
                            if (equalsCount == 3)
                            {
                                thirdEqualsLine = i;
                                break;
                            }
                        }

                        if (thirdEqualsLine >= 0)
                        {
                            int nextContentLine = -1;

                            // Buscamos qué hay después del primer resumen,
                            // ignorando líneas eliminadas y líneas vacías.
                            for (int i = thirdEqualsLine + 1; i < lines.Length; i++) if (lines[i] != null && !string.IsNullOrWhiteSpace(lines[i]))
                            {
                                nextContentLine = i;
                                break;
                            }
                            

                            // Si lo siguiente empieza por "[", todavía quedan
                            // registros pertenecientes al primer resumen.
                            bool firstSummaryFinished = nextContentLine >= 0 && !lines[nextContentLine].StartsWith("[");

                            if (firstSummaryFinished)
                            {
                                // Eliminamos el primer resumen completo,
                                // incluida su tercera línea "=".
                                for (int i = firstContentLine; i <= thirdEqualsLine; i++) if (lines[i] != null)
                                {
                                    long lineSize = System.Text.Encoding.UTF8.GetByteCount(lines[i] + Environment.NewLine);
                                    lines[i] = null;
                                    currentSize -= lineSize;
                                }

                                // Eliminamos las líneas vacías posteriores al resumen.
                                for (int i = thirdEqualsLine + 1; i < lines.Length; i++)
                                {
                                    if (lines[i] == null) continue;

                                    if (string.IsNullOrWhiteSpace(lines[i]))
                                    {
                                        long lineSize = System.Text.Encoding.UTF8.GetByteCount(lines[i] + Environment.NewLine);
                                        lines[i] = null;
                                        currentSize -= lineSize;
                                    }
                                    else break;
                                }
                                // Eliminamos cualquier línea vacía que haya quedado
                                // al principio del archivo, hasta encontrar contenido.
                                for (int i = 0; i < lines.Length; i++)
                                {
                                    if (lines[i] == null) continue;

                                    if (string.IsNullOrWhiteSpace(lines[i]))
                                    {
                                        long lineSize = System.Text.Encoding.UTF8.GetByteCount(lines[i] + Environment.NewLine);
                                        lines[i] = null;
                                        currentSize -= lineSize;
                                    }
                                    else break;
                                }
                            }
                        }
                    }

                // Si ya no quedan registros continuos, no podemos liberar
                // más espacio mediante este mecanismo.
                bool remainingRecords = false;

                for (int i = 0; i < lines.Length; i++) if (lines[i] != null && lines[i].StartsWith("["))
                {
                    remainingRecords = true;
                    break;
                }
                if (!remainingRecords) break;
                }
                
                // Reconstruimos el archivo conservando las líneas restantes.
                using (StreamWriter writer = new StreamWriter(logPath, false))
                {
                    foreach (string line in lines) if (line != null) writer.WriteLine(line);
                    writer.Write(text);
                }
            }
        }
    }
}