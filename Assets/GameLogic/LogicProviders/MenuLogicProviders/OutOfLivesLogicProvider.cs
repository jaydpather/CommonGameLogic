using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders;

namespace GameLogic.LogicProviders.MenuLogicProviders
{
    public class OutOfLivesLogicProvider : BaseLogicProvider
    {
        private IGameObject _pnlOutOfLives;
        private IGameObject _btnOutOfLivesOk;

        public OutOfLivesLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {
        }

        public override void OnStart()
        {
            base.OnStart();

            _btnOutOfLivesOk = _gameEngineInterface.FindGameObject("btnOutOfLivesOk");
            _btnOutOfLivesOk.LogicHandler = _logicHandler;

            _pnlOutOfLives = _gameEngineInterface.FindGameObject("pnlOutOfLives");
            _pnlOutOfLives.SetActive(false);
        }

        public override void OnActivate()
        {
            _pnlOutOfLives.SetActive(true);
        }

        public override void OnDeActivate()
        {
            _pnlOutOfLives.SetActive(false);
        }

        public override void OnClick(string sender)
        {
            switch(sender)
            {
                case "btnOutOfLivesOk":
                    btnOutOfLivesOk_Click();
                    break;
            }
        }

        public void btnOutOfLivesOk_Click()
        {
            _logicHandler.SetSceneState((int)MenuState.GetMoreLives);
        }
    }
}
