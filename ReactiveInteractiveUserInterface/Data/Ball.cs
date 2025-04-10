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

        private Vector Position;

        private void RaiseNewPositionChangeNotification()
        {
            NewPositionNotification?.Invoke(this, Position);
        }

        internal void Move(Vector delta)
        {
            double newX = Position.x + delta.x;
            double newY = Position.y + delta.y;

            if (newX - diameter / 2 < 0 - windowData.BorderWidth)
            {
                newX = 0;
            }
            else if (newX + diameter > windowData.ScreenWidth - windowData.BorderWidth)
            {
                newX = windowData.ScreenWidth - diameter - windowData.BorderWidth;
            }

            if (newY - diameter / 2 < 0 - windowData.BorderWidth)
            {
                newY = 0;
            }
            else if (newY + diameter > windowData.ScreenHeight - windowData.BorderWidth)
            {
                newY = windowData.ScreenHeight - diameter - windowData.BorderWidth;
            }

            Position = new Vector(newX, newY);

            RaiseNewPositionChangeNotification();
        }

        #endregion private
    }
}