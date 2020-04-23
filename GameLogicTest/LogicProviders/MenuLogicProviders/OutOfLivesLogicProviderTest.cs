using GameLogic.LogicProviders.MenuLogicProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdEyeSoftware.GameLogic;
using ThirdEyeSoftware.GameLogic.LogicHandlers;

namespace GameLogicTest.LogicProviders.MenuLogicProviders
{
    [TestClass]
    public class OutOfLivesLogicProviderTest : TestBase
    {
        IGameEngineInterface _gameEngineInterface;
        ILogicHandler _logicHandler;
        IDataLayer _dataLayer;
        IGameObject _btnOutOfLivesOk;
        IGameObject _pnlOutOfLives;
        IGameObject _txtDebugOutput;
        OutOfLivesLogicProvider _outOfLivesLogicProvider;

        [TestInitialize]
        public void TestInitialize()
        {
            _gameEngineInterface = Substitute.For<IGameEngineInterface>();

            _logicHandler = Substitute.For<ILogicHandler>();

            _btnOutOfLivesOk = Substitute.For<IGameObject>();
            _pnlOutOfLives = Substitute.For<IGameObject>();
            _txtDebugOutput = Substitute.For<IGameObject>();

            _outOfLivesLogicProvider = new OutOfLivesLogicProvider(_logicHandler, _gameEngineInterface, _dataLayer);

            _gameEngineInterface.FindGameObject("btnOutOfLivesOk").Returns(_btnOutOfLivesOk);
            _gameEngineInterface.FindGameObject("pnlOutOfLives").Returns(_pnlOutOfLives);
            _gameEngineInterface.FindGameObject("txtDebugOutput").Returns(_txtDebugOutput);
        }

        [TestMethod]
        public void OnStart()
        {
            _outOfLivesLogicProvider.OnStart();

            _gameEngineInterface.Received().FindGameObject("btnOutOfLivesOk");
            Assert.AreEqual(_logicHandler, _btnOutOfLivesOk.LogicHandler);

            _gameEngineInterface.Received().FindGameObject("pnlOutOfLives");
            _pnlOutOfLives.Received().SetActive(false);
        }

        [TestMethod]
        public void OnActivate()
        {
            SetPrivateMember("_pnlOutOfLives", _outOfLivesLogicProvider, _pnlOutOfLives);
            _outOfLivesLogicProvider.OnActivate();

            _pnlOutOfLives.Received().SetActive(true);
        }

        [TestMethod]
        public void OnDeActivate()
        {
            SetPrivateMember("_pnlOutOfLives", _outOfLivesLogicProvider, _pnlOutOfLives);
            _outOfLivesLogicProvider.OnDeActivate();

            _pnlOutOfLives.Received().SetActive(false);
        }

        [TestMethod]
        public void btnOutOfLivesOk_Click()
        {
            _outOfLivesLogicProvider.btnOutOfLivesOk_Click();

            _logicHandler.Received().SetSceneState((int)MenuState.GetMoreLives);
        }
    }
}
