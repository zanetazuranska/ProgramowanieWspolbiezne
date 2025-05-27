using System;
using System.Text;

namespace TP.ConcurrentProgramming.Data
{
    public class DiagnosticData
    {
        public DateTime Timestamp { get; set; }
        public int BallId { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
        public double VelocityX { get; set; }
        public double VelocityY { get; set; }

        public string ToCsv()
            => $"{Timestamp:O};{BallId};{PositionX};{PositionY};{VelocityX};{VelocityY}";

        public static DiagnosticData ParseCsv(string csvLine)
        {
            var parts = csvLine.Split(';');
            if (parts.Length != 6)
                throw new FormatException("Invalid CSV format for DiagnosticData.");

            return new DiagnosticData
            {
                Timestamp = DateTime.Parse(parts[0], null, System.Globalization.DateTimeStyles.RoundtripKind),
                BallId = int.Parse(parts[1]),
                PositionX = float.Parse(parts[2]),
                PositionY = float.Parse(parts[3]),
                VelocityX = float.Parse(parts[4]),
                VelocityY = float.Parse(parts[5])
            };
        }
    }
}