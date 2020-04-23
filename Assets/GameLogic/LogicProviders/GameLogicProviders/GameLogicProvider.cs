using GameLogic.LogicImplementers;
using GameLogic.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.Utils;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders
{
    public class GameLogicProvider : BaseLogicProvider
    {
        private int _halfScreenWidth;
        private IGameObject _playerShip;
        private float _shipWidth;
        //private IGameObject _playerShield;
        private IGameObject _camera;
        private IPlayerShipScript _playerShipScript;
        private IGameObject _sourceExplosion;
        private IGameObject _sourceAsteroid;
        private IGameObject _lastAsteroid;
        private IGameObject _sunSpotLight;
        private IGameObject _shipBrighteningSpotLight;
        private IGameObject _shipSpotLight;
        private IAudioSource[] _audioSources;
        private IAudioSource _audioSourceShip;
        private IAudioSource _music1;
        private IAudioSource _music2;
        private IAudioSource _music3;
        private IText _txtLives;
        private IAsteroidPlacementLogicImplementer _asteroidPlacementLogicImplementer;
        private int _curLevel;
        private List<IVector3> _asteroidRotations;
        private float _worldPointXMin;
        private float _worldPointXMax;
        private float _horizontalInputLastFrame = 0;
        private DateTime _gameStartTime;
        private DateTime _pauseStartTime;
        private TimeSpan _totalTimePaused = TimeSpan.Zero;

        public float PlayerHorizontalSpeed { get { return 3.75f; } }

        public float ShipHorizontalDirection { get; set; } //set to 1 for RIGHT or -1 for LEFT
        public float ShipVelocity { get; set; }
        public bool ShouldPlayShipSound { get; set; }

        private IAudioSource MusicAudioSource
        {
            get
            {
                switch(_logicHandler.GameController.CurLevel.SubLevel)
                {
                    case 1:
                        return _music1;
                        break;
                    case 2:
                        return _music2;
                        break;
                    case 3:
                        return _music3;
                        break;
                    default:
                        return null;
                }
            }
        }

        public string GlobalMessage { get; set; }

        public GameLogicProvider(ILogicHandler gameLogicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer, IAsteroidPlacementLogicImplementer asteroidPlacementLogicImplementer)
            : base(gameLogicHandler, gameEngineInterface, dataLayer)
        {
            _halfScreenWidth = _gameEngineInterface.Screen.Width / 2;

            _inputStates[InputAxis.Horizontal] = 0f;
            _inputStates[InputAxis.Cancel] = 0f;

            _gameEngineInterface.TimeScale = 1;

            _asteroidPlacementLogicImplementer = asteroidPlacementLogicImplementer;
        }

        public override void OnActivate()
        {
            foreach (var curAudioSource in _audioSources)
            {
                curAudioSource.UnPause();
            }

            var timeSpentPaused = DateTime.Now - _pauseStartTime;
            _totalTimePaused += timeSpentPaused;
        }

        public override void OnDeActivate()
        {
            _pauseStartTime = DateTime.Now;

            foreach (var curAudioSource in _audioSources)
            {
                curAudioSource.Pause();
            }
        }

        public override void OnStart()
        {
            try
            { 
                base.OnStart();
                ClearDebugOutput();

                InitMusic();

                _playerShip = _gameEngineInterface.FindGameObject("PlayerShip");
                _playerShip.LogicHandler = _logicHandler; //todo v2: should we just have a delegate for OnCollision?
                _shipWidth = _playerShip.GetSize().X;

                _camera = _gameEngineInterface.FindGameObject("MainCamera");

                //todo Post-UT: remove PlayerShield from code and Unity project
                //_playerShield = _gameEngineInterface.FindGameObject("PlayerShield");
                //_playerShield.SetActive(false);

                _audioSourceShip = _playerShip.GetComponent<IAudioSource>();
                _audioSources = new IAudioSource[] { MusicAudioSource, _audioSourceShip };

                _sourceAsteroid = _gameEngineInterface.FindGameObject("Asteroid");

                _asteroidPlacementLogicImplementer.SourceAsteroid = _sourceAsteroid;
                _asteroidPlacementLogicImplementer.LogToDebugOutput = LogToDebugOutput;

                _sourceExplosion = _gameEngineInterface.FindGameObject("Explosion");
                _sourceExplosion.SetActive(false); //we are going to clone this game object each time an asteroid explodes, but we don't actually want the original explosion in the game.

                var quad = _gameEngineInterface.FindGameObject("Quad");
                quad.EnableTextureWrapping();

                var txtLivesGameObject = _gameEngineInterface.FindGameObject("txtLives");
                _txtLives = txtLivesGameObject.GetComponent<IText>();
                _txtLives.Text = "LIVES: " + _dataLayer.GetNumLivesRemaining();

                var txtCurLevelGameObject = _gameEngineInterface.FindGameObject("txtCurLevel");
                var txtCurLevel = txtCurLevelGameObject.GetComponent<IText>();
                txtCurLevel.Text = "LEVEL: " + _logicHandler.GameController.CurLevel.DisplayValue;

                _sunSpotLight = _gameEngineInterface.FindGameObject("SunSpotLight");
                _shipBrighteningSpotLight = _gameEngineInterface.FindGameObject("ShipBrighteningSpotLight");
                _shipSpotLight = _gameEngineInterface.FindGameObject("ShipSpotLight");
                _gameEngineInterface.SetupLighting();

                var btnPause = _gameEngineInterface.FindGameObject("btnPause");
                btnPause.LogicHandler = _logicHandler;

                InitStage();
                InitAsteroidRotations();
                InitScreenBounds();
                InitShip();
                //InitLights();
                InitNebulas();

                GC.Collect();

                _gameStartTime = DateTime.Now;
                _totalTimePaused = TimeSpan.Zero; //reset this each time the player starts a new level              
            }
            catch(Exception ex)
            {
                LogToDebugOutput($"message: {ex.Message}");
                LogToDebugOutput($"stack trace: {ex.StackTrace}");
            }
        }

        private void InitNebulas()
        {
            var nebula1 = _gameEngineInterface.FindGameObject("Nebula1");
            var nebula2 = _gameEngineInterface.FindGameObject("Nebula2");
            var nebula3 = _gameEngineInterface.FindGameObject("Nebula3");
            var nebula4 = _gameEngineInterface.FindGameObject("Nebula4");
            var nebula5 = _gameEngineInterface.FindGameObject("Nebula5");

            switch (_logicHandler.GameController.CurLevel.Level)
            {
                case 1:
                    //red
                    nebula2.SetActive(false);
                    nebula3.SetActive(false);
                    nebula4.SetActive(false);
                    nebula5.SetActive(false);
                    break;
                case 2:
                    //green
                    nebula1.SetActive(false);
                    nebula3.SetActive(false);
                    nebula4.SetActive(false);
                    nebula5.SetActive(false);
                    break;
                case 3:
                    //blue
                    nebula1.SetActive(false);
                    nebula2.SetActive(false);
                    nebula4.SetActive(false);
                    nebula5.SetActive(false);
                    break;
                case 4:
                    //yellow
                    nebula1.SetActive(false);
                    nebula2.SetActive(false);
                    nebula3.SetActive(false);
                    nebula5.SetActive(false);
                    break;
                case 5:
                    //purple
                    nebula1.SetActive(false);
                    nebula2.SetActive(false);
                    nebula3.SetActive(false);
                    nebula4.SetActive(false);
                    break;
            }
        }

        private void InitNebulas_Old()
        {
            switch(_logicHandler.GameController.CurLevel.Level)
            {
                case 1:
                    //red
                    _gameEngineInterface.SetNebulaColor(1, 0, 0, 0.207f);
                    break;
                case 2:
                    //green
                    _gameEngineInterface.SetNebulaColor(0, 1, 0, 0.152f);
                    break;
                case 3:
                    //blue
                    _gameEngineInterface.SetNebulaColor(0, 0.33f, 1, 0.242f);
                    break;
                case 4:
                    //yellow
                    _gameEngineInterface.SetNebulaColor(1, 1, 0, 0.152f);
                    break;
                case 5:
                    //purple
                    _gameEngineInterface.SetNebulaColor(0.5f, 0, 1, 0.196f);
                    break;
            }

            
        }

        private void InitMusic()
        {
            var music1 = _gameEngineInterface.FindGameObject("Music1");
            var music2 = _gameEngineInterface.FindGameObject("Music2");
            var music3 = _gameEngineInterface.FindGameObject("Music3");

            _music1 = music1.GetComponent<IAudioSource>();
            _music2 = music2.GetComponent<IAudioSource>();
            _music3 = music3.GetComponent<IAudioSource>();

            //_music1.Time = 23.975f;
            _music1.Time = 24.5f;
            _music2.Time = 38f;
            _music3.Time = 0f;

            switch (_logicHandler.GameController.CurLevel.SubLevel)
            {
                case 1:
                    music2.SetActive(false);
                    music3.SetActive(false);
                    break;
                case 2:
                    music1.SetActive(false);
                    music3.SetActive(false);
                    break;
                case 3:
                    music1.SetActive(false);
                    music2.SetActive(false);
                    break;
            }
        }

        private void InitShip()
        {
            _playerShipScript = _playerShip.GetComponent<IPlayerShipScript>();
            _playerShipScript.GameEngineInterface = _gameEngineInterface;

            _playerShipScript.Health = 1;
            _playerShipScript.HeadingAngle = 90;
        }

        public void InitStage()
        {
            var levelInfo = _logicHandler.GameController.CurLevel;

            const float MIN_SHIP_VELOCITY = 2.769f; //3.692f;
            const float SHIP_VELOCITY_INCREMENT = 0.601f;//0.572f; //0.635f; //0.706f;
            const float LEVEL_TIME_SECONDS = 40f;
            const float ASTEROID_LINES_PER_UNIT_DISTANCE = 0.42f; ////original number of asteroid lines was 84 lines per 200 units of distance

            ShipVelocity = MIN_SHIP_VELOCITY + SHIP_VELOCITY_INCREMENT * (levelInfo.Level - 1);
            var levelLength = (int)(ShipVelocity * LEVEL_TIME_SECONDS); //we want the level to last 40 seconds.
            var numAsteroidLines = (int)(levelLength * ASTEROID_LINES_PER_UNIT_DISTANCE);

            _asteroidPlacementLogicImplementer.AsteroidDensity = _logicHandler.GameController.CurLevel.SubLevel;
            _lastAsteroid = _asteroidPlacementLogicImplementer.InitAsteroids(levelLength, numAsteroidLines);
        }

        public void InitLights()
        {
            //vars for red, green, blue light color:
            //init to white:
            var r = 0.511f;
            var g = 0.511f;
            var b = 0.511f;

            var curLevel = _logicHandler.GameController.CurLevel;

            //level 1 will use default value of white
            if (curLevel.Level == 2) 
            {
                //blue
                r = 0.4f;
                g = 0.499f;
                b = 1f;
            }
            else if (curLevel.Level == 3) 
            {
                //red
                r = 0.922f;
                g = 0.461f;
                b = 0.461f;
            }
            else if (curLevel.Level == 4) 
            {
                //orange
                r = 0.712f;
                g = 0.527f;
                b = 0.263f;

            }
            else if (curLevel.Level == 5) 
            {
                //yellow
                r = 0.808f;
                g = 0.770f;
                b = 0.444f;
            }

            _gameEngineInterface.SetMainLightColor(r, g, b);
        }

        public void InitScreenBounds()
        {
            var distanceFromCamera = _camera.Transform.Position.Y - _sourceAsteroid.Transform.Position.Y;

            var screenPointXMin = new Vector3(0, 0, distanceFromCamera);
            _worldPointXMin = _gameEngineInterface.ScreenUtils.GetWorldPointFromScreenPoint(screenPointXMin).X;

            var screenPointXMax = new Vector3(_gameEngineInterface.Screen.Width, 0, distanceFromCamera);
            _worldPointXMax = _gameEngineInterface.ScreenUtils.GetWorldPointFromScreenPoint(screenPointXMax).X;
        }
        
        public override void UpdateInputStates()
        {
            base.UpdateInputStates();

            _inputStates[InputAxis.Horizontal] = _logicHandler.GetAxisMultiplier(InputAxis.Horizontal.ToString());

            if (_touch != null) //note: when we are running in the Unity Player, _touch is always null, and _inputStates[InputAxis.Vertical] is set by BaseLogicProvider.UpdateInputStates()
            {
                if (_touch.Position.X > _halfScreenWidth)
                {
                    _inputStates[InputAxis.Horizontal] = 1;
                }
                else
                {
                    _inputStates[InputAxis.Horizontal] = -1;
                }
            }
        }

        public override void OnFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                _logicHandler.SetSceneState((int)GameState.Pause);
            }
        }

        public override void HandleInput()
        {
            if (_inputStates[InputAxis.Cancel] != 0)
            {
                PauseGame();
            }
            else
            {   
                ShipHorizontalDirection = _inputStates[InputAxis.Horizontal];

                var horizontalInputCurrentFrame = _inputStates[InputAxis.Horizontal];
                ShouldPlayShipSound = (horizontalInputCurrentFrame != 0 && horizontalInputCurrentFrame != _horizontalInputLastFrame);
                _horizontalInputLastFrame = horizontalInputCurrentFrame; //update for next frame
            }
        }

        public override void OnClick(string sender)
        {
            switch (sender)
            {
                case "btnPause":
                    PauseGame();
                    break;
            }
        }

        private void PauseGame()
        {
            _logicHandler.SetSceneState((int)GameState.Pause);
        }

        public override void UpdateGameObjects()
        {
            //LogToDebugOutput($"{1f/_gameEngineInterface.Time.DeltaTime}fps");
            
            if (ShouldPlayShipSound)
                _audioSourceShip.PlayOneShot();

            UpdateAsteroidRotation();

            UpdateShipRotation();
            UpdateShipPosition();

            if (_playerShipScript.Health <= 0)
            {
                _dataLayer.DecrementNumLivesRemaining(1);
                _logicHandler.SetSceneState((int)GameState.LoseTransition);
            }
            else
            {
                //if the last asteroid flies off the screen, you win
                var screenCoordsAsteroid = _gameEngineInterface.ScreenUtils.GetScreenPointFromWorldPoint(_lastAsteroid.Transform.Position);
                if (screenCoordsAsteroid.Y < -20.0f) // checking for < 0 is just when the center of the asteroid is off screen. todo real game: make a new constant ASTEROID_HEIGHT that you can use to tell if the asteroid is completely off-screen
                {
                    CalculateScore();

                    _logicHandler.GameController.ProgressToNextLevel();
                    _logicHandler.SetSceneState((int)GameState.WinTransition);
                }
            }
        }

        private int CalculateScore()
        {
            var finishingTime = DateTime.Now;
            var timeDiff = (finishingTime - _gameStartTime);
            timeDiff -= _totalTimePaused;

            var levelTime = (int)timeDiff.TotalMilliseconds;

            //multiply levelBonus and SubLevelBonus by 10^6 and 10^5, so that they contribute most to the score
            //(time bonus will be a 5-digit number)
            var levelBonus = (int)Math.Pow(10, 6) * _logicHandler.GameController.CurLevel.Level;
            var subLevelBonus = (int)Math.Pow(10, 5) * _logicHandler.GameController.CurLevel.SubLevel;

            
            //levelTime will be a 5-digit number 
            //(because the levels take 40 seconds, and we are measuring in TotalMilliseconds)
            //(40 is 2 digits, and milliseconds adds an extra 3, so that makes for a total of 5 digits)
            //so, we'll subtract levelTime from 99,999 to get the time bonus. (99,999 is the highest 5-digit number)
            //(this means faster times get a higher time bonus)
            const int highestPossibleFiveDigitNumber = 99999;
            var timeBonus = highestPossibleFiveDigitNumber - levelTime;

            var totalScore = levelBonus + subLevelBonus + timeBonus;

            _logicHandler.GameController.ScoreInfo.LevelBonus = levelBonus;
            _logicHandler.GameController.ScoreInfo.SubLevelBonus = subLevelBonus;
            _logicHandler.GameController.ScoreInfo.TimeBonus = timeBonus;

            _logicHandler.GameController.ScoreInfo.TotalScore = totalScore;

            return totalScore;
            //_gameEngineInterface.SubmitScoreToLeaderboard(score, SubmitScore_Complete);
        }

        private void SubmitScore_Complete(bool success)
        {
            //LogToDebugOutput("SubmitScore_Complete - GameLogicProvider callback");
            //LogToDebugOutput($"success == {success}");
        }

        private void InitAsteroidRotations()
        {
            _asteroidRotations = new List<IVector3>();
            RandomNumberGenerator randomGenerator = new RandomNumberGenerator(0.33f);

            foreach (var curAsteroid in _asteroidPlacementLogicImplementer.AllAsteroids)
            {
                var rotations = new Vector3(0, 0, 0);
                rotations.X = -_gameEngineInterface.Time.DeltaTime * randomGenerator.GetRandomValue(50, 100);
                rotations.Y = _gameEngineInterface.Time.DeltaTime * randomGenerator.GetRandomValue(50, 100);
                rotations.Z = _gameEngineInterface.Time.DeltaTime * randomGenerator.GetRandomValue(50, 100);

                _asteroidRotations.Add(rotations);
            }
        }

        private float ConvertToRadians(float degrees)
        {
            return (float)(Math.PI / 180) * degrees;
        }

        private void UpdateShipRotation()
        {
            var rotationalSpeed_degreesPerSecond = 200f;
            rotationalSpeed_degreesPerSecond *= ShipHorizontalDirection; //multiply by 1 or -1
            var rotationalDifference = rotationalSpeed_degreesPerSecond * _gameEngineInterface.Time.DeltaTime;

            var prevAngle = _playerShipScript.HeadingAngle;
            _playerShipScript.HeadingAngle += rotationalDifference;
            _playerShipScript.HeadingAngle = Clamp(_playerShipScript.HeadingAngle, 45, 135);

            //now that we've clamped the heading angle, we need to check how much rotational difference was actually applied, before we rotate the model
            var actualRotationalDifference = _playerShipScript.HeadingAngle - prevAngle;

            _playerShipScript.ModelRotation.X = 0;
            _playerShipScript.ModelRotation.Y = actualRotationalDifference; //ConvertToRadians(rotationalDifference);
            _playerShipScript.ModelRotation.Z = -actualRotationalDifference / 1f;

            _playerShip.Transform.Rotate(_playerShipScript.ModelRotation);
            _shipSpotLight.Transform.Rotate(_playerShipScript.ModelRotation);

            _playerShip.Transform.EulerAngleX = 0;
            _shipSpotLight.Transform.EulerAngleX = 0;
        }

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        private void UpdateShipPosition()
        {
            var distance = ShipVelocity * _gameEngineInterface.Time.DeltaTime;

            var angleInRadians = ConvertToRadians(_playerShipScript.HeadingAngle);
            var verticalProportion = (float)Math.Sin(angleInRadians);
            var horizontalProportion = (float)Math.Cos(angleInRadians);

            var verticalDistance = verticalProportion * distance;
            var horizontalDistance = horizontalProportion * distance;
            horizontalDistance *= -1; //somehow this is in the wrong direction. todo post-prod: figure out why.
            horizontalDistance = ClampHorizontalDistance(horizontalDistance);
            

            _playerShip.Transform.Translate(horizontalDistance, 0, verticalDistance);
            _shipBrighteningSpotLight.Transform.Translate(horizontalDistance, 0, verticalDistance);
            _shipSpotLight.Transform.Translate(horizontalDistance, 0, verticalDistance);

            _sunSpotLight.Transform.Translate(0, 0, verticalDistance);
            _gameEngineInterface.TranslateCamera(0, 0, verticalDistance);
        }

        private float ClampHorizontalDistance(float desiredHorizontalDistance)
        {
            var retVal = desiredHorizontalDistance;

            var shipPosition = _playerShip.Transform.Position;
            if ((desiredHorizontalDistance < 0 && shipPosition.X > (_worldPointXMin + _shipWidth / 2)) ||
              (desiredHorizontalDistance > 0 && shipPosition.X < (_worldPointXMax - _shipWidth / 2)))
            {
                //CALCULATE REMAINING DISTANCE AND APPLY IF NECESSARY:
                //let's say horizontalDistance is 1, but the player is only 0.5 units from the edge
                //in this case, we should move the ship only 0.5 units instead of 1 unit.
                if (desiredHorizontalDistance > 0)
                {
                    var remainingDistance = (_worldPointXMax - _shipWidth / 2) - shipPosition.X;
                    if (remainingDistance < desiredHorizontalDistance)
                    {
                        retVal = 0;
                    }
                }
                else if (desiredHorizontalDistance < 0)
                {
                    var remainingDistance = (_worldPointXMin + _shipWidth / 2) - shipPosition.X;
                    if (remainingDistance > desiredHorizontalDistance)
                    {
                        retVal = 0;
                    }
                }
            }

            return retVal;
        }

        private void UpdateAsteroidRotation()
        {
            for(var i=0; i<_asteroidPlacementLogicImplementer.AllAsteroids.Count; i++)
            {
                var curAsteroid = _asteroidPlacementLogicImplementer.AllAsteroids[i];
                var curRotation = _asteroidRotations[i];
                
                curAsteroid.Transform.Rotate(curRotation);
            }
        }

        public override void OnCollision(IComponent player, IGameObject asteroid)
        {
            if (!asteroid.Name.Contains("Asteroid")) //we get 2 collision events, one with the asteroid game object, another with "Collider"
                return;

            _playerShipScript.Health -= 1; //todo v2: damage should be a property in IAsteroidScript

            //removing asteroid explosion. it looks weird to have 2 explosions when the ship also explodes
            //var newExplosion = _gameEngineInterface.Clone(_sourceExplosion, string.Empty);
            //newExplosion.Transform.Position = asteroid.Transform.Position;
            //newExplosion.SetActive(true); //todo real game: need to pause the explosion's audio source if the user pauses while the explosion is still going
            
            asteroid.SetActive(false);
        }
    }
}