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

        internal Ball(Vector initialPosition, Vector initialVelocity, double diameter)
        {
            Position = initialPosition;
            Velocity = initialVelocity;
            this.diameter = diameter;
        }

        #endregion ctor

        #region IBall

        // Event for notifying about position changes
        public event EventHandler<IVector>? NewPositionNotification;

        // Properties for Position and Velocity
        public IVector Velocity { get; set; }
        public IVector Position { get; set; }

        // Property for Diameter
        private double diameter;
        public double Diameter
        {
            get { return diameter; }
            set { diameter = value; } // Now you can set Diameter
        }

        #endregion IBall

        #region private

        // Method to notify when position changes
        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        // Method for moving the ball
        internal void Move(Vector delta)
        {
            // Update the position based on the velocity and delta
            double newX = Position.x + delta.x;
            double newY = Position.y + delta.y;

            Position = new Vector(newX, newY);

            // Notify listeners about the new position
            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}