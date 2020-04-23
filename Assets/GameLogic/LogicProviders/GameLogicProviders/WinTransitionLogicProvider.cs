using System;
using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders
{
    public class WinTransitionLogicProvider : BaseLogicProvider
    {
        private IAudioSource _audioSourceWinTransition;

        public WinTransitionLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {

        }

        public override void OnStart()
        {
            base.OnStart();

            var winTransitionSound = _gameEngineInterface.FindGameObject("WinTransitionSound");
            _audioSourceWinTransition = winTransitionSound.GetComponent<IAudioSource>();
        }

        public override void OnActivate()
        {
            _audioSourceWinTransition.Play();
        }

        public override void UpdateGameObjects()
        {
            if (!_audioSourceWinTransition.IsPlaying)
            {
                _logicHandler.SetSceneState((int)GameState.Win);
            }
        }
    }
}