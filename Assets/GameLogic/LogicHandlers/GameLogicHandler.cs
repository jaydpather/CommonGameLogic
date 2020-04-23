using System.Collections;
using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders;
using ThirdEyeSoftware.GameLogic.LogicProviders;
using GameLogic.LogicImplementers;

namespace ThirdEyeSoftware.GameLogic.LogicHandlers
{
    public class GameLogicHandler : BaseLogicHandler, ILogicHandler
    {
        private static GameLogicHandler _instance;
        private ILogicProvider _curLogicProvider;
        private Dictionary<GameState, ILogicProvider> _logicProvidersByGameState = new Dictionary<GameState, ILogicProvider>();
        private ILogicProvider _gameLogicProvider;
        private ILogicProvider _pauseLogicProvider;
        private ILogicProvider _winTransitionLogicProvider;
        private ILogicProvider _winLogicProvider;
        private ILogicProvider _loseTransitionLogicProvider;
        private ILogicProvider _loseLogicProvider;

        public ILogicProvider GameLogicProvider
        {
            get
            {
                if (_gameLogicProvider == null)
                {
                    var asteroidPlacementLogicImplementer = new AsteroidPlacementLogicImplementer(_gameEngineInterface, _gameController.CurLevel.SubLevel);
                    _gameLogicProvider = new GameLogicProvider(this, _gameEngineInterface, _gameController.DataLayer, asteroidPlacementLogicImplementer);
                }

                return _gameLogicProvider;
            }
            set
            {
                _gameLogicProvider = value;
            }
        }
        public ILogicProvider PauseLogicProvider
        {
            get
            {
                if (_pauseLogicProvider == null)
                {
                    _pauseLogicProvider = new PauseLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _pauseLogicProvider;
            }
            set
            {
                _pauseLogicProvider = value;
            }
        }
        public ILogicProvider WinTransitionLogicProvider
        {
            get
            {
                if (_winTransitionLogicProvider == null)
                {
                    _winTransitionLogicProvider = new WinTransitionLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _winTransitionLogicProvider;
            }
            set
            {
                _winTransitionLogicProvider = value;
            }
        }
        public ILogicProvider WinLogicProvider
        {
            get
            {
                if (_winLogicProvider == null)
                {
                    _winLogicProvider = new WinLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _winLogicProvider;
            }
            set
            {
                _winLogicProvider = value;
            }
        }
        public ILogicProvider LoseTransitionLogicProvider
        {
            get
            {
                if (_loseTransitionLogicProvider == null)
                {
                    _loseTransitionLogicProvider = new LoseTransitionLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _loseTransitionLogicProvider;
            }
            set
            {
                _loseTransitionLogicProvider = value;
            }
        }
        public ILogicProvider LoseLogicProvider
        {
            get
            {
                if(_loseLogicProvider == null)
                {
                    _loseLogicProvider = new LoseLogicProvider(_gameController, this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _loseLogicProvider;
            }

            set
            {
                _loseLogicProvider = value;
            }
        }
        public ILogicProvider CurLogicProvider
        {
            get { return _curLogicProvider; }
            set { _curLogicProvider = value; }
        }

        public static GameLogicHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameLogicHandler();
                }

                return _instance;
            }
        }

        private GameLogicHandler()
        {

        }

        public override void OnStart()
        {
            GameController.ShouldUpdate = true;

            GameEngineInterface.ClearGameObjectCache();


            _logicProvidersByGameState.Clear(); //added this line b/c the dictionary keys already exist if you quit to the main menu and come back!
            _logicProvidersByGameState.Add(GameState.InGame, GameLogicProvider);
            _logicProvidersByGameState.Add(GameState.Pause, PauseLogicProvider);
            _logicProvidersByGameState.Add(GameState.WinTransition, WinTransitionLogicProvider);
            _logicProvidersByGameState.Add(GameState.Win, WinLogicProvider);
            _logicProvidersByGameState.Add(GameState.LoseTransition, LoseTransitionLogicProvider);
            _logicProvidersByGameState.Add(GameState.Lose, LoseLogicProvider);

            foreach (var curKvp in _logicProvidersByGameState)
            {
                curKvp.Value.OnStart();
            }

            CurLogicProvider = _logicProvidersByGameState[GameState.InGame];

            _InputStates[InputAxis.Cancel] = 0f;

            //var debugOutputGameObject = _gameEngineInterface.FindGameObject("txtDebugOutput");
            //_txtDebugOutput = debugOutputGameObject.GetComponent<IText>();
            //_txtDebugOutput.Text += Environment.NewLine + "OnStart()";

            SetAppState(AppState.Game);
            SetSceneState((int)GameState.InGame);
        }

        public void OnUpdate()
        {
            CurLogicProvider.UpdateInputStates();
            CurLogicProvider.HandleInput();
            CurLogicProvider.ClearInputStates();
            CurLogicProvider.UpdateGameObjects();
        }

        public override void OnClick(string sender)
        {
            CurLogicProvider.OnClick(sender);
        }

        public void OnCollision(IComponent player, IGameObject cube)
        {
            _curLogicProvider.OnCollision(player, cube);
        }

        public override void SetSceneState(int sceneState)
        {
            GameState gameState = (GameState)sceneState;
            var prevLogicProvider = _curLogicProvider;
            _curLogicProvider = _logicProvidersByGameState[gameState];

            if (prevLogicProvider != null) //todo v2: do we need this if statement? seems like prevLogicProvider is never null
            {
                prevLogicProvider.ClearInputStates(); //we don't want old input states to still be active when we switch back to the original logic provider
                prevLogicProvider.OnDeActivate();
            }

            _curLogicProvider.OnActivate();
        }

        public override void SetAppState(AppState appState, int? sceneState = null)
        {
            GameController.SetAppState(appState, sceneState);
        }

        public void OnFocus(bool hasFocus)
        {
            _curLogicProvider.OnFocus(hasFocus);
        }
    }
}