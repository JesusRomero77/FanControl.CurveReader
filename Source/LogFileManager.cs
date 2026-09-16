

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FanControl.CurveReader
{
    internal class LogFileManager
    {
        private const string EndOfLogMarker = "============ FIN DEL LOG ============";
        private static readonly FileInfo _logFile = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FanControl", "CurveReader.log"));
        private static readonly List<string> _profileSummaries = new List<string>();
        public static void AddProfileSummary(string summary)
        {
            _profileSummaries.Add(summary);
        }
        public static void LoadExistingProfileSummaries()
        {
            var keptLines = new List<string>();
            var currentSummary = new StringBuilder();
            int equalsCount = 0;
            bool markerFound = false;

            if (!_logFile.Exists) return;
            string[] lines = File.ReadAllLines(_logFile.FullName);

            // Recorremos el archivo línea a línea, descartando la marca de fin de log
            // y acumulando cada bloque de resumen de perfil (desde la primera línea "="
            // hasta la tercera) para añadirlo a la lista mediante AddProfileSummary.
            foreach (string line in lines)
            {
                if (line.Trim() == EndOfLogMarker)
                {
                    markerFound = true;
                    continue;
                }

                if (equalsCount > 0)
                {
                    currentSummary.Append(line).Append(Environment.NewLine);

                    if (line.StartsWith("="))
                    {
                        equalsCount++;
                        if (equalsCount == 3)
                        {
                            AddProfileSummary(currentSummary.ToString());
                            currentSummary.Clear();
                            equalsCount = 0;
                        }
                    }
                }
                else if (line.StartsWith("="))
                {
                    equalsCount = 1;
                    currentSummary.Append(line).Append(Environment.NewLine);
                }

                keptLines.Add(line);
            }

            // Solo reescribimos el archivo si se encontró y descartó la marca de fin de log.
            if (markerFound)
            {
                using (StreamWriter writer = new StreamWriter(_logFile.FullName, false))
                {
                    foreach (string line in keptLines) writer.WriteLine(line);
                }
            }
        }
        public static string GetEndOfLogMarker()
        {
            return EndOfLogMarker;
        }
        private static void DeleteFirstProfileSummary(string[] lines, ref long currentSize)
        {
            if (_profileSummaries.Count == 0) return;

            // El resumen guardado en memoria conserva los saltos de línea tal cual se
            // escribieron en el archivo; lo partimos en líneas para poder compararlas
            // una a una contra las líneas del archivo y borrar exactamente ese bloque.
            List<string> summaryLines = new List<string>(
                _profileSummaries[0].Split(new[] { Environment.NewLine }, StringSplitOptions.None));
            if (summaryLines.Count > 0 && summaryLines[summaryLines.Count - 1] == string.Empty)
                summaryLines.RemoveAt(summaryLines.Count - 1);

            int summaryIndex = 0;
            int lastMatchedLine = -1;

            for (int i = 0; i < lines.Length && summaryIndex < summaryLines.Count; i++)
            {
                if (lines[i] == null || lines[i] != summaryLines[summaryIndex]) continue;

                long lineSize = Encoding.UTF8.GetByteCount(lines[i] + Environment.NewLine);
                lines[i] = null;
                currentSize -= lineSize;
                lastMatchedLine = i;
                summaryIndex++;
            }

            // Limpiamos líneas en blanco sueltas que hayan quedado justo después del
            // bloque borrado, y también al principio del archivo.
            if (lastMatchedLine >= 0)
            {
                for (int i = lastMatchedLine + 1; i < lines.Length; i++)
                {
                    if (lines[i] == null) continue;
                    if (!string.IsNullOrWhiteSpace(lines[i])) break;

                    long lineSize = Encoding.UTF8.GetByteCount(lines[i] + Environment.NewLine);
                    lines[i] = null;
                    currentSize -= lineSize;
                }
            }

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == null) continue;
                if (!string.IsNullOrWhiteSpace(lines[i])) break;

                long lineSize = Encoding.UTF8.GetByteCount(lines[i] + Environment.NewLine);
                lines[i] = null;
                currentSize -= lineSize;
            }

            _profileSummaries.RemoveAt(0);
        }
        public static void WriteLog(string text)
        {
            const long maxLogSize = 100L * 1024L * 1024L; // 100MB

            Directory.CreateDirectory(_logFile.DirectoryName);

            if (!_logFile.Exists)
            {
                File.WriteAllText(_logFile.FullName, text);
                return;
            }

            long newTextSize = System.Text.Encoding.UTF8.GetByteCount(text);
            _logFile.Refresh();

            if (_logFile.Length + newTextSize <= maxLogSize)
            {
                File.AppendAllText(_logFile.FullName, text);
                return;
            }

            string[] lines = File.ReadAllLines(_logFile.FullName);

            long currentSize = _logFile.Length;

            // Eliminamos registros antiguos (en memoria, marcando líneas como null)
            // hasta liberar suficiente espacio; la reescritura del archivo se hace
            // una sola vez, después del bucle, no en cada iteración.
            while (currentSize + newTextSize > maxLogSize * 0.8)
            {
                bool removedLine = false;

                for (int i = 0; i < lines.Length; i++) if (lines[i] != null && lines[i].StartsWith("["))
                {
                    long lineSize = System.Text.Encoding.UTF8.GetByteCount(lines[i] + Environment.NewLine);
                    lines[i] = null;
                    currentSize -= lineSize;
                    removedLine = true;
                    break;
                }

                if (removedLine)
                {
                    int firstContentLine = -1;

                    for (int i = 0; i < lines.Length; i++) if (lines[i] != null && !string.IsNullOrWhiteSpace(lines[i]))
                    {
                        firstContentLine = i;
                        break;
                    }

                    if (firstContentLine >= 0)
                    {

                        if (_profileSummaries.Count == 0) continue; // o el control de flujo que corresponda si no hay resumen

                        string[] summaryLines = _profileSummaries[0].Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                        int summaryLineCount = summaryLines.Length;
                        if (summaryLineCount > 0 && summaryLines[summaryLineCount - 1] == string.Empty) summaryLineCount--;

                        int lastSummaryLine  = firstContentLine + summaryLineCount - 1;

                        if (lastSummaryLine >= 0)
                        {
                            int nextContentLine = -1;

                            for (int i = lastSummaryLine + 1; i < lines.Length; i++) if (lines[i] != null && !string.IsNullOrWhiteSpace(lines[i]))
                            {
                                nextContentLine = i;
                                break;
                            }

                            if (nextContentLine >= 0 && !lines[nextContentLine].StartsWith("[")) DeleteFirstProfileSummary(lines, ref currentSize);
                        }
                    }

                    bool remainingRecords = false;

                    for (int i = 0; i < lines.Length; i++) if (lines[i] != null && lines[i].StartsWith("["))
                    {
                        remainingRecords = true;
                        break;
                    }
                    if (!remainingRecords) break;
                }
            }

            // Reconstruimos el archivo una sola vez, ya con todo el espacio liberado,
            // y añadimos el nuevo texto al final.
            using (StreamWriter writer = new StreamWriter(_logFile.FullName, false))
            {
                foreach (string line in lines) if (line != null) writer.WriteLine(line);
                writer.Write(text);
            }
        }
    }
}
