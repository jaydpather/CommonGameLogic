using System;
using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders
{
    public class PauseLogicProvider : BaseLogicProvider
    {
        private IGameObject _pnlPauseMenu;
        private float _timeScaleBackup;

        public PauseLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {
            _pnlPauseMenu = null;
        }

        public override void OnStart()
        {
            base.OnStart();

            var btnResumeGame = _gameEngineInterface.FindGameObject("btnResumeGame");
            btnResumeGame.LogicHandler = _logicHandler;

            var btnQuitGame = _gameEngineInterface.FindGameObject("btnQuitGame");
            btnQuitGame.LogicHandler = _logicHandler;

            var btnHome = _gameEngineInterface.FindGameObject("btnHome");
            btnHome.LogicHandler = _logicHandler;

            _pnlPauseMenu = _gameEngineInterface.FindGameObject("pnlPauseMenu");
            _pnlPauseMenu.SetActive(false);
        }

        public override void OnActivate()
        {
            _timeScaleBackup = _gameEngineInterface.TimeScale;
            _gameEngineInterface.TimeScale = 0f;

            _pnlPauseMenu.SetActive(true);
        }

        public override void OnDeActivate()
        {
            if (_pnlPauseMenu != null) //if we re-enter the game state after exiting to the main menu, it's possible that this method will get called before OnStart() has been called, meaning _pnlPauseMenu will be set to a disposed object
            {
                _pnlPauseMenu.SetActive(false);
                _gameEngineInterface.TimeScale = _timeScaleBackup;
            }
        }

        public override void HandleInput()
        {
            if (_inputStates[InputAxis.Cancel] != 0 || _uiInputStates[UIInputAxis.btnResumeGame])
            {
                _logicHandler.SetSceneState((int)GameState.InGame);
            }
            else if (_uiInputStates[UIInputAxis.btnQuitGame])
            {
                _logicHandler.SetAppState(AppState.MainMenu);
            }
        }

        public override void OnClick(string sender)
        {
            switch (sender)
            {
                case "btnResumeGame":
                    _uiInputStates[UIInputAxis.btnResumeGame] = true;
                    break;
                case "btnQuitGame":
                    _uiInputStates[UIInputAxis.btnQuitGame] = true;
                    break;
                case "btnHome":
                    btnHome_OnClick();
                    break;
            }
        }

        private void btnHome_OnClick()
        {
            _gameEngineInterface.MinimizeApp();
        }
    }
}