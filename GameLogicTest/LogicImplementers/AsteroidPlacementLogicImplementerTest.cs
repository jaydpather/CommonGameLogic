using GameLogic.LogicImplementers;
using GameLogic.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;

namespace GameLogicTest.LogicImplementers
{
    [TestClass]
    public class AsteroidPlacementLogicImplementerTest : TestBase
    {
        AsteroidPlacementLogicImplementer _asteroidPlacementLogicImplementer;

        IGameObject _sourceAsteroid;
        IGameObject _playerShip;
        IGameObject _mainCamera;
        IGameEngineInterface _gameEngineInterface;
        private const int WORLD_WIDTH = 50; //in 3D world coordinates, the screen is only 50 units wide

        int _asteroidDensity;
        
        private void LogToDebugOutput(string msg)
        {

        }

        [TestInitialize]
        public void TestInitialize()
        {
            _sourceAsteroid = Substitute.For<IGameObject>();
            _playerShip = Substitute.For<IGameObject>();
            _mainCamera = Substitute.For<IGameObject>();

            _gameEngineInterface = Substitute.For<IGameEngineInterface>();
            _gameEngineInterface.FindGameObject("PlayerShip").Returns(_playerShip);
            _gameEngineInterface.FindGameObject("Asteroid").Returns(_sourceAsteroid);
            _gameEngineInterface.FindGameObject("MainCamera").Returns(_mainCamera);

            _sourceAsteroid.GetSize().X.Returns(2);
            _playerShip.GetSize().X.Returns(10);
            
            _asteroidPlacementLogicImplementer = new AsteroidPlacementLogicImplementer(_gameEngineInterface, _asteroidDensity);
            _asteroidPlacementLogicImplementer.SourceAsteroid = _sourceAsteroid;
            _asteroidPlacementLogicImplementer.LogToDebugOutput = LogToDebugOutput;

            SetPrivateMember("X_MAX", _asteroidPlacementLogicImplementer, 40);
            SetPrivateMember("_asteroidDensity", _asteroidPlacementLogicImplementer, 1);

            //asserts for constructor test:
            _playerShip.Received().GetSize();
            _sourceAsteroid.Received().GetSize();

            _gameEngineInterface.Received(1).FindGameObject("PlayerShip");

            //these mocks used to be part of CalculateXMinAndXMax
            const float CAMERA_HEIGHT = 5;
            const int SCREEN_WIDTH = 100; //the screen is 100 pixels wide

            var leftEdgeOfScreen = new Vector3(0, 0, 0);
            var rightEdgeOfScreen = new Vector3(SCREEN_WIDTH, 0, 0);

            _sourceAsteroid.Transform.Position.Y.Returns(0);
            _mainCamera.Transform.Position.Y.Returns(CAMERA_HEIGHT);

            _gameEngineInterface.Screen.Width.Returns(SCREEN_WIDTH);

            _gameEngineInterface.ScreenUtils = Substitute.For<IScreenUtils>();
            _gameEngineInterface.ScreenUtils
                .GetWorldPointFromScreenPoint(Arg.Is<IVector3>(v => v.X == 0 && v.Y == 0 && v.Z == CAMERA_HEIGHT))
                .Returns(new Vector3(0, 0, 0));

            _gameEngineInterface.ScreenUtils
                .GetWorldPointFromScreenPoint(Arg.Is<IVector3>(v => v.X == SCREEN_WIDTH && v.Y == 0 && v.Z == CAMERA_HEIGHT))
                .Returns(new Vector3(WORLD_WIDTH, 0, 0));

        }

        [TestMethod]
        public void CalculateXMinAndXMax()
        {
            
            var result = CallPrivateMethod<float[]>(_asteroidPlacementLogicImplementer, "CalculateXMinAndXMax", new object[0]);

            _gameEngineInterface.Received(1).FindGameObject("MainCamera");

            _gameEngineInterface.ScreenUtils.Received(2).GetWorldPointFromScreenPoint(Arg.Any<IVector3>());

            _gameEngineInterface.ScreenUtils.ReceivedWithAnyArgs(2).GetWorldPointFromScreenPoint(null);
            //todo 2nd game: figure out how to check for receiving matching arguments when it's a reference type
            //_gameEngineInterface.ScreenUtils.Received(2).GetWorldPointFromScreenPoint(Arg.Is<Vector3>(v => v.X == 0 && v.Y == 0 && v.Z == CAMERA_HEIGHT));
            //_gameEngineInterface.ScreenUtils.ReceivedWithAnyArgs(2).GetWorldPointFromScreenPoint(Arg.Is<Vector3>(v => v.X == SCREEN_WIDTH && v.Y == 0 && v.Z == CAMERA_HEIGHT));
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result[0]);
            Assert.AreEqual(WORLD_WIDTH, result[1]);
        }

        [TestMethod]
        public void InitAsteroids()
        {
            SetPrivateMember("X_MAX", _asteroidPlacementLogicImplementer, 50f);
            var result = _asteroidPlacementLogicImplementer.InitAsteroids(100, 10);

            _sourceAsteroid.Received().SetActive(false);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PlaceAsteroidsOnLine()
        {
            var results = CallPrivateMethod<IGameObject[]>(_asteroidPlacementLogicImplementer, "PlaceAsteroidsOnLine", new object[] { new Random(), new AsteroidPlacement(), null });

            //with the values we've set up for asteroid/ship width, density level, etc., the do-while loop in PlaceAsteroidsOnLine will execute twice. the second time, transvector will be null, so it will break out of the loop.
            _gameEngineInterface.Received(1).Clone(_sourceAsteroid, Arg.Any<string>());

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);
            Assert.IsNotNull(results[0]);
        }

        [TestMethod]
        public void GetAsteroidPosition_AsteroidOnScreen()
        {
            var prevXCoord = 0; //using a value of 0 here will result in an asteroid that's on-screen. (b/c of the other float values we've mocked)
            var result = CallPrivateMethod<Vector3>(_asteroidPlacementLogicImplementer, "GetAsteroidPosition", new object[] { new Random(), new AsteroidPlacement(), prevXCoord, true });

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GetAsteroidPosition_AsteroidOffScreen()
        {
            var prevXCoord = 30; //using a value of 30 here will result in an asteroid that's off-screen. (b/c of the other float values we've mocked)
            var result = CallPrivateMethod<Vector3>(_asteroidPlacementLogicImplementer, "GetAsteroidPosition", new object[] { new Random(), new AsteroidPlacement(), prevXCoord, false });

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCurDistance()
        {
            //there isn't really anything we can assert for here, because of the randomness involved in calculating the distance.
            //each density level has overlapping ranges of min/max values
            //here we just call the method to make sure it doesn't throw an exception.
            CallPrivateMethod<float>(_asteroidPlacementLogicImplementer, "getCurDistance", new object[] { 1, false });
        }


        [TestMethod]
        public void GetCurDistance_FirstLineAlwaysOnScreen()
        {
            var xMin = -1f;
            var xMax = 1f;
            var asteroidWidth = 0.1f;

            SetPrivateMember("SHIP_WIDTH", _asteroidPlacementLogicImplementer, 0.2f);
            SetPrivateMember("ASTEROID_WIDTH", _asteroidPlacementLogicImplementer, asteroidWidth);
            SetPrivateMember("X_MIN", _asteroidPlacementLogicImplementer, xMin);
            SetPrivateMember("X_MAX", _asteroidPlacementLogicImplementer, xMax);

            var minDistance = xMin + asteroidWidth / 2;
            var maxDistance = (xMax - xMin) - asteroidWidth / 2;

            for (var i=0; i<1000000; i++)
            {
                var result =  CallPrivateMethod<float>(_asteroidPlacementLogicImplementer, "getCurDistance", new object[] { 1, true });

                Assert.IsTrue(result >= minDistance);
                Assert.IsTrue(result <= maxDistance);
            }
            
        }

        [TestMethod]
        public void GetZCoord()
        {
            var random = new Random();
            var zMin = 10;

            var numLines = 3;
            var results = new float[numLines];
            for(var i=0; i<numLines; i++)
            {
                results[i] =  CallPrivateMethod<float>(_asteroidPlacementLogicImplementer, "GetZCoord", new object[] { 100, 10, zMin, random, i });
            }

            //assert:
            for(var i=0; i<numLines; i++)
            {
                if(i == 0)
                {
                    Assert.IsTrue(results[i] >= zMin);
                }
                else
                {
                    Assert.IsTrue(results[i] > results[i-1]);
                }
            }
        }
    }
}
