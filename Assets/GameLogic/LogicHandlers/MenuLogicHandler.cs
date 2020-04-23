using GameLogic.LogicProviders.MenuLogicProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic.LogicProviders;
using ThirdEyeSoftware.GameLogic.LogicProviders.MenuLogicProviders;

namespace ThirdEyeSoftware.GameLogic.LogicHandlers
{
    public class MenuLogicHandler : BaseLogicHandler, ILogicHandler
    {
        private Dictionary<MenuState, ILogicProvider> _logicProvidersByState = new Dictionary<MenuState, ILogicProvider>();
        private ILogicProvider _curLogicProvider;
        private ILogicProvider _loadingLogicProvider;
        private ILogicProvider _inMenuLogicProvider;
        private ILogicProvider _outOfLivesLogicProvider;
        private ILogicProvider _chooseLevelLogicProvider;
        private ILogicProvider _getMoreLivesLogicProvider;
        private ILogicProvider _howToPlayLogicProvider;

        private IText _txtLoadingPercentText;

        private static MenuLogicHandler _instance;

        public ILogicProvider OutOfLivesLogicProvider
        {
            get
            {
                if (_outOfLivesLogicProvider == null)
                {
                    _outOfLivesLogicProvider = new OutOfLivesLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _outOfLivesLogicProvider;
            }

            set
            {
                _outOfLivesLogicProvider = value;
            }
        }

        public ILogicProvider ChooseLevelLogicProvider
        {
            get
            {
                if (_chooseLevelLogicProvider == null)
                {
                    _chooseLevelLogicProvider = new ChooseLevelLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _chooseLevelLogicProvider;
            }

            set
            {
                _chooseLevelLogicProvider = value;
            }
        }

        public ILogicProvider LoadingLogicProvider
        {
            get
            {
                if (_loadingLogicProvider == null)
                {
                    _loadingLogicProvider = new LoadingLogicProvider(_gameController, this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _loadingLogicProvider;
            }

            set
            {
                _loadingLogicProvider = value;
            }
        }

        public ILogicProvider InMenuLogicProvider
        {
            get
            {
                if (_inMenuLogicProvider == null)
                {
                    _inMenuLogicProvider = new InMenuLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _inMenuLogicProvider;
            }

            set
            {
                _inMenuLogicProvider = value;
            }
        }

        public ILogicProvider HowToPlayLogicProvider
        {
            get
            {
                if (_howToPlayLogicProvider == null)
                {
                    _howToPlayLogicProvider = new HowToPlayLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);
                }

                return _howToPlayLogicProvider;
            }

            set
            {
                _howToPlayLogicProvider = value;
            }
        }

        public ILogicProvider EULALogicProvider { get; set; }

        public ILogicProvider CurLogicProvider
        {
            get
            {
                return _curLogicProvider;
            }

            set
            {
                _curLogicProvider = value;
            }
        }

        public string GlobalMessage
        {
            get;
            set;
        }

        public int DefaultSceneState
        {
            get;
            set;
        }

        private MenuLogicHandler()
        {
            
        }

        private MenuState GetDefaultSceneState()
        {
            if (_gameController.DataLayer.GetIsEULAAccepted())
            {
                if(_gameController.DataLayer.GetNumLivesRemaining() > 0)
                {
                    return MenuState.InMenu;
                }
                else
                {
                    return MenuState.GetMoreLives;
                }
            }
            else
            {
                return MenuState.EULA;
            }
        }

        public override IGameController GameController
        {
            get { return base.GameController; }
            set
            {
                base.GameController = value;
                DefaultSceneState = (int)GetDefaultSceneState(); //this relies on the game controller
            }
        }

        public static MenuLogicHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MenuLogicHandler();
                }

                return _instance;
            }
        }

        public override void OnClick(string sender)
        {
            _curLogicProvider.OnClick(sender);
        }

        public override void OnStart()
        {
            _gameController.ShouldUpdate = true;

            _getMoreLivesLogicProvider = new GetMoreLivesLogicProvider(this, _gameEngineInterface, _gameController.DataLayer);

            EULALogicProvider = new EULALogicProvider(this, _gameEngineInterface, _gameController.DataLayer);

            _logicProvidersByState[MenuState.Loading] = LoadingLogicProvider;
            _logicProvidersByState[MenuState.ChooseLevel] = ChooseLevelLogicProvider;
            _logicProvidersByState[MenuState.InMenu] = InMenuLogicProvider;
            _logicProvidersByState[MenuState.OutOfLives] = OutOfLivesLogicProvider;
            _logicProvidersByState[MenuState.GetMoreLives] = _getMoreLivesLogicProvider;
            _logicProvidersByState[MenuState.HowToPlay] = HowToPlayLogicProvider;
            _logicProvidersByState[MenuState.EULA] = EULALogicProvider;

            
            foreach (var curKvp in _logicProvidersByState)
            {
                curKvp.Value.OnStart();
            }

            _curLogicProvider = _logicProvidersByState[(MenuState)DefaultSceneState];
            SetSceneState(DefaultSceneState);
            DefaultSceneState = (int)GetDefaultSceneState(); //reset this to the default value. Callers can set DefaultSceneState, but the valueis reset each time we reach OnStart again

            //_gameEngineInterface.LogToConsole("MenuLogHandler.Start()");

            //_curLogicProvider.LogToDebugOutput("OnStart()");

            //_curLogicProvider.LogToDebugOutput("Bundle Version Code: " + _gameEngineInterface.AppVersion); //todo real game: make this compatible with iOS too

            _gameEngineInterface.AppStoreService.LogToDebugOutput = LogToDebugOutput;
        }

        private void LogToDebugOutput(string msg)
        {
            _curLogicProvider.LogToDebugOutput(msg);
        }

        public void OnUpdate()
        {
            _curLogicProvider.UpdateInputStates();
            _curLogicProvider.HandleInput();
            _curLogicProvider.UpdateGameObjects();
        }

        public override void SetAppState(AppState appState, int? sceneState = null)
        {
            _gameController.SetAppState(appState, sceneState);
        }

        //todo v2: move SetSceneState into base class. This also means you have to move _curLogicProvider and _logicProvidersByGameState into the base class, and make _logicProvidersByGameState have an int key
        public override void SetSceneState(int sceneState)
        {
            MenuState menuState = (MenuState)sceneState;

            var prevLogicProvider = _curLogicProvider;
            _curLogicProvider = _logicProvidersByState[menuState];

            if (prevLogicProvider != null)
            {
                prevLogicProvider.ClearInputStates(); //we don't want old input states to still be active when we switch back to the original logic provider
                prevLogicProvider.OnDeActivate();
            }

            _curLogicProvider.OnActivate();
        }
    }
}