
using ThirdEyeSoftware.GameLogic.LogicProviders;

namespace ThirdEyeSoftware.GameLogic.LogicHandlers
{
    public interface ILogicHandler
    {
        void OnUpdate();
        void OnClick(string sender);
        void OnCollision(IComponent player, IGameObject cube);
        void SetSceneState(int sceneState);
        void SetAppState(AppState appState, int? sceneState = null);
        void OnStart();
        float GetAxisMultiplier(string axisName);
        float GetButtonState(string buttonName);
        ITouch GetTouch(int touchIndex);
        int DefaultSceneState { get; set; }

        IGameController GameController { get; set; }
        IGameEngineInterface GameEngineInterface { get; set; }

        string GlobalMessage { get; set; }

        void OnAdCompleted();
    }
}