using System;
using System.Collections;
using ThirdEyeSoftware.GameLogic.LogicHandlers;
using GameLogic.Utils;
using ThirdEyeSoftware.GameLogic.StoreLogicService;
using System.Collections.Generic;

namespace ThirdEyeSoftware.GameLogic
{
    public interface IDataLayer
    {
        bool GetBool(string key);
        void SetBool(string value, bool key);
        void Save();
        void ClearAllAndSave();
        int GetNumLivesRemaining();
        void DecrementNumLivesRemaining(int numLivesToRemove);
        void IncrementNumLivesRemaining(int numLivesToAdd);
        LevelInfo GetCurLevel();
        void SaveCurLevel(LevelInfo levelInfo);
        bool GetIsEULAAccepted();
        void SetIsEULAAccepted(bool value);
    }

    public interface IGameEngineInterface
    {
        string AppVersion { get; }
        IAdvertisement Advertisement { get; set; }
        IScreen Screen { get; set; }
        IInput Input { get; }
        ITime Time { get; set; }
        float TimeScale { get; set; }
        IAppStoreService AppStoreService { get; set; }
        IScreenUtils ScreenUtils { get; set; }
        int VSyncCount { set; }

        void LogToConsole(string msg);
        void LoadScene(string sceneName);
        IAsyncOperation LoadSceneAsync(string sceneName);
        IGameObject FindGameObject(string name);
        T[] FindObjectsOfType<T>();
        void ClearGameObjectCache();
        void CopyToClipBoard(string value);
        void TranslateCamera(float x, float y, float z);
        IGameObject Clone(IGameObject gameObject, string nameSuffix);
        void SetupLighting();
        void ExitApp();
        void OpenURL(string url);
        void OpenShareDialog(string msg);
        void SubmitScoreToLeaderboard(int score);
        void SubmitScoreToLeaderboard(int score, Action<bool> callback);
        void SetMainLightColor(float r, float g, float b);
        void SetNebulaColor(float r, float g, float b, float a);
        void MinimizeApp();
    }

    public interface IAsyncOperation
    {
        bool AllowSceneActivation { get; set; }
        float Progress { get; }
    }

    public interface IScreenUtils
    {
        IVector3 GetScreenPointFromWorldPoint(IVector3 worldPoint);
        IVector3 GetWorldPointFromScreenPoint(IVector3 screenPoint);
    }

    public interface IText
    {
        string Text { get; set; }
        void SetColor(float r, float g, float b);
    }

    public interface IAdvertisement
    {
        void Initialize();
        bool IsReady();
        void Show(Action onAdCompleted);
    }

    public interface IScreen
    {
        int Width { get; }
        int Height { get; }
    }

    public interface IInput
    {
        float GetAxis(string AxisName);
        bool GetButtonUp(string buttonName);
        ITouch GetTouch(int index);
        int TouchCount { get; }
    }

    public interface ITouch
    {
        IVector2 Position { get; }

    }

    public interface IVector2
    {
        float X { get; set; }
        float Y { get; set; }
    }

    public interface IVector3
    {
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }
    }

    public interface ITransform
    {
        float EulerAngleX { set; }
        void Translate(float x, float y, float z);
        IVector3 Position { get; set; }
        void Rotate(IVector3 rotations);
        void Scale(float x, float y, float z);
    }

    public interface ISphereBehavior
    {
        float Speed { get; set; }
        float Health { get; set; }
        IGameEngineInterface GameEngineInterface { get; set; }
    }

    public interface IPlayerShipScript
    {
        IGameEngineInterface GameEngineInterface { get; set; }
        int Health { get; set; }
        float HeadingAngle { get; set; }
        Vector3 ModelRotation { get; set; }
}

    public interface IAudioSource
    {
        void Play();
        void PlayOneShot();
        void UnPause();
        void Pause();
        bool IsPlaying { get; }
        float Length { get; }
        string Name { get; }
        float Time { set; }
    }

    public interface ITime
    {
        float DeltaTime { get; }
    }

    public interface IAppStoreService
    {
        bool IsInitialized { get; }
        void Initialize();
        bool BuyProductByID(string productId);
        Action OnInitializationFailedEventHandler { get; set; }
        Action OnPurchaseFailedEventHandler { get; set; }
        Action<string> OnPurchaseSucceededEventHandler { get; set; }
        Action<string> LogToDebugOutput { get; set; }
        Action<List<ProductInfo>> OnAppStoreInitialized { get; set; }
    }

    public interface IProductInfo
    {
        decimal Price { get; }
        string PriceStirng { get; }
        string Id { get; }
    }

    public interface IProductViewModel : IProductInfo
    {
        string SavePct { get; }
    }

    public interface IGameObject
    {
        ILogicHandler LogicHandler { get; set; }

        ITransform Transform { get; set; }

        string Name { get; set; }

        string SourceGameObjectName { get; set; }

        T GetComponent<T>() where T : class;

        //floats range from 0 - 1
        void SetMaterialColor(float r, float g, float b);

        //floats range from 0 - 1
        void SetMaterialColor(float r, float g, float b, float a);

        void SetActive(bool isActive);

        void EnableTextureWrapping();
        IVector3 GetSize();
    }

    public interface IComponent
    {
        ILogicHandler LogicHandler { get; set; }
    }
}