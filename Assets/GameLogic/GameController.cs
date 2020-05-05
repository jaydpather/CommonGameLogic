using System;
using System.Collections;
using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.StoreLogicService;

//todo v2: move GameLogic project above Assets folder, like GameLogicTest. (This prevents Unity from including the GameLogic source files in the TestGame project).
namespace ThirdEyeSoftware.GameLogic
{
    public interface IGameController
    {
        IAsyncOperation LoadSceneAsync(string sceneName);
        IDataLayer DataLayer { get; }
        void SetAppState(AppState appState, int? sceneState = null);
        void OnStart(IGameEngineInterface gameEngineInterface, IDataLayer dataLayer, IStoreLogicService storeLogicService);
        void OnUpdate();
        bool ShouldUpdate { get; set; } //we need to prevent OnUpdate() from doing anything while loading scenes. This will get set to true again by the Logic Handler's OnStart() method, which is called by an Event Handler class in the Unity project
        LevelInfo CurLevel { get; set; }
        ScoreInfo ScoreInfo { get; set; }
        void ProgressToNextLevel();
        bool JustBeatMaxLevel { get; }
        bool JustSavedLatestLevel { get; set; }
    }

    public class ScoreInfo
    {
        public int LevelBonus { get; set; }
        public int SubLevelBonus { get; set; }
        public int TimeBonus { get; set; }
        public int TotalScore { get; set; }
    }

    public class LevelInfo
    {
        private static string[] _levelTitles = new string[]
        {
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
        };

        public int Level { get; set; }
        public int SubLevel { get; set; }
        public string Title
        {
            get
            {
                return _levelTitles[Level];
            }
        }

        public string DisplayValue
        {
            get
            {
                return string.Format("{0}-{1}", Level, SubLevel);
            }
        }

        public LevelInfo(int level, int subLevel)
        {
            Level = level;
            SubLevel = subLevel;
        }

        public static bool operator> (LevelInfo a, LevelInfo b)
        {
            bool retVal;

            if(a.Level > b.Level)
            {
                retVal = true;
            }
            else if(a.Level == b.Level)
            {
                retVal = a.SubLevel > b.SubLevel;
            }
            else //a.Level < b.Level
            {
                retVal = false;
            }

            return retVal;
        }

        public static bool operator <(LevelInfo a, LevelInfo b)
        {
            bool retVal;

            if (a.Level < b.Level)
            {
                retVal = true;
            }
            else if (a.Level == b.Level)
            {
                retVal = a.SubLevel < b.SubLevel;
            }
            else //a.Level > b.Level
            {
                retVal = false;
            }

            return retVal;
        }
    }

    public class GameController : IGameController
    {
        private static GameController _instance;

        private ILogicHandler _curLogicHandler;
        private Dictionary<AppState, ILogicHandler> _logicHandlersByAppState = new Dictionary<AppState, ILogicHandler>();
        private AppState _curAppState;
        public IGameEngineInterface GameEngineInterface { get; set; }
        public IDataLayer DataLayer { get; set; }
        private ILogicHandler _gameLogicHandler;
        private ILogicHandler _menuLogicHandler;
        public bool ShouldUpdate { get; set; }


        public static GameController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameController();
                }
                return _instance;
            }
        }

        public ScoreInfo ScoreInfo { get; set; }

        public LevelInfo CurLevel
        {
            get;
            set;
        }

        public ILogicHandler MenuLogicHandler
        {
            get
            {
                if(_menuLogicHandler == null)
                {
                    _menuLogicHandler = ThirdEyeSoftware.GameLogic.LogicHandlers.MenuLogicHandler.Instance;
                }

                return _menuLogicHandler;
            }

            set
            {
                _menuLogicHandler = value;
            }
        }
        
        public ILogicHandler GameLogicHandler
        {
            get
            {
                if(_gameLogicHandler == null)
                {
                    _gameLogicHandler = ThirdEyeSoftware.GameLogic.LogicHandlers.GameLogicHandler.Instance;
                }

                return _gameLogicHandler;
            }

            set
            {
                _gameLogicHandler = value;
            }
        }

        private GameController()
        {
            _curLogicHandler = MenuLogicHandler;

            _logicHandlersByAppState.Add(AppState.Game, GameLogicHandler);
            _logicHandlersByAppState.Add(AppState.MainMenu, MenuLogicHandler);

            ScoreInfo = new ScoreInfo();

            ShouldUpdate = true;
        }

        private bool IsAtMaxLevel
        {
            get
            {
                return CurLevel.Level == Constants.Levels.NumLevels && CurLevel.SubLevel == Constants.Levels.NumSubLevels;
            }
        }

        public bool JustBeatMaxLevel
        {
            get;
            private set;
        }

        public bool JustSavedLatestLevel
        {
            get;
            set;
        }

        public void ProgressToNextLevel()
        {
            if (CurLevel.SubLevel < Constants.Levels.NumSubLevels)
            {
                CurLevel.SubLevel++;
            }
            else
            {
                if(IsAtMaxLevel)
                {
                    JustBeatMaxLevel = true;
                }
                else
                {
                    CurLevel.Level++;
                    CurLevel.SubLevel = 1;
                }
            }

            if(CurLevel > DataLayer.GetCurLevel()) //this condition prevents us from saving in the case that the player is going back to play an earlier level just to warm up (saving in that case would overwrite the player's real progress)
            {
                JustSavedLatestLevel = true;
                DataLayer.SaveCurLevel(CurLevel);
            }
            
        }

        public void OnStart(IGameEngineInterface gameEngineInterface, IDataLayer dataLayer, IStoreLogicService storeLogicService)
        {
            GameEngineInterface = gameEngineInterface;
            DataLayer = dataLayer;

            MenuLogicHandler.GameController = this;
            GameLogicHandler.GameController = this;

            MenuLogicHandler.GameEngineInterface = gameEngineInterface;
            GameLogicHandler.GameEngineInterface = gameEngineInterface;

            GameEngineInterface.AppStoreService.OnPurchaseSucceededEventHandler = storeLogicService.OnAppStorePurchaseSucceeded;

            GameEngineInterface.VSyncCount = 1;

            storeLogicService.DataLayer = dataLayer;

            if(CurLevel == null) //CurLevel will only be null the very first time the app is loaded. 
            {
                CurLevel = dataLayer.GetCurLevel(); //the player's current level is the default value for the Choose Level screen
            }

            //GameEngineInterface.LogToConsole(string.Format("screen size: {0} x {1} px", GameEngineInterface.Screen.Width, GameEngineInterface.Screen.Height));
        }

        public void OnUpdate()
        {
            if(ShouldUpdate)
            {
                _curLogicHandler.OnUpdate();
            }
        }

        public void SetAppState(AppState appState, int? sceneState = null)
        {
            _curAppState = appState;
            _curLogicHandler = _logicHandlersByAppState[appState];

            if (sceneState.HasValue)
            {
                MenuLogicHandler.DefaultSceneState = sceneState.Value;
            }

            if (appState == AppState.MainMenu)
            {
                ShouldUpdate = false; //we don't want to call _curLogicHandler.OnUpdate() until we've called OnStart() on the new current logic handler.
                GameEngineInterface.LoadScene(Constants.SceneNames.UIScene);
            }
        }

        //todo 2nd game: why do we need both LoadSceneAsync and SetAppState?
        public IAsyncOperation LoadSceneAsync(string sceneName)
        {
            JustBeatMaxLevel = false; //each time we start the level, we know the player hasn't just beaten the last level. This is necessary for the case of a player beating the last level and then going back to play another level

            var retVal = GameEngineInterface.LoadSceneAsync(Constants.SceneNames.GameScene); //todo: this should use the scene name from the parameter, not a hardcoded scene name

            if (retVal != null)
            {
                retVal.AllowSceneActivation = true;
            }
            //todo Post-UT: what if retVal is null? We need to log some kind of exception in that case.

            return retVal;
        }
    }
}