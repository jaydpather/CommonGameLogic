using System;
using System.Collections.Generic;
using System.Globalization;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders
{
    public class WinLogicProvider : BaseLogicProvider
    {
        private IGameObject _pnlGameWin;
        private IGameObject _btnGameWinNextLevel;
        private IText _txtWinMessage;
        private IText _txtLevelBonus;
        private IText _txtSubLevelBonus;
        private IText _txtTimeBonus;
        private IText _txtTotalScore;

        public WinLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {

        }

        public override void OnStart()
        {
            var btnGameWinOk = _gameEngineInterface.FindGameObject("btnGameWinOk");
            btnGameWinOk.LogicHandler = _logicHandler;

            _btnGameWinNextLevel = _gameEngineInterface.FindGameObject("btnGameWinNextLevel");
            _btnGameWinNextLevel.LogicHandler = _logicHandler;

            var txtWinMessageGameObject = _gameEngineInterface.FindGameObject("txtWinMessage");
            _txtWinMessage = txtWinMessageGameObject.GetComponent<IText>();

            var gameObject = _gameEngineInterface.FindGameObject("txtLevelBonus");
            _txtLevelBonus = gameObject.GetComponent<IText>();

            gameObject = _gameEngineInterface.FindGameObject("txtSubLevelBonus");
            _txtSubLevelBonus = gameObject.GetComponent<IText>();

            gameObject = _gameEngineInterface.FindGameObject("txtTimeBonus");
            _txtTimeBonus = gameObject.GetComponent<IText>();

            gameObject = _gameEngineInterface.FindGameObject("txtTotalScore");
            _txtTotalScore = gameObject.GetComponent<IText>();

            _pnlGameWin = _gameEngineInterface.FindGameObject("pnlGameWin");
            _pnlGameWin.SetActive(false);
        }

        public override void OnActivate()
        {
            var scoreInfo = _logicHandler.GameController.ScoreInfo;

            var cultureInfo = new CultureInfo("en-US", false).NumberFormat;
            _txtLevelBonus.Text = scoreInfo.LevelBonus.ToString("N0", cultureInfo);
            _txtSubLevelBonus.Text = scoreInfo.SubLevelBonus.ToString("N0", cultureInfo);
            _txtTimeBonus.Text = scoreInfo.TimeBonus.ToString("N0", cultureInfo);
            _txtTotalScore.Text = scoreInfo.TotalScore.ToString("N0", cultureInfo);

            if(_logicHandler.GameController.JustBeatMaxLevel)
            {
                //player just beat the entire game (because we're in the Win state, and we're at the max level)
                _txtWinMessage.Text = Constants.UIMessages.BeatGame;
                _btnGameWinNextLevel.SetActive(false);
            }
            else
            {
                //player just beat a regular level
                _txtWinMessage.Text = Constants.UIMessages.BeatLevel;

                if(_logicHandler.GameController.JustSavedLatestLevel)
                {
                    _txtWinMessage.Text += Environment.NewLine + Constants.UIMessages.ProgressSaved;
                    _logicHandler.GameController.JustSavedLatestLevel = false; //reset this flag to false, b/c we've just shown the user the "progress saved" message. (if we don't reset this to false, we'll show the "progress saved" message even if the user goes back to the main men and then beats a lower level.
                }

                _btnGameWinNextLevel.SetActive(true); //player could have beaten the game, then come back to play a lower level
            }

            _pnlGameWin.SetActive(true);
        }

        public override void HandleInput()
        {
            if (_uiInputStates[UIInputAxis.btnGameWinOk])
            {
                _uiInputStates[UIInputAxis.btnGameWinOk] = false;
                _logicHandler.SetAppState(AppState.MainMenu);
            }
        }

        //todo: public only for UT
        public void btnGameWinNextLevel_Click()
        {
            _logicHandler.GameController.LoadSceneAsync(Constants.SceneNames.GameScene);
        }

        public override void OnClick(string sender)
        {
            switch (sender)
            {
                case "btnGameWinOk":
                    _uiInputStates[UIInputAxis.btnGameWinOk] = true; //todo: convert to btnGameWinOk_Click()
                    break;
                case "btnGameWinNextLevel":
                    btnGameWinNextLevel_Click(); //todo 2nd game: use Substitute.ForPartsOf so you can check if we received a call
                    break;
            }
        }
    }
}
