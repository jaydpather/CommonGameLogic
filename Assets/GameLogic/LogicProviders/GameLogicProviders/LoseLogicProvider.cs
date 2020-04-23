using System;
using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders
{
    public class LoseLogicProvider : BaseLogicProvider
    {
        private IGameObject _pnlGameOver;
        private IGameObject _pnlGameLose;
        private IText _txtCurrentLives;

        private IGameController _gameController;

        public LoseLogicProvider(IGameController gameController, ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {
            _gameController = gameController;
        }

        public override void OnStart()
        {
            base.OnStart();
            
            //LogToDebugOutput(_logicHandler.GlobalMessage);

            //todo 2nd game: this logic provider shouldn't have cases for both the GameOver and GameLose states. the whole point of logic providers is that each one handles only 1 state, so you don't need logic around what state you're in

            //Game Over:
            var btnGameOverMainMenu = _gameEngineInterface.FindGameObject("btnGameOverMainMenu");
            btnGameOverMainMenu.LogicHandler = _logicHandler;

            var btnGameOverGetMoreLives = _gameEngineInterface.FindGameObject("btnGameOverGetMoreLives");
            btnGameOverGetMoreLives.LogicHandler = _logicHandler;
            btnGameOverGetMoreLives.SetActive(false);

            _pnlGameOver = _gameEngineInterface.FindGameObject("pnlGameOver");
            _pnlGameOver.SetActive(false);


            //Game Lose:
            var btnGameLoseMainMenu = _gameEngineInterface.FindGameObject("btnGameLoseMainMenu");
            btnGameLoseMainMenu.LogicHandler = _logicHandler;

            var btnGameLoseTryAgain = _gameEngineInterface.FindGameObject("btnGameLoseTryAgain");
            btnGameLoseTryAgain.LogicHandler = _logicHandler;

            var txtCurrentLivesGameObject = _gameEngineInterface.FindGameObject("txtCurrentLives");
            _txtCurrentLives = txtCurrentLivesGameObject.GetComponent<IText>();

            _pnlGameLose = _gameEngineInterface.FindGameObject("pnlGameLose");
            _pnlGameLose.SetActive(false);
        }

        public override void OnActivate()
        {
            var numLivesRemaining = _dataLayer.GetNumLivesRemaining();

            _txtCurrentLives.Text = "REMAINING LIVES: " + numLivesRemaining;

            if (numLivesRemaining > 0)
                _pnlGameLose.SetActive(true);
            else
                _pnlGameOver.SetActive(true);

            _gameEngineInterface.AppStoreService.LogToDebugOutput = LogToDebugOutput;
        }

        public override void OnDeActivate()
        {
            _pnlGameLose.SetActive(false);
            _pnlGameOver.SetActive(false);
        }

        public override void OnClick(string sender)
        {
            switch (sender)
            {
                case "btnGameOverGetMoreLives":
                    _uiInputStates[UIInputAxis.btnGameOverGetMoreLives] = true;
                    break;
                case "btnGameOverMainMenu":
                    _uiInputStates[UIInputAxis.btnGameOverMainMenu] = true;
                    break;
                case "btnGameLoseMainMenu":
                    _uiInputStates[UIInputAxis.btnGameLoseMainMenu] = true;
                    break;
                case "btnGameLoseTryAgain":
                    _uiInputStates[UIInputAxis.btnGameLoseTryAgain] = true;
                    break;
            }
        }

        public override void HandleInput()
        {
            if (_uiInputStates[UIInputAxis.btnGameOverMainMenu])
            {
                _uiInputStates[UIInputAxis.btnGameOverMainMenu] = false;

                _logicHandler.SetAppState(AppState.MainMenu, (int)MenuState.GetMoreLives);
            }
            else if(_uiInputStates[UIInputAxis.btnGameOverGetMoreLives])
            {
                _uiInputStates[UIInputAxis.btnGameOverGetMoreLives] = false;
                _gameEngineInterface.AppStoreService.BuyProductByID(Constants.ProductNames.BuyLivesSmall);
                //todo: redirect to Get More Lives screen

                //todo real game: what should we do after the player buys lives? I think we should restart the level
            }
            else if (_uiInputStates[UIInputAxis.btnGameLoseMainMenu])
            {
                _uiInputStates[UIInputAxis.btnGameLoseMainMenu] = false;
                _logicHandler.SetAppState(AppState.MainMenu);
            }
            else if(_uiInputStates[UIInputAxis.btnGameLoseTryAgain])
            {
                _uiInputStates[UIInputAxis.btnGameLoseTryAgain] = false;
                _gameController.ShouldUpdate = false;
                _gameController.LoadSceneAsync(Constants.SceneNames.GameScene);
            }
        }
    }
}

