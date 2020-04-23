using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using ThirdEyeSoftware.GameLogic.LogicProviders;

namespace GameLogic.LogicProviders.MenuLogicProviders
{
    public class EULALogicProvider : BaseLogicProvider
    {
        private IGameObject _pnlEULA;
        private Dictionary<string, Action> _buttonHandlers = new Dictionary<string, Action>();

        public EULALogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer) 
            : base(logicHandler, gameEngineInterface, dataLayer)
        {
            var btnEULAAccept = _gameEngineInterface.FindGameObject("btnEULAAccept");
            btnEULAAccept.LogicHandler = _logicHandler;

            var btnEULACancel = _gameEngineInterface.FindGameObject("btnEULACancel");
            btnEULACancel.LogicHandler = _logicHandler;

            var btnEULAPrompt = _gameEngineInterface.FindGameObject("btnEULAPrompt");
            btnEULAPrompt.LogicHandler = _logicHandler;

            var btnPrivacyPolicyPrompt = _gameEngineInterface.FindGameObject("btnPrivacyPolicyPrompt");
            btnPrivacyPolicyPrompt.LogicHandler = _logicHandler;

            _pnlEULA = _gameEngineInterface.FindGameObject("pnlEULA");

            if (_dataLayer.GetIsEULAAccepted())
            {
                _pnlEULA.SetActive(false);
            }

            _buttonHandlers.Add("btnEULAAccept", btnAccept_Click);
            _buttonHandlers.Add("btnEULACancel", btnCancel_Click);
            _buttonHandlers.Add("btnEULAPrompt", btnEULA_Click);
            _buttonHandlers.Add("btnPrivacyPolicyPrompt", btnPrivacyPolicy_Click);
        }

        public override void OnActivate()
        {
            _pnlEULA.SetActive(true);
        }

        public override void OnDeActivate()
        {
            _pnlEULA.SetActive(false);
        }

        public override void OnClick(string sender)
        {
            _buttonHandlers[sender]();
        }

        private void btnAccept_Click()
        {
            _dataLayer.SetIsEULAAccepted(true);

            _logicHandler.SetSceneState((int)MenuState.InMenu);
        }

        private void btnCancel_Click()
        {
            _gameEngineInterface.MinimizeApp();
        }

        private void btnEULA_Click()
        {
            _gameEngineInterface.OpenURL(Constants.URLs.EULA);
        }

        private void btnPrivacyPolicy_Click()
        {
            _gameEngineInterface.OpenURL(Constants.URLs.PrivacyPolicy);
        }
    }
}
