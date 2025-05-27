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
            velocity = initialVelocity;
            this.diameter = diameter;
        }

        #endregion ctor

        #region IBall

        public event EventHandler<IVector>? NewPositionNotification;

        private Vector position;
        private Vector velocity;
        private readonly object velocityLock = new object();
        private readonly object positionLock = new object();

        public IVector Position
        {
            get
            {
                lock (positionLock)
                    return position;
            }
            set
            {
                lock (positionLock)
                    position = (Vector)value;
            }
        }

        public IVector Velocity
        {
            get
            {
                lock (velocityLock)
                    return new Vector(velocity.x, velocity.y); // zwróć kopię
            }
            private set
            {
                lock (velocityLock)
                    velocity = (Vector)value;
            }
        }

        private double diameter;
        public double Diameter
        {
            get { return diameter; }
            set { diameter = value; }
        }

        #endregion IBall

        public void ApplyImpulse(Vector impulse)
        {
            lock (velocityLock)
            {
                velocity.x += impulse.x;
                velocity.y += impulse.y;
            }
        }

        public void ReflectHorizontally()
        {
            lock (velocityLock)
                velocity.x = -velocity.x;
        }

        public void ReflectVertically()
        {
            lock (velocityLock)
                velocity.y = -velocity.y;
        }

        #region Movement

        internal void Move(Vector delta)
        {
            lock (positionLock)
            {
                position.x += delta.x;
                position.y += delta.y;
            }

            RaiseNewPositionChangeNotification();
        }

        #endregion Movement

        #region private

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        #endregion private
    }
}
