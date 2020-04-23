using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic;

namespace ThirdEyeSoftware.GameLogic.LogicHandlers
{
    public abstract class BaseLogicHandler : ILogicHandler
    {
        protected Dictionary<InputAxis, float> _InputStates = new Dictionary<InputAxis, float>();
        protected IGameController _gameController;
        protected IGameEngineInterface _gameEngineInterface;

        public virtual IGameController GameController
        {
            get
            {
                return _gameController;
            }
            set
            {
                _gameController = value;
            }
        }

        public IGameEngineInterface GameEngineInterface
        {
            get
            {
                return _gameEngineInterface;
            }
            set
            {
                _gameEngineInterface = value;
            }
        }

        public int DefaultSceneState
        {
            get;
            set;
        }

        public string GlobalMessage { get; set; }

        //todo v2: virtual
        public void OnUpdate()
        {
        }

        public virtual void OnAdCompleted()
        {

        }

        public float GetAxisMultiplier(string axisName)
        {
            float retVal = 0;
            float axisValue = _gameEngineInterface.Input.GetAxis(axisName);
            if (axisValue > 0)
            {
                retVal = 1;
            }
            else if (axisValue < 0)
            {
                retVal = -1;
            }

            return retVal;
        }

        public float GetButtonState(string buttonName)
        {
            return _gameEngineInterface.Input.GetButtonUp(buttonName) ? 1f : 0f;
        }

        public virtual void OnClick(string sender)
        {
            throw new System.NotImplementedException();
        }

        public void OnCollision(IComponent player, IGameObject cube)
        {
            throw new System.NotImplementedException();
        }

        public ITouch GetTouch(int touchIndex)
        {
            ITouch retVal = null;

            if (_gameEngineInterface.Input.TouchCount > touchIndex)
            {
                retVal = _gameEngineInterface.Input.GetTouch(touchIndex);
            }

            return retVal;
        }

        public virtual void SetSceneState(int sceneState)
        {

        }

        public virtual void SetAppState(AppState appState, int? sceneState = null)
        {

        }

        public virtual void OnStart()
        {
        }
    }
}