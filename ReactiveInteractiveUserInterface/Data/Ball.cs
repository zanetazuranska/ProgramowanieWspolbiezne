//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
    internal class Ball : IBall
    {
        #region ctor

        internal Ball(Vector initialPosition, Vector initialVelocity, double diameter, WindowData.WindowData windowData)
        {
            Position = initialPosition;
            Velocity = initialVelocity;

            this.diameter = diameter;
            this.windowData = windowData;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        public IVector Velocity { get; set; }

        private double diameter;
        private WindowData.WindowData windowData;


        #endregion IBall

        #region private

        public Vector Position { get; set; }
        public double Diameter { get { return this.diameter; } }

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta)
        {
            // Update the position based on the velocity and delta
            double newX = Position.x + delta.x;
            double newY = Position.y + delta.y;

            Position = new Vector(newX, newY);

            // Check and resolve wall collisions
            ResolveCollision();

            // Notify listeners about the new position
            RaiseNewPositionChangeNotification();
        }

        private void ResolveCollision()
        {
            // Check for collisions with the left and right walls
            if (Position.x - diameter / 2 < 0)
            {
                Velocity = new Vector(-Velocity.x, Velocity.y); // Reverse X velocity
                Position = new Vector(diameter / 2, Position.y); // Adjust position to prevent overlap
            }
            else if (Position.x + diameter / 2 > windowData.ScreenWidth - windowData.BorderWidth)
            {
                Velocity = new Vector(-Velocity.x, Velocity.y); // Reverse X velocity
                Position = new Vector(windowData.ScreenWidth - windowData.BorderWidth - diameter / 2, Position.y); // Adjust position
            }

            // Check for collisions with the top and bottom walls
            if (Position.y - diameter / 2 < 0)
            {
                Velocity = new Vector(Velocity.x, -Velocity.y); // Reverse Y velocity
                Position = new Vector(Position.x, diameter / 2); // Adjust position to prevent overlap
            }
            else if (Position.y + diameter / 2 > windowData.ScreenHeight - windowData.BorderWidth)
            {
                Velocity = new Vector(Velocity.x, -Velocity.y); // Reverse Y velocity
                Position = new Vector(Position.x, windowData.ScreenHeight - windowData.BorderWidth - diameter / 2); // Adjust position
            }
        }

        #endregion private
    }
}