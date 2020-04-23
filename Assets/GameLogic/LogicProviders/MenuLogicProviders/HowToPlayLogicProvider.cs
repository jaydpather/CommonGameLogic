using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders;

namespace GameLogic.LogicProviders.MenuLogicProviders
{
    public class HowToPlayLogicProvider : BaseLogicProvider
    {
        private IGameObject _pnlHowToPlay;
        private IGameObject _btnHowToPlayOk;

        public HowToPlayLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer) : base(logicHandler, gameEngineInterface, dataLayer)
        {
        }

        public override void OnStart()
        {
            base.OnStart();

            _btnHowToPlayOk = _gameEngineInterface.FindGameObject("btnHowToPlayOk");
            _btnHowToPlayOk.LogicHandler = _logicHandler;

            _pnlHowToPlay = _gameEngineInterface.FindGameObject("pnlHowToPlay");
            _pnlHowToPlay.SetActive(false);
        }

        public override void OnActivate()
        {
            _pnlHowToPlay.SetActive(true);
        }

        public override void OnDeActivate()
        {
            _pnlHowToPlay.SetActive(false);
        }

        public override void OnClick(string sender)
        {
            switch (sender)
            {
                case "btnHowToPlayOk":
                    btnHowToPlayOk_Click();
                    break;
            }
        }

        private void btnHowToPlayOk_Click()
        {
            _logicHandler.SetSceneState((int)MenuState.InMenu);
        }
    }
}
