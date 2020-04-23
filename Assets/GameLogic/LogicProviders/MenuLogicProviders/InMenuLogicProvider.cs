using GameLogic;
using System;
using System.Collections.Generic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders.MenuLogicProviders
{
    /*
CRASH REPORTING:
    * global catch-all in UnityGameEngineInterface
    * send exception message and stack trace by email
    * store email in an obscured way, so that people cannot just find the email with a hex editor
        * example:
            * insert junk characters between the letters of the real email
            * compute the space between each character, using fibonnaci sequence
            * email stored as: "JaKaPbbb.cccccTddddddddHeeeeeeeeeeeeeI...
                * except without capitals
    
    * Unity forum post to see if this is the best practice
        * does Android automatically report crashes
        * how to cause a crash by throwing an exception?
        * 

     */

    public class InMenuLogicProvider : BaseLogicProvider
    {
        private IGameObject _pnlMainMenu;
        private IGameObject _pnlPurchaseSucceeded;
        private IGameObject _pnlPurchaseFailed;
        private IGameObject _pnlOutOfLives;
        private IGameObject _pnlChooseLevel;
        private IGameObject _btnGetMoreLives;
        private IText _txtVersionNumber;

        public IGameObject btnGetMoreLives //used for UTs only
        {
            get
            {
                return _btnGetMoreLives;
            }

            set
            {
                _btnGetMoreLives = value;
            }
        }

        private ILogicHandler _menuLogicHandler;
        public ILogicHandler MenuLogicHandler //todo Post-UT: we should replace all references to this with _logicHandler, right?
        {
            get
            {
                if(_menuLogicHandler == null)
                {
                    return ThirdEyeSoftware.GameLogic.LogicHandlers.MenuLogicHandler.Instance;
                }

                return _menuLogicHandler;
            }

            set
            {
                _menuLogicHandler = value;
            }
        }

        public InMenuLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
            : base(logicHandler, gameEngineInterface, dataLayer)
        {
        }

        public override void OnStart()
        {
            base.OnStart();
            ClearDebugOutput();
            //_dataLayer.ClearAllAndSave();
            
            var btnStartGame = _gameEngineInterface.FindGameObject("btnStartGame");
            btnStartGame.LogicHandler = MenuLogicHandler;

            var btnHowToPlay = _gameEngineInterface.FindGameObject("btnHowToPlay");
            btnHowToPlay.LogicHandler = MenuLogicHandler;

            var btnShare = _gameEngineInterface.FindGameObject("btnShare");
            btnShare.LogicHandler = MenuLogicHandler;

            _menuUIInputStates[MenuUIInputAxis.btnGetMoreLives] = false;
            _btnGetMoreLives = _gameEngineInterface.FindGameObject("btnGetMoreLives");
            _btnGetMoreLives.LogicHandler = MenuLogicHandler;

            var btnExitGame = _gameEngineInterface.FindGameObject("btnExitGame");
            btnExitGame.LogicHandler = MenuLogicHandler;

            var btnPrivacyPolicy = _gameEngineInterface.FindGameObject("btnPrivacyPolicy");
            btnPrivacyPolicy.LogicHandler = MenuLogicHandler;

            var btnEULA = _gameEngineInterface.FindGameObject("btnEULA");
            btnEULA.LogicHandler = MenuLogicHandler;

            var txtVersionNumberGameObject = _gameEngineInterface.FindGameObject("txtVersionNumber");
            _txtVersionNumber = txtVersionNumberGameObject.GetComponent<IText>();
            _txtVersionNumber.Text = "v" + _gameEngineInterface.AppVersion;

            //todo v2: string constants for all button names
            _menuUIInputStates[MenuUIInputAxis.btnPurchaseSucceededOk] = false;
            var btnPurchaseSucceeded = _gameEngineInterface.FindGameObject("btnStorePurchaseSucceededOk");
            btnPurchaseSucceeded.LogicHandler = MenuLogicHandler;

            _menuUIInputStates[MenuUIInputAxis.btnPurchaseFailedOk] = false;
            var btnPurchaseFailed = _gameEngineInterface.FindGameObject("btnStorePurchaseFailedOk");
            btnPurchaseFailed.LogicHandler = MenuLogicHandler;

            _pnlMainMenu = _gameEngineInterface.FindGameObject("pnlMainMenu");
            _pnlMainMenu.SetActive(false);

            _pnlPurchaseSucceeded = _gameEngineInterface.FindGameObject("pnlStorePurchaseSucceeded");
            _pnlPurchaseSucceeded.SetActive(false);

            _pnlPurchaseFailed = _gameEngineInterface.FindGameObject("pnlStorePurchaseFailed");
            _pnlPurchaseFailed.SetActive(false);

            _pnlChooseLevel = _gameEngineInterface.FindGameObject("pnlChooseLevel");
            _pnlChooseLevel.SetActive(false);

            _menuUIInputStates[MenuUIInputAxis.btnStartGame] = false;
            //_InputStates[InputAxis.Quit] = 0; //todo: remove this input axis. it was added as a KB shortcut in the Unity player

            //todo Post-UT: do you need this line? TimeScale logic has been moved to pause logic provider
            _gameEngineInterface.TimeScale = 1; //TimeScale was set to 0 when GameLogicProvider deactivated

            _gameEngineInterface.AppStoreService.OnPurchaseFailedEventHandler = OnAppStorePurchaseFailed;
        }

        public override void OnActivate()
        {

            _pnlMainMenu.SetActive(true);

            //LogToDebugOutput("test msg");

            //_unityGameEngineInterface is re-instantiated when we switch scenes, so we have to reset these delegates here. Otherwise, purchases won't work if you switch from the game back to UI and then try to purchase.
            //todo: why are these lines commented? the comment above says we need them
            //_gameEngineInterface.AppStoreService.OnPurchaseFailedEventHandler = OnAppStorePurchaseFailed;
            //_gameEngineInterface.AppStoreService.OnPurchaseSucceededEventHandler = OnAppStorePurchaseSucceeded;
        }

        public override void OnDeActivate()
        {
            _pnlMainMenu.SetActive(false);
        }

        public override void OnClick(string sender)
        {
            ClearDebugOutput(); //if there's any debug output, we got it from the previous event (previous button click). Now we want to clear the output, because hopefully the current event will succeed. (Or, if it fails, we want to clear some room for the current error message).

            switch (sender)
            {
                case "btnStartGame": //todo v2: string constants
                    _menuUIInputStates[MenuUIInputAxis.btnStartGame] = true;
                    break;
                case "btnGetMoreLives":
                    _menuUIInputStates[MenuUIInputAxis.btnGetMoreLives] = true;
                    break;
                case "btnStorePurchaseSucceededOk":
                    _menuUIInputStates[MenuUIInputAxis.btnPurchaseSucceededOk] = true;
                    break;
                case "btnStorePurchaseFailedOk":
                    _menuUIInputStates[MenuUIInputAxis.btnPurchaseFailedOk] = true;
                    break;
                case "btnExitGame":
                    btnExitGame_Click();
                    break;
                case "btnPrivacyPolicy":
                    btnPrivacyPolicy_Click();
                    break;
                case "btnHowToPlay":
                    btnHowToPlay_Click();
                    break;
                case "btnShare":
                    btnShare_Click();
                    break;
                case "btnEULA":
                    btnEULA_Click();
                    break;
            }
        }

        public void btnEULA_Click()
        {
            _gameEngineInterface.OpenURL(Constants.URLs.EULA);
        }

        public void btnShare_Click()
        {
            _logicHandler.GameEngineInterface.OpenShareDialog(Constants.URLs.Share);
        }

        public void btnHowToPlay_Click()
        {
            _logicHandler.SetSceneState((int)MenuState.HowToPlay);
        }

        public void btnPrivacyPolicy_Click()
        {
            _gameEngineInterface.OpenURL(Constants.URLs.PrivacyPolicy);
        }

        public void btnExitGame_Click()
        {
            //don't actually exit the game, just minimize.
            //why? b/c Unity has a known bug where purchases don't work after you exit and restart the app
            //see UnityGameEngineInterface.ExitApp() for more info
            _gameEngineInterface.MinimizeApp();
        }

        private void OnAppStorePurchaseFailed()
        {
            //Google Play handles all the success/failure dialogs for us. We don't need to display anything here.
            //_pnlPurchaseFailed.SetActive(true);
            //_pnlMainMenu.SetActive(false);
        }

        //todo v2: remove. overriden method just calls base class
        public override void UpdateInputStates()
        {
            base.UpdateInputStates();
        }

        private bool DoesPlayerHaveLives()
        {
            var numLives = _dataLayer.GetNumLivesRemaining();

            return (numLives > 0);
        }

        public override void HandleInput()
        {
            //todo Post-UT: refactor into 1 button handler method for each if-else case
            //todo v2: add/override a ClearInputStates method instead of setting input states to false in each if-block
            if (_menuUIInputStates[MenuUIInputAxis.btnStartGame])
            {
                _menuUIInputStates[MenuUIInputAxis.btnStartGame] = false;

                if (DoesPlayerHaveLives())
                    MenuLogicHandler.SetSceneState((int)MenuState.ChooseLevel);
                else
                    MenuLogicHandler.SetSceneState((int)MenuState.OutOfLives);
            }
            else if (_menuUIInputStates[MenuUIInputAxis.btnGetMoreLives])
            {
                _menuUIInputStates[MenuUIInputAxis.btnGetMoreLives] = false;

                MenuLogicHandler.SetSceneState((int)MenuState.GetMoreLives);

            }
            else if (_menuUIInputStates[MenuUIInputAxis.btnPurchaseSucceededOk])
            {
                //todo Post-UT: is this button still used?
                _menuUIInputStates[MenuUIInputAxis.btnPurchaseSucceededOk] = false;

                _pnlMainMenu.SetActive(true);
                _pnlPurchaseSucceeded.SetActive(false);
            }
            else if (_menuUIInputStates[MenuUIInputAxis.btnPurchaseFailedOk])
            {
                //todo Post-UT: is this button still used?
                _menuUIInputStates[MenuUIInputAxis.btnPurchaseFailedOk] = false;

                _pnlMainMenu.SetActive(true);
                _pnlPurchaseFailed.SetActive(false);
            }
        }
    }
}