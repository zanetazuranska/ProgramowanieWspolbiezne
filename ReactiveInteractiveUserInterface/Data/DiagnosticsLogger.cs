using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data
{
    public class DiagnosticsLogger : IDisposable
    {
        private readonly string _filePath;
        private readonly ConcurrentQueue<DiagnosticData> _queue = new();
        private readonly CancellationTokenSource _cts = new();
        private readonly Task _worker;
        private readonly int _flushIntervalMs = 1000; // Próba zapisu co sekundę

        public DiagnosticsLogger(string filePath)
        {
            _filePath = filePath;
            _worker = Task.Run(LogWorker);
        }

        public void Log(DiagnosticData data)
        {
            _queue.Enqueue(data);
        }

        private async Task LogWorker()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    int count = _queue.Count;

                    if (count > 0)
                    {
                        StringBuilder batch = new();
                        while (_queue.TryDequeue(out var data))
                        {
                            batch.AppendLine(data.ToCsv());
                        }

                        // Próbujemy zapisać do pliku
                        try
                        {
                            // ASCII encoding
                            await File.AppendAllTextAsync(_filePath, batch.ToString(), Encoding.ASCII, _cts.Token);
                        }
                        catch (IOException)
                        {
                            // Brak przepustowości -> Odkładamy dane z powrotem do kolejki
                            foreach (var line in batch.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                            {
                                try
                                {
                                    var data = DiagnosticData.ParseCsv(line);
                                    _queue.Enqueue(data);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"[{DateTime.UtcNow:O}] Parse error: \"{line}\" - {ex.Message}");
                                }
                            }
                            await Task.Delay(_flushIntervalMs, _cts.Token);
                        }
                    }
                    else
                    {
                        await Task.Delay(_flushIntervalMs, _cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _worker.Wait();
        }
    }
}
