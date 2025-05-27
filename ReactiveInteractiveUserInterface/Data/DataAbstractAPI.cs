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
    public abstract class DataAbstractAPI : IDisposable
    {
        #region Layer Factory

        public static DataAbstractAPI GetDataLayer()
        {
            return modelInstance.Value;
        }

        #endregion Layer Factory

        #region public API

        public abstract void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler, double diameter);

        #endregion public API

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<DataAbstractAPI> modelInstance = new Lazy<DataAbstractAPI>(() => new DataImplementation());

        #endregion private

        public abstract List<IBall> GetBallsList();
    }

    public interface IVector
    {
        double x { get; set; }
        double y { get; set; }
    }

    public interface IBall
    {
        event EventHandler<IVector> NewPositionNotification;
        IVector Velocity { get; }
        IVector Position { get; set; }
        double Diameter { get; set; }

        public void ApplyImpulse(Vector impulse);
        public void ReflectHorizontally();
        public void ReflectVertically();
    }
}