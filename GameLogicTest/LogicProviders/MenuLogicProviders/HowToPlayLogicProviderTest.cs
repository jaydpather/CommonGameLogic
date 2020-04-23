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
    public class HowToPlayLogicProviderTest
    {
        private HowToPlayLogicProvider _howToPlayLogicProvider;
        private ILogicHandler _logicHandler;
        private IGameEngineInterface _gameEngineInterface;
        private IDataLayer _dataLayer;

        private IGameObject _pnlHowToPlay;
        private IGameObject _btnHowToPlayOk;

        [TestInitialize]
        public void TestInitialize()
        {
            _logicHandler = Substitute.For<ILogicHandler>();
            _gameEngineInterface = Substitute.For<IGameEngineInterface>();

            _pnlHowToPlay = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("pnlHowToPlay").Returns(_pnlHowToPlay);

            _btnHowToPlayOk = Substitute.For<IGameObject>();
            _gameEngineInterface.FindGameObject("btnHowToPlayOk").Returns(_btnHowToPlayOk);

            _howToPlayLogicProvider = new HowToPlayLogicProvider(_logicHandler, _gameEngineInterface, _dataLayer);
        }

        [TestMethod]
        public void OnStart()
        {
            _howToPlayLogicProvider.OnStart();

            _gameEngineInterface.Received(1).FindGameObject("pnlHowToPlay");
            _pnlHowToPlay.Received(1).SetActive(false);

            _gameEngineInterface.Received(1).FindGameObject("btnHowToPlayOk");
            Assert.AreEqual(_logicHandler, _btnHowToPlayOk.LogicHandler);
        }

        [TestMethod]
        public void OnActivate()
        {
            _howToPlayLogicProvider.OnStart(); //initialize

            _howToPlayLogicProvider.OnActivate();

            _pnlHowToPlay.Received(1).SetActive(true);
        }

        [TestMethod]
        public void OnDeActivate()
        {
            _howToPlayLogicProvider.OnStart(); //initialize

            _howToPlayLogicProvider.OnDeActivate();
            _pnlHowToPlay.Received(2).SetActive(false); //first call happens in OnStart()
        }

        [TestMethod]
        public void OnClick_HowToPlayOk()
        {
            _howToPlayLogicProvider.OnClick("btnHowToPlayOk");

            _logicHandler.Received(1).SetSceneState((int)MenuState.InMenu);
        }
    }
}
