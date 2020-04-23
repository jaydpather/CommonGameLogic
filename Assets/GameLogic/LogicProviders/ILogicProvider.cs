
namespace ThirdEyeSoftware.GameLogic.LogicProviders
{
    public interface ILogicProvider
    {
        void HandleInput();
        void LogToDebugOutput(string msg);
        void OnActivate();
        void OnClick(string sender);
        void OnCollision(IComponent player, IGameObject cube);
        void OnDeActivate();
        void OnStart();
        void UpdateGameObjects();
        void UpdateInputStates();
        void ClearInputStates();
        void OnFocus(bool hasFocus);
    }
}