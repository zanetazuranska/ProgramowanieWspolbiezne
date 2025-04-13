//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.BusinessLogic;
using TP.ConcurrentProgramming.Data.WindowData;

namespace TP.ConcurrentProgramming.Presentation.Model.Test
{
    [TestClass]
    public class PresentationModelUnitTest
    {
        [TestMethod]
        public void DisposeTestMethod()
        {
            UnderneathLayerFixture underneathLayerFixture = new UnderneathLayerFixture();
            ModelImplementation? newInstance = null;
            using (newInstance = new(underneathLayerFixture))
            {
                newInstance.CheckObjectDisposed(x => Assert.IsFalse(x));
                newInstance.CheckUnderneathLayerAPI(x => Assert.AreSame(underneathLayerFixture, x));
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
                Assert.IsFalse(underneathLayerFixture.Disposed);
            }
            newInstance.CheckObjectDisposed(x => Assert.IsTrue(x));
            newInstance.CheckUnderneathLayerAPI(x => Assert.AreSame(underneathLayerFixture, x));
            Assert.IsTrue(underneathLayerFixture.Disposed);
            Assert.ThrowsException<ObjectDisposedException>(() => newInstance.Dispose());
        }

        [TestMethod]
        public void StartTestMethod()
        {
            UnderneathLayerFixture underneathLayerFixture = new UnderneathLayerFixture();
            using (ModelImplementation newInstance = new(underneathLayerFixture))
            {
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
                IDisposable subscription = newInstance.Subscribe(x => { });
                newInstance.CheckBallChangedEvent(x => Assert.IsFalse(x));
                newInstance.Start(10, 20.0d, new(100, 100, 1));
                Assert.AreEqual<int>(10, underneathLayerFixture.NumberOfBalls);
                Assert.AreEqual<double>(20.0d, underneathLayerFixture.Diameter);
                Assert.AreEqual<WindowData>(new(100, 100, 1), underneathLayerFixture.WindowData);
                subscription.Dispose();
                newInstance.CheckBallChangedEvent(x => Assert.IsTrue(x));
            }
        }

        #region testing instrumentation

        private class UnderneathLayerFixture : BusinessLogicAbstractAPI
        {
            #region testing instrumentation

            internal bool Disposed = false;
            internal int NumberOfBalls = 0;
            internal double Diameter = 0;
            internal WindowData WindowData = new(0, 0, 0);

            #endregion testing instrumentation

            #region BusinessLogicAbstractAPI

            public override void Dispose()
            {
                Disposed = true;
            }

            public override void Start(int numberOfBalls, Action<IPosition, BusinessLogic.IBall> upperLayerHandler, double diameter, WindowData windowData)
            {
                NumberOfBalls = numberOfBalls;
                Diameter = diameter;
                WindowData = windowData;
                Assert.IsNotNull(upperLayerHandler);
            }

            #endregion BusinessLogicAbstractAPI
        }

        #endregion testing instrumentation
    }
}