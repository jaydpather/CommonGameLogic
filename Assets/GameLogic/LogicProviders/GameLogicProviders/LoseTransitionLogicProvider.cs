using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.GameLogicProviders
{
    public class LoseTransitionLogicProvider : BaseLogicProvider
    {
        private IAudioSource _audioSourceExplosion;
        private IGameObject _sourceExplosion;
        private IGameObject _playerShip;
        private DateTime _explosionSoundEndedTime = DateTime.MinValue;

        public LoseTransitionLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {
        }

        public override void OnStart()
        {
            base.OnStart();
   
            _sourceExplosion = _gameEngineInterface.FindGameObject("Explosion");
            _playerShip = _gameEngineInterface.FindGameObject("PlayerShip");
            //_audioSourceLoseTransition = _gameEngineInterface.FindGameObject("LoseTransitionSound").GetComponent<IAudioSource>();
        }

        public override void OnActivate()
        {
            var newExplosion = _gameEngineInterface.Clone(_sourceExplosion, string.Empty);
            newExplosion.Transform.Position = _playerShip.Transform.Position;
            newExplosion.Transform.Scale(1.1f, 1.1f, 1.1f);
            newExplosion.SetActive(true);

            //_audioSourceLoseTransition.Play();

            _audioSourceExplosion = newExplosion.GetComponent<IAudioSource>(); //this audio source has PlayOnAwake = true

            _playerShip.SetActive(false);
        }

        public override void UpdateGameObjects()
        {
            if (_explosionSoundEndedTime == DateTime.MinValue && !_audioSourceExplosion.IsPlaying) //audio source started playing in OnActivate
            {
                _explosionSoundEndedTime = DateTime.Now;   
            }
            else if(!_audioSourceExplosion.IsPlaying)
            {
                var timeSinceExplosionEnded = DateTime.Now - _explosionSoundEndedTime;

                if(timeSinceExplosionEnded.TotalSeconds > 1.1)
                {
                    _logicHandler.SetSceneState((int)GameState.Lose);
                }
            }
        }
    }
}