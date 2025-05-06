//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using TP.ConcurrentProgramming.Data.WindowData;
using System.Numerics;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler, double diameter, WindowData windowData)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBellow.Start(numberOfBalls, (startingPosition, databall) => upperLayerHandler(new Position(startingPosition.x, startingPosition.x), new Ball(databall)), diameter);

            BallsList = layerBellow.GetBallsList();
            // Initialize and start collision handling
            var collisionHandler = new CollisionHandler(BallsList, windowData);
            collisionHandler.StartCollisionDetection();

        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBellow;
        private List<TP.ConcurrentProgramming.Data.IBall> BallsList; // Assuming a list of balls is available

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }

    // ======================================================================
    // CollisionHandler class - encapsulates collision detection and resolution.
    // ======================================================================
    internal class CollisionHandler
    {
        private readonly List<TP.ConcurrentProgramming.Data.IBall> ballsList;
        private WindowData windowData;

        public CollisionHandler(List<TP.ConcurrentProgramming.Data.IBall> balls, WindowData windowData)
        {
            ballsList = balls;
            this.windowData = windowData;
        }

        public void StartCollisionDetection()
        {
            // Start handling collisions in a background thread
            Task.Run(async () => await HandleCollisions());
        }

        private async Task HandleCollisions(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                lock (ballsList)
                {
                    // Check for collisions with other balls
                    for (int i = 0; i < ballsList.Count; i++)
                    {
                        var ball = ballsList[i];
                        for (int j = i + 1; j < ballsList.Count; j++)
                        {
                            var otherBall = ballsList[j];
                            if (CheckCollision(ball, otherBall))
                            {
                                ResolveCollision(ball, otherBall);
                            }
                        }
                        ResolveWallsCollision(ball);
                    }
                }

                // Introduce a delay to control the movement speed (e.g., ~60 FPS)
                await Task.Delay(16);
            }
        }

        private bool CheckCollision(TP.ConcurrentProgramming.Data.IBall ball1, TP.ConcurrentProgramming.Data.IBall ball2)
        {
            double dx = ball1.Position.x - ball2.Position.x;
            double dy = ball1.Position.y - ball2.Position.y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance < (ball1.Diameter / 2 + ball2.Diameter / 2);
        }

        private void ResolveCollision(TP.ConcurrentProgramming.Data.IBall ball1, TP.ConcurrentProgramming.Data.IBall ball2)
        {
            double dx = ball1.Position.x - ball2.Position.x;
            double dy = ball1.Position.y - ball2.Position.y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            if (distance == 0) return; // Prevent division by zero

            // Normalny wektor zderzenia
            double nx = dx / distance;
            double ny = dy / distance;

            // Separacja nakładających się kul
            double overlap = (ball1.Diameter / 2 + ball2.Diameter / 2) - distance;
            if (overlap > 0)
            {
                ball1.Position.x += nx * overlap / 2;
                ball1.Position.y += ny * overlap / 2;
                ball2.Position.x -= nx * overlap / 2;
                ball2.Position.y -= ny * overlap / 2;
            }

            // Wektor względnej prędkości
            double vx = ball1.Velocity.x - ball2.Velocity.x;
            double vy = ball1.Velocity.y - ball2.Velocity.y;

            // Prędkość względna wzdłuż normalnej
            double dotProduct = vx * nx + vy * ny;

            // Jeśli kulki się oddalają — nic nie robić
            if (dotProduct >= 0) return;

            // Obliczenie impulsu (dla równych mas i odbicia sprężystego)
            double impulse = 2 * dotProduct / 2; // dzielimy przez sumę mas (1 + 1)

            // Aktualizacja prędkości
            ball1.Velocity.x -= impulse * nx;
            ball1.Velocity.y -= impulse * ny;

            ball2.Velocity.x += impulse * nx;
            ball2.Velocity.y += impulse * ny;
        }

        private void ResolveWallsCollision(TP.ConcurrentProgramming.Data.IBall ball)
        {
            // Check for collisions with the left and right walls
            if (ball.Position.x - ball.Diameter / 2 < 0)
            {
                ball.Velocity.x = -ball.Velocity.x;
                ball.Position.x = ball.Diameter / 2;
            }
            else if (ball.Position.x + ball.Diameter / 2 > windowData.ScreenWidth - windowData.BorderWidth)
            {
                ball.Velocity.x = -ball.Velocity.x;
                ball.Position.x = windowData.ScreenWidth - windowData.BorderWidth - ball.Diameter / 2;
            }

            // Check for collisions with the top and bottom walls
            if (ball.Position.y - ball.Diameter / 2 < 0)
            {
                ball.Velocity.y = -ball.Velocity.y;
                ball.Position.y = ball.Diameter / 2; // Adjust position to prevent overlap
            }
            else if (ball.Position.y + ball.Diameter / 2 > windowData.ScreenHeight - windowData.BorderWidth)
            {
                ball.Velocity.y = -ball.Velocity.y;
                ball.Position.y = windowData.ScreenHeight - windowData.BorderWidth - ball.Diameter / 2; // Adjust position
            }
        }
    }
}