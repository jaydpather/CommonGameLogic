using System;
using System.Collections.Generic;
using System.Linq;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace ThirdEyeSoftware.GameLogic.LogicProviders
{
    abstract public class BaseLogicProvider : ILogicProvider
    {
        public enum UIInputAxis //this is to define input axes from UI input (e.g., button clicks), not from in-game input, like pressing arrow keys
        {
            //todo Post-UT: are these all referenced?
            btnResumeGame = 1,
            btnQuitGame = 2,
            btnGameOverMainMenu = 3,
            btnGameWinOk = 4,
            btnGameLoseMainMenu = 5,
            btnGameLoseTryAgain = 6,
            btnGameOverGetMoreLives = 7,
        }

        public enum MenuUIInputAxis //this is to define input axes from UI input (e.g., button clicks), not from in-game input, like pressing arrow keys
        {
            btnStartGame = 1,
            btnGetMoreLives = 2,
            btnPurchaseSucceededOk = 3,
            btnPurchaseFailedOk = 4,
        }

        protected Dictionary<MenuUIInputAxis, bool> _menuUIInputStates = new Dictionary<MenuUIInputAxis, bool>(); //this is to track input states from UI input (e.g., button clicks), not from in-game input, like pressing arrow keys

        protected Dictionary<InputAxis, float> _inputStates = new Dictionary<InputAxis, float>();
        protected Dictionary<UIInputAxis, bool> _uiInputStates = new Dictionary<UIInputAxis, bool>(); //todo v2: replace this dictionary with a dictionary of <string, bool>. each logic provider initializes the dictionary with only the button names they deal with
        protected ITouch _touch;
        protected ILogicHandler _logicHandler;
        private IText _txtDebugOutput;
        protected IGameEngineInterface _gameEngineInterface;
        protected IDataLayer _dataLayer;


        public BaseLogicProvider(ILogicHandler logicHandler, IGameEngineInterface gameEngineInterface, IDataLayer dataLayer)
        {
            _logicHandler = logicHandler;
            _gameEngineInterface = gameEngineInterface;
            _dataLayer = dataLayer;

            ClearInputStates();
        }

        public virtual void OnActivate()
        {

        }

        public virtual void OnDeActivate()
        {

        }

        public virtual void UpdateInputStates()
        {
            _inputStates[InputAxis.Cancel] = _logicHandler.GetButtonState(InputAxis.Cancel.ToString());

            _touch = _logicHandler.GetTouch(0);
        }

        public void ClearInputStates()
        {
            _touch = null;

            _inputStates[InputAxis.Horizontal] = 0f;
            _inputStates[InputAxis.Cancel] = 0f;

            _uiInputStates[UIInputAxis.btnGameOverGetMoreLives] = false;
            _uiInputStates[UIInputAxis.btnGameOverMainMenu] = false;
            _uiInputStates[UIInputAxis.btnGameWinOk] = false;
            _uiInputStates[UIInputAxis.btnQuitGame] = false;
            _uiInputStates[UIInputAxis.btnResumeGame] = false;
            _uiInputStates[UIInputAxis.btnGameLoseMainMenu] = false;
            _uiInputStates[UIInputAxis.btnGameLoseTryAgain] = false;

            //todo Post-UT: clear _menuUIInputStates too. (after you've done that, you can remove the lines in InMenuLogicProvider.OnStart() that set each menuUIInputState to false
        }

        public virtual void LogToDebugOutput(string msg)
        {
            _txtDebugOutput.Text += msg + Environment.NewLine;

            var lineCount = _txtDebugOutput.Text.Count(x => x == '\n');
            const int maxLines = 10;
            var linesToRemove = lineCount - maxLines;
            if (linesToRemove > 0)
            {
                var linesFound = 0;
                var curLinePosition = 0;
                while (linesFound < linesToRemove)
                {
                    curLinePosition = _txtDebugOutput.Text.IndexOf(Environment.NewLine, curLinePosition);

                    if (curLinePosition == -1)
                    {
                        break;
                    }
                    linesFound++;
                }

                var substringStartIndex = curLinePosition;
                if (curLinePosition < _txtDebugOutput.Text.Length - Environment.NewLine.Length)
                {
                    substringStartIndex += Environment.NewLine.Length + 1; //we want to start on the next char after the newline
                }

                _txtDebugOutput.Text = _txtDebugOutput.Text.Substring(substringStartIndex);
            }

            //todo real game: add a secret feature to enable debug logging. (e.g., maybe tap and hold for 10 seconds to enable clipboard logging)
            //    _gameEngineInterface.CopyToClipBoard(_txtDebugOutput.Text);
        }

        public virtual void OnStart()
        {
            var debugOutputGameObject = _gameEngineInterface.FindGameObject("txtDebugOutput");
            _txtDebugOutput = debugOutputGameObject.GetComponent<IText>();
        }

        protected void ClearDebugOutput()
        {
            _txtDebugOutput.Text = string.Empty;
        }

        public virtual void HandleInput()
        {

        }

        public virtual void UpdateGameObjects()
        {

        }

        public virtual void OnClick(string sender)
        {

        }

        public virtual void OnCollision(IComponent player, IGameObject cube)
        {

        }

        public virtual void OnFocus(bool hasFocus)
        {

        }
    }
}