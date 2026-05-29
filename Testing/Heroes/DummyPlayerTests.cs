using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using System.Linq;

namespace Studio29Tests
{
    [TestFixture()]
    public class DummyPlayerTests : CustomBaseTest
    {      

        [Test()]
        public void TestDummyPlayerLoads()
        {
            SetupGameController("BaronBlade", "Studio29.DummyPlayer", "Haka", "Ra", "Megalopolis");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(dummyPlayer);

            AssertIncapacitated(dummyPlayer);
        }

        

    }
}
