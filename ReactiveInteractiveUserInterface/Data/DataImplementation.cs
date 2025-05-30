﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor
        private DiagnosticsLogger _logger;

        public DataImplementation()
        {
            _logger = new DiagnosticsLogger("diagnostics.txt");
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler, double diameter)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            Random random = new Random();
            double minSpacing = diameter * 2; // Slightly more than the diameter to ensure spacing

            for (int i = 0; i < numberOfBalls; i++)
            {
                Vector startingPosition;
                bool isValidPosition;

                // Generate random positions until a valid one is found
                do
                {
                    isValidPosition = true;
                    startingPosition = new Vector(random.Next(100, 400 - 100), random.Next(100, 400 - 100));

                    // Check if the new ball overlaps with any existing balls
                    foreach (var existingBall in BallsList)
                    {
                        double dx = existingBall.Position.x - startingPosition.x;
                        double dy = existingBall.Position.y - startingPosition.y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance < minSpacing)
                        {
                            isValidPosition = false;
                            break;
                        }
                    }
                } while (!isValidPosition);

                // Generate random velocity for the ball
                Vector velocity = new((random.NextDouble() - 0.5d) * 150, (random.NextDouble() - 0.5d) * 150);

                // Create and initialize the ball
                Ball newBall = new(startingPosition, velocity, diameter);
                upperLayerHandler(startingPosition, newBall);
                BallsList.Add(newBall);

                // Start a task for each ball to move continuously
                CancellationTokenSource cts = new();
                CancellationTokens.Add(cts); // Store token for later cancellation
                StartBallMovement(newBall, cts.Token);
            }
        }

        private void StartBallMovement(Ball ball, CancellationToken cancellationToken)
        {
            Task.Run(async () =>
            {
                var stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                long lastTick = stopwatch.ElapsedMilliseconds;

                while (!cancellationToken.IsCancellationRequested)
                {
                    long currentTick = stopwatch.ElapsedMilliseconds;
                    float deltaTime = (currentTick - lastTick) / 1000.0f; // czas w sekundach
                    lastTick = currentTick;

                    lock (BallsList)
                    {
                        // Uwzględnienie upływu czasu w przesunięciu piłki
                        ball.Move(new(ball.Velocity.x * deltaTime, ball.Velocity.y * deltaTime));
                    }

                    _logger.Log(new DiagnosticData
                    {
                        Timestamp = DateTime.UtcNow,
                        BallId = ball.Id,
                        PositionX = ball.Position.x,
                        PositionY = ball.Position.y,
                        VelocityX = ball.Velocity.x,
                        VelocityY = ball.Velocity.y,
                    });

                    // Kontrola częstotliwości aktualizacji (np. ~60 FPS)
                    await Task.Delay(16);
                }
            }, cancellationToken);
        }

        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            _logger.Dispose();

            if (!Disposed)
            {
                if (disposing)
                {
                    BallsList.Clear();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        //private bool disposedValue;
        private bool Disposed = false;

        //private readonly Timer MoveTimer;
        //private Random RandomGenerator = new();
        private List<IBall> BallsList = [];
        private List<CancellationTokenSource> CancellationTokens = new();

        #endregion private

        public override List<IBall> GetBallsList() // Publiczna metoda do pobrania listy kulek
        {
            return BallsList;
        }

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}