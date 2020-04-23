using System;
using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.MenuLogicProviders
{
    public class LoadingLogicProvider : BaseLogicProvider
    {
        private const float TIME_DIFF_DOT_CHANGE = 0.2f;

        private float _timeAccumulated = 0;
        private int _redDotIndex = 0;

        private IText _txtProgressDot0;
        private IText _txtProgressDot1;
        private IText _txtProgressDot2;

        private IGameObject _GOProgressDot0;
        private IGameObject _GOProgressDot1;
        private IGameObject _GOProgressDot2;

        private IGameObject _pnlLoading;
        private IAsyncOperation _asyncOpLoading;

        protected IGameController _gameController;

        public LoadingLogicProvider(IGameController gameController, ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {
            _gameController = gameController;
        }

        public override void OnStart()
        {
            base.OnStart();

            _timeAccumulated = 0;

            _GOProgressDot0 = _gameEngineInterface.FindGameObject("txtProgressDot0"); //todo v2: string constants
            _txtProgressDot0 = _GOProgressDot0.GetComponent<IText>();

            _GOProgressDot1 = _gameEngineInterface.FindGameObject("txtProgressDot1");
            _txtProgressDot1 = _GOProgressDot1.GetComponent<IText>();

            _GOProgressDot2 = _gameEngineInterface.FindGameObject("txtProgressDot2");
            _txtProgressDot2 = _GOProgressDot2.GetComponent<IText>();

            _pnlLoading = _gameEngineInterface.FindGameObject("pnlLoading");
            _pnlLoading.SetActive(false);
        }

        public override void OnActivate()
        {
            _pnlLoading.SetActive(true);
            _asyncOpLoading = _gameController.LoadSceneAsync(Constants.SceneNames.GameScene); //todo v2: Logic Providers should not reference IGameController. Here we should call ILogicHandler.SetAppState, which will call IGameController.LoadSceneAsync
        }

        public override void UpdateGameObjects()
        {
            if (_timeAccumulated >= TIME_DIFF_DOT_CHANGE)
            {
                _redDotIndex++;
                _redDotIndex %= 3; //3 is red dot count

                switch (_redDotIndex)
                {
                    case 0:
                        _txtProgressDot0.SetColor(1, 0, 0);
                        _txtProgressDot2.SetColor(1, 1, 1);
                        break;
                    case 1:
                        _txtProgressDot1.SetColor(1, 0, 0);
                        _txtProgressDot0.SetColor(1, 1, 1);
                        break;
                    case 2:
                        _txtProgressDot2.SetColor(1, 0, 0);
                        _txtProgressDot1.SetColor(1, 1, 1);
                        break;
                }

                _timeAccumulated -= TIME_DIFF_DOT_CHANGE; //don't reset to zero, or else you'll lose some leftover time

            }
            else
            {
                _timeAccumulated += _gameEngineInterface.Time.DeltaTime;
            }
        }
    }
}