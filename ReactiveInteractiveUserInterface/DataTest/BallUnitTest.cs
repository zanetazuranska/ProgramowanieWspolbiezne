﻿//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector testinVector = new Vector(0.0, 0.0);
            double testDiameter = 20.0d;
            Ball newInstance = new(testinVector, testinVector, testDiameter);
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            double testDiameter = 20.0d;
            Ball newInstance = new(initialPosition, new Vector(0.0, 0.0), testDiameter);
            IVector curentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); curentPosition = position; numberOfCallBackCalled++; };
            newInstance.Move(new Vector(0.0, 0.0));
            Assert.AreEqual<int>(1, numberOfCallBackCalled);
            Assert.AreEqual<double>(initialPosition.x, curentPosition.x);
            Assert.AreEqual<double>(initialPosition.y, curentPosition.y);
        }
    }
}