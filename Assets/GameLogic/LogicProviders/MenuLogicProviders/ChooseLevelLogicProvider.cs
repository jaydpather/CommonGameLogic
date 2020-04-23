using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.MenuLogicProviders
{
    public class ChooseLevelLogicProvider : BaseLogicProvider
    {
        private IGameObject _pnlChooseLevel;
        private IGameObject _btnPrevLevel;
        private IGameObject _btnNextLevel;
        private IText _txtSelectedLevel;
        private IText _txtLevelTitle;

        private int _selectedLevelIndex = 0;
        private int _maxLevelIndex = 0;
        private LevelInfo[] _levels = new LevelInfo[]
        {
            new LevelInfo(1, 1),
            new LevelInfo(1, 2),
            new LevelInfo(1, 3),

            new LevelInfo(2, 1),
            new LevelInfo(2, 2),
            new LevelInfo(2, 3),

            new LevelInfo(3, 1),
            new LevelInfo(3, 2),
            new LevelInfo(3, 3),

            new LevelInfo(4, 1),
            new LevelInfo(4, 2),
            new LevelInfo(4, 3),

            new LevelInfo(5, 1),
            new LevelInfo(5, 2),
            new LevelInfo(5, 3),
        };

        public ChooseLevelLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            :base(logicHandler, gameEngineInterface, dataLayer)
        {

        }

        public override void OnStart()
        {
            base.OnStart();
            
            var curLevel = _dataLayer.GetCurLevel();
            _selectedLevelIndex = findLevelIndex(curLevel);
            _maxLevelIndex = _selectedLevelIndex; //user cannot select a level they haven't beaten

            var goSelectedLevel = _gameEngineInterface.FindGameObject("txtSelectedLevel");
            _txtSelectedLevel = goSelectedLevel.GetComponent<IText>();

            var goLevelTitle = _gameEngineInterface.FindGameObject("txtLevelTitle");
            _txtLevelTitle = goLevelTitle.GetComponent<IText>();

            _txtSelectedLevel.Text = _levels[_selectedLevelIndex].DisplayValue;
            _txtLevelTitle.Text = _levels[_selectedLevelIndex].Title;

           
            var btnStart = _gameEngineInterface.FindGameObject("btnChooseLevelStart");
            btnStart.LogicHandler = _logicHandler;

            var btnCancel = _gameEngineInterface.FindGameObject("btnChooseLevelCancel");
            btnCancel.LogicHandler = _logicHandler;

            _btnNextLevel = _gameEngineInterface.FindGameObject("btnNextLevel");
            _btnNextLevel.LogicHandler = _logicHandler;

            _btnPrevLevel = _gameEngineInterface.FindGameObject("btnPrevLevel");
            _btnPrevLevel.LogicHandler = _logicHandler;

            if (_selectedLevelIndex == 0)
            { 
                _btnPrevLevel.SetActive(false);
            }

            _btnNextLevel.SetActive(false); //btnNextLevel will always be disabled on start, because we are defaulting to the max level they can play right now. (They have to beat another level before they can select a higher level

            _pnlChooseLevel = _gameEngineInterface.FindGameObject("pnlChooseLevel");
        }

        private int findLevelIndex(LevelInfo curLevel)
        {
            var retVal = -1;

            for(var i=0; i < _levels.Length; i++)
            {
                if(curLevel.Level == _levels[i].Level && curLevel.SubLevel == _levels[i].SubLevel)
                {
                    retVal = i;
                    break;
                }
            }

            return retVal;
        }

        public override void OnActivate()
        {
            _pnlChooseLevel.SetActive(true);
        }

        public override void OnDeActivate()
        {
            _pnlChooseLevel.SetActive(false);
        }

        public override void OnClick(string sender)
        {
            //todo: dictionary instead of switch
            switch(sender)
            {
                case "btnChooseLevelStart":
                    btnChooseLevelStart_Click();
                    break;
                case "btnChooseLevelCancel":
                    btnChooseLevelCancel_Click();
                    break;
                case "btnNextLevel":
                    btnNextLevel_Click();
                    break;
                case "btnPrevLevel":
                    btnPrevLevel_Click();
                    break;
            }
        }

        private void btnNextLevel_Click()
        {
            _selectedLevelIndex++;
            _txtSelectedLevel.Text = _levels[_selectedLevelIndex].DisplayValue;
            _txtLevelTitle.Text = _levels[_selectedLevelIndex].Title;

            _btnPrevLevel.SetActive(true); //if the user just clicked Next level, then we can't be on Min level anymore

            if(_selectedLevelIndex == _maxLevelIndex)
            {
                _btnNextLevel.SetActive(false);
            }
        }

        private void btnPrevLevel_Click()
        {
            _selectedLevelIndex--;
            _txtSelectedLevel.Text = _levels[_selectedLevelIndex].DisplayValue;
            _txtLevelTitle.Text = _levels[_selectedLevelIndex].Title;

            _btnNextLevel.SetActive(true); //if the user just clicked Prev Level, then we can't be on Max Level anymore
            if(_selectedLevelIndex == 0)
            {
                _btnPrevLevel.SetActive(false);
            }
        }

        private void btnChooseLevelStart_Click()
        {
            _logicHandler.GameController.CurLevel.Level = _levels[_selectedLevelIndex].Level;
            _logicHandler.GameController.CurLevel.SubLevel = _levels[_selectedLevelIndex].SubLevel;

            _logicHandler.SetSceneState((int)MenuState.Loading);
        }

        private void btnChooseLevelCancel_Click()
        {
            _logicHandler.SetSceneState((int)MenuState.InMenu);
        }
    }
}
