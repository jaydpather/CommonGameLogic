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
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders;

namespace GameLogicTest.LogicProviders
{
    [TestClass]
    public class GameLogicProviderTest : TestBase
    {
        const int SCREEN_WIDTH = 100;

        IGameController _gameController;
        ILogicHandler _logicHandler;
        IGameEngineInterface _gameEngineInterface;
        IDataLayer _dataLayer;
        IAsteroidPlacementLogicImplementer _asteroidPlacementLogicImplementer;
        IAudioSource _audioSourceShip;
        IGameObject _playerShip;
        IGameObject _camera;
        IGameObject _sourceAsteroid;
        IPlayerShipScript _playerShipScript;
        GameLogicProvider _gameLogicProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _gameController = Substitute.For<IGameController>();
            _logicHandler = Substitute.For<ILogicHandler>();
            _gameEngineInterface = Substitute.For<IGameEngineInterface>();
            _dataLayer = Substitute.For<IDataLayer>();

            _audioSourceShip = Substitute.For<IAudioSource>();

            _playerShip = Substitute.For<IGameObject>();
            _playerShip.Transform = Substitute.For<ITransform>();

            _camera = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("MainCamera").Returns(_camera);
            _camera.Transform.Position.Returns(new Vector3(0, 0, 0));

            _sourceAsteroid = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("sourceAsteroid").Returns(_sourceAsteroid);
            _sourceAsteroid.Transform.Position.Returns(new Vector3(0, 0, 0));

            _playerShipScript = Substitute.For<IPlayerShipScript>();
            _playerShipScript.Health.Returns(100);

            _playerShip.GetSize().Returns(new Vector3(10, 10, 10));

            _gameEngineInterface.Screen.Width.Returns(SCREEN_WIDTH);
            _logicHandler.GameController = _gameController;

            _asteroidPlacementLogicImplementer = Substitute.For<IAsteroidPlacementLogicImplementer>();

            _gameLogicProvider = new GameLogicProvider(_logicHandler, _gameEngineInterface, _dataLayer, _asteroidPlacementLogicImplementer); //todo 2nd game: couldn't get Substitute.ForPartsOf to work, so I had to mock all the stuff needed for the call to InitStage()

            _gameEngineInterface.Time = Substitute.For<ITime>();
            _gameEngineInterface.Time.DeltaTime.Returns(1);
        }

        [TestMethod]
        public void ConstructorTest()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            gameEngineInterface.Screen.Width.Returns(SCREEN_WIDTH); 

            var dataLayer = Substitute.For<IDataLayer>();

            var asteroidPlacementLogicImplementer = Substitute.For<IAsteroidPlacementLogicImplementer>();
            #endregion

            #region act
            var gameLogicProvider = new GameLogicProvider(gameLogicHandler, gameEngineInterface, dataLayer, asteroidPlacementLogicImplementer);
            #endregion

            #region assert
            var halfScreenWidth = GetPrivateMember<int>("_halfScreenWidth", gameLogicProvider);
            Assert.AreEqual(50, halfScreenWidth);

            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", gameLogicProvider);
            Assert.AreEqual(0, inputStates[InputAxis.Horizontal]);
            Assert.AreEqual(0, inputStates[InputAxis.Cancel]);

            Assert.AreEqual(1, gameEngineInterface.TimeScale);
            #endregion 
        }

        [TestMethod]
        public void OnActivate()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();
            var gameEngineInterface = Substitute.For<IGameEngineInterface>();
            var dataLayer = Substitute.For<IDataLayer>();
            
            var audioSources = new IAudioSource[2];
            audioSources[0] = Substitute.For<IAudioSource>();
            audioSources[1] = Substitute.For<IAudioSource>();
            SetPrivateMember("_audioSources", _gameLogicProvider, audioSources);
            #endregion

            #region act
            _gameLogicProvider.OnActivate();
            #endregion

            #region assert
            foreach (var curAudioSource in audioSources)
            {
                curAudioSource.Received(1).UnPause();
            }
            #endregion
        }

        [TestMethod]
        public void OnDeActivate()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var audioSources = new IAudioSource[2];
            audioSources[0] = Substitute.For<IAudioSource>();
            audioSources[1] = Substitute.For<IAudioSource>();
            SetPrivateMember("_audioSources", _gameLogicProvider, audioSources);
            #endregion

            #region act
            _gameLogicProvider.OnDeActivate();
            #endregion

            #region assert
            foreach (var curAudioSource in audioSources)
            {
                curAudioSource.Received(1).Pause();
            }
            #endregion
        }

        [TestMethod]
        public void OnStart()
        {
            #region arrange
            var playerShip = Substitute.For<IGameObject>();
            var playerShipScript = Substitute.For<IPlayerShipScript>();
            playerShipScript.GameEngineInterface = null;
            playerShip.GetComponent<IPlayerShipScript>().Returns(playerShipScript);
            _gameEngineInterface.FindGameObject("PlayerShip").Returns(playerShip);

            var quad = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("Quad").Returns(quad);

            var txtLives = Substitute.For<IGameObject>();
            var iTextLives = Substitute.For<IText>();
            txtLives.GetComponent<IText>().Returns(iTextLives);
            _gameEngineInterface.FindGameObject("txtLives").Returns(txtLives);

            var txtCurLevel = Substitute.For<IGameObject>();
            var iTextCurLevel = Substitute.For<IText>();
            txtCurLevel.GetComponent<IText>().Returns(iTextCurLevel);
            _gameEngineInterface.FindGameObject("txtCurLevel").Returns(txtCurLevel);

            _dataLayer.GetNumLivesRemaining().Returns(1);

            _logicHandler.GameController = Substitute.For<IGameController>();
            _logicHandler.GameController.CurLevel.Returns(new LevelInfo(1, 1));

            _asteroidPlacementLogicImplementer.InitAsteroids(Arg.Any<int>(), Arg.Any<int>()).Returns((IGameObject)null);

            #endregion

            #region act 
            _gameLogicProvider.OnStart();
            #endregion

            #region assert
            _gameEngineInterface.Received(1).FindGameObject("GlobalObject");
            _gameEngineInterface.Received(1).FindGameObject("PlayerShip");
            //_gameEngineInterface.Received(1).FindGameObject("PlayerShield");
            _gameEngineInterface.Received(1).FindGameObject("MainCamera");

            playerShip.Received().GetComponent<IPlayerShipScript>();
            Assert.IsNotNull(playerShipScript.GameEngineInterface);

            _gameEngineInterface.Received(1).FindGameObject("Asteroid");
            _gameEngineInterface.Received(1).FindGameObject("Explosion");
            _gameEngineInterface.Received(1).FindGameObject("Quad");
            quad.Received(1).EnableTextureWrapping();

            _gameEngineInterface.Received(1).FindGameObject("txtLives");
            Assert.AreEqual("LIVES: 1", iTextLives.Text);

            _gameEngineInterface.Received(1).FindGameObject("txtCurLevel");
            Assert.AreEqual("LEVEL: 1-1", iTextCurLevel.Text);

            _gameEngineInterface.Received(1).SetupLighting();

            _asteroidPlacementLogicImplementer.Received(1).InitAsteroids(Arg.Any<int>(), Arg.Any<int>());

            Assert.AreEqual(1, playerShipScript.Health);
            #endregion
        }

        [TestMethod]
        public void HandleInput_Cancel()
        {
            #region arrange
            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", _gameLogicProvider);
            inputStates[InputAxis.Cancel] = 1;
            #endregion

            #region act
            _gameLogicProvider.HandleInput();
            #endregion

            #region assert
            _logicHandler.Received().SetSceneState((int)GameState.Pause);
            #endregion
        }

        [TestMethod]
        public void HandleInput_Move()
        {
            #region arrange
            var gameLogicHandler = Substitute.For<ILogicHandler>();

            var gameEngineInterface = Substitute.For<IGameEngineInterface>();

            var dataLayer = Substitute.For<IDataLayer>();

            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", _gameLogicProvider);
            inputStates[InputAxis.Horizontal] = 1;
            #endregion

            #region act
            _gameLogicProvider.HandleInput();
            #endregion

            #region assert
            Assert.AreNotEqual(0, _gameLogicProvider.ShipHorizontalDirection);
            Assert.IsTrue(_gameLogicProvider.ShouldPlayShipSound);
            #endregion
        }

        private void UpdateGameObjects_Init()
        {
            SetPrivateMember("_audioSourceShip", _gameLogicProvider, _audioSourceShip);
            SetPrivateMember("_playerShip", _gameLogicProvider, _playerShip);
            SetPrivateMember("_playerShipScript", _gameLogicProvider, _playerShipScript);
            SetPrivateMember("_sourceAsteroid", _gameLogicProvider, _sourceAsteroid);

            var lastAsteroid = Substitute.For<IGameObject>();
            lastAsteroid.Transform = Substitute.For<ITransform>();
            SetPrivateMember("_lastAsteroid", _gameLogicProvider, lastAsteroid);
        }

        [TestMethod]
        public void UpdateGameObjects_SomeInput()
        {
            #region arrange
            _gameEngineInterface.ScreenUtils = Substitute.For<IScreenUtils>();
            _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(Arg.Any<IVector3>()).Returns(new GameLogic.Utils.Vector3(0, 0, 0));

            _gameLogicProvider.ShouldPlayShipSound = true;
            _gameLogicProvider.ShipHorizontalDirection = 1;
            _gameLogicProvider.ShipVelocity = 2;

            UpdateGameObjects_Init();
            #endregion

            #region act
            _gameLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            _audioSourceShip.Received().Play();
            _playerShip.Transform.Received(1).Translate(_gameLogicProvider.ShipHorizontalDirection, 0, 0);
            _playerShip.Transform.Received(1).Translate(0, 0, _gameLogicProvider.ShipVelocity);

            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_OutOfBounds_Left()
        {
            #region arrange
            _gameEngineInterface.ScreenUtils = Substitute.For<IScreenUtils>();
            var minXCoord = _playerShip.GetSize().X / 2; //left edge of player ship is at the left edge of the screen. (b/c the center of the ship is 5 units away from the left edge, since ship width is 10).
            _playerShip.Transform.Position.Returns(new Vector3(minXCoord, 0, 0));

            //screenPointXMin:
            _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(Arg.Is<IVector3>(v => v.X == 0 && v.Y == 0 && v.Z == 0)).Returns(new GameLogic.Utils.Vector3(0, 0, 0));
            //worldPointXMin:
            _gameEngineInterface.ScreenUtils.GetWorldPointFromScreenPoint(Arg.Is<IVector3>(v => v.X == 0 && v.Y == 0 && v.Z == 0)).Returns(new Vector3(0, 0, 0));
            //screenPointXMax:
            _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(Arg.Is<IVector3>(v => v.X == 100 && v.Y == 0 && v.Z == 0)).Returns(new GameLogic.Utils.Vector3(SCREEN_WIDTH, 0, 0));
            //worldPointXMax:
            _gameEngineInterface.ScreenUtils.GetWorldPointFromScreenPoint(Arg.Is<IVector3>(v => v.X == 100 && v.Y == 0 && v.Z == 0)).Returns(new Vector3(100, 0, 0));

            _gameLogicProvider.ShouldPlayShipSound = true;
            _gameLogicProvider.ShipHorizontalDirection = -1; //player is trying to move even farther left
            _gameLogicProvider.ShipVelocity = 2;

            UpdateGameObjects_Init();
            #endregion

            #region act
            _gameLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            _audioSourceShip.Received().Play();
            _playerShip.Transform.Received(0).Translate(_gameLogicProvider.ShipHorizontalDirection, 0, 0); //player should not have moved horizontally
            _playerShip.Transform.Received(1).Translate(0, 0, _gameLogicProvider.ShipVelocity);

            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_OutOfBounds_Right()
        {
            #region arrange
            _gameEngineInterface.ScreenUtils = Substitute.For<IScreenUtils>();

            var shipWidth = _playerShip.GetSize().X;
            var maxXCoord = SCREEN_WIDTH - (shipWidth / 2); //the right edge of the ship is at the right edge of the screen. (because the center of the ship is 5 units away from the right edge of the screen, and the ship is 10 units wide)
            _playerShip.Transform.Position.Returns(new Vector3(maxXCoord, 0, 0));

            //screenPointXMin:
            _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(Arg.Is<IVector3>(v => v.X == 0 && v.Y == 0 && v.Z == 0)).Returns(new GameLogic.Utils.Vector3(0, 0, 0));
            //worldPointXMin:
            _gameEngineInterface.ScreenUtils.GetWorldPointFromScreenPoint(Arg.Is<IVector3>(v => v.X == 0 && v.Y == 0 && v.Z == 0)).Returns(new Vector3(0, 0, 0));
            //screenPointXMax:
            _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(Arg.Is<IVector3>(v => v.X == 100 && v.Y == 0 && v.Z == 0)).Returns(new GameLogic.Utils.Vector3(SCREEN_WIDTH, 0, 0));
            //worldPointXMax:
            _gameEngineInterface.ScreenUtils.GetWorldPointFromScreenPoint(Arg.Is<IVector3>(v => v.X == 100 && v.Y == 0 && v.Z == 0)).Returns(new Vector3(100, 0, 0));

            _gameLogicProvider.ShouldPlayShipSound = true;
            _gameLogicProvider.ShipHorizontalDirection = 1; //player is trying to move even farther right
            _gameLogicProvider.ShipVelocity = 2;

            UpdateGameObjects_Init();
            #endregion

            #region act
            _gameLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            _audioSourceShip.Received().Play();
            _playerShip.Transform.Received(0).Translate(_gameLogicProvider.ShipHorizontalDirection, 0, 0); //player should not have moved horizontally
            _playerShip.Transform.Received(1).Translate(0, 0, _gameLogicProvider.ShipVelocity);

            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_NoInput()
        {
            #region arrange

            _gameEngineInterface.ScreenUtils = Substitute.For<IScreenUtils>();
            _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(Arg.Any<IVector3>()).Returns(new GameLogic.Utils.Vector3(0, 0, 0));

            _gameLogicProvider.ShouldPlayShipSound = false;
            _gameLogicProvider.ShipHorizontalDirection = 0;
            _gameLogicProvider.ShipVelocity = 2;

            UpdateGameObjects_Init();
            #endregion

            #region act
            _gameLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            _audioSourceShip.DidNotReceive().Play();
            _playerShip.Transform.Received(1).Translate(0, 0, _gameLogicProvider.ShipVelocity);
            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_PlayerDeath()
        {
            #region arrange
            _gameEngineInterface.ScreenUtils = Substitute.For<IScreenUtils>();
            _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(Arg.Any<IVector3>()).Returns(new GameLogic.Utils.Vector3(0, 0, 0));

            _gameLogicProvider.ShouldPlayShipSound = false;
            _gameLogicProvider.ShipHorizontalDirection = 0;
            _gameLogicProvider.ShipVelocity = 2;

            UpdateGameObjects_Init();
            _playerShipScript.Health.Returns(0);
            
            #endregion

            #region act
            _gameLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            _dataLayer.Received().DecrementNumLivesRemaining(1);
            _logicHandler.Received(1).SetSceneState((int)GameState.LoseTransition);
            #endregion
        }

        [TestMethod]
        public void UpdateGameObjects_BeatLevel()
        {
            #region arrange
            _gameEngineInterface.ScreenUtils = Substitute.For<IScreenUtils>();
            _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(Arg.Any<IVector3>()).Returns(new GameLogic.Utils.Vector3(0, -21, 0));

            _gameLogicProvider.ShouldPlayShipSound = false;
            _gameLogicProvider.ShipHorizontalDirection = 0;
            _gameLogicProvider.ShipVelocity = 2;

            UpdateGameObjects_Init();
            #endregion

            #region act
            _gameLogicProvider.UpdateGameObjects();
            #endregion

            #region assert
            _logicHandler.GameController.Received(1).ProgressToNextLevel();
            _logicHandler.Received(1).SetSceneState((int)GameState.WinTransition);
            #endregion
        }

        [TestMethod]
        public void OnCollision_Asteroid()
        {
            #region arrange
            var asteroid = Substitute.For<IGameObject>();
            asteroid.Name.Returns("Asteroid");
            

            var playerShipScript = Substitute.For<IPlayerShipScript>();
            playerShipScript.Health.Returns(1);
            SetPrivateMember("_playerShipScript", _gameLogicProvider, playerShipScript);
            #endregion

            #region act
            _gameLogicProvider.OnCollision(null, asteroid);
            #endregion

            #region assert
            Assert.AreEqual(0, playerShipScript.Health);
            _gameEngineInterface.Received(1).Clone(Arg.Any<IGameObject>(), Arg.Any<string>());
            asteroid.Received(1).SetActive(false);
            #endregion
        }

        [TestMethod]
        public void OnCollision_NotAsteroid()
        {
            #region arrange
            var nonAsteroid = Substitute.For<IGameObject>();
            nonAsteroid.Name.Returns("ABC"); //asteroid.Name needs to be something that does not contain "Asteroid"


            var playerShipScript = Substitute.For<IPlayerShipScript>();
            playerShipScript.Health.Returns(1);
            SetPrivateMember("_playerShipScript", _gameLogicProvider, playerShipScript);
            #endregion

            #region act
            _gameLogicProvider.OnCollision(null, nonAsteroid);
            #endregion

            #region assert
            //assert that nothing happened, b/c we should have just returned:
            Assert.AreEqual(1, playerShipScript.Health);
            _gameEngineInterface.Received(0).Clone(Arg.Any<IGameObject>(), Arg.Any<string>());
            nonAsteroid.Received(0).SetActive(Arg.Any<bool>());
            #endregion
        }

        [TestMethod]
        public void InitStage()
        {
            _gameController.CurLevel.Returns(new LevelInfo(1, 1));

            _gameLogicProvider.InitStage();

            Assert.AreNotEqual(0, _gameLogicProvider.ShipVelocity);
            _asteroidPlacementLogicImplementer.Received(1).InitAsteroids(Arg.Any<int>(), Arg.Any<int>());

            var lastAsteroid = GetPrivateMember<IGameObject>("_lastAsteroid", _gameLogicProvider);
            Assert.IsNotNull(lastAsteroid);
        }

        [TestMethod]
        public void UpdateInputStates_Horizontal()
        {
            _logicHandler.GetButtonState(InputAxis.Cancel.ToString()).Returns(0);
            _logicHandler.GetTouch(0).Returns((ITouch)null);
            _logicHandler.GetAxisMultiplier(InputAxis.Horizontal.ToString()).Returns(1);

            _gameLogicProvider.UpdateInputStates();

            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", _gameLogicProvider);
            Assert.AreNotEqual(0, inputStates[InputAxis.Horizontal]);
        }

        [TestMethod]
        public void UpdateInputStates_TouchLeft()
        {
            _logicHandler.GetButtonState(InputAxis.Cancel.ToString()).Returns(0);

            var touch = Substitute.For<ITouch>();
            touch.Position.X.Returns(0);
            _logicHandler.GetTouch(0).Returns(touch);
            _logicHandler.GetAxisMultiplier(InputAxis.Horizontal.ToString()).Returns(0); //this ensures we're only setting the horizontal input axis due to a touch on the phone screen, not because of input in the Unity Editor

            _gameLogicProvider.UpdateInputStates();

            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", _gameLogicProvider);
            Assert.AreEqual(-1, inputStates[InputAxis.Horizontal]);
        }

        [TestMethod]
        public void UpdateInputStates_TouchRight()
        {
            _logicHandler.GetButtonState(InputAxis.Cancel.ToString()).Returns(0);

            var touch = Substitute.For<ITouch>();
            touch.Position.X.Returns(51);
            _logicHandler.GetTouch(0).Returns(touch);
            _logicHandler.GetAxisMultiplier(InputAxis.Horizontal.ToString()).Returns(0); //this ensures we're only setting the horizontal input axis due to a touch on the phone screen, not because of input in the Unity Editor

            _gameLogicProvider.UpdateInputStates();

            var inputStates = GetPrivateMember<Dictionary<InputAxis, float>>("_inputStates", _gameLogicProvider);
            Assert.AreEqual(1, inputStates[InputAxis.Horizontal]);
        }
    }
}