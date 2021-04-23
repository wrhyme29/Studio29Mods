using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Studio29.DummyPlayer;
using System.Collections.Generic;
using System.Linq;

namespace Studio29Tests
{
    [TestFixture()]
    public class TestPlayerTests : BaseTest
    {
        #region TestPlayerHelperFunctions
        protected HeroTurnTakerController tester { get { return FindHero("TestPlayer"); } }
      


        #endregion

        [Test()]
        public void TestTestPlayerPower()
        {
            SetupGameController("BaronBlade", "Studio29.TestPlayer", "Haka/XtremePrimeWardensHakaCharacter", "Ra", "Megalopolis");
            StartGame();

            UsePower(tester);

            QuickHPStorage(baron, tester, haka, ra);
            DealDamage(baron, haka, 3, DamageType.Fire);
            QuickHPCheck(0, -1, 0, 0);
            QuickHPUpdate();
            DealDamage(baron, haka, 3, DamageType.Fire);
            QuickHPCheck(0, 0, -3, 0);
            PrintTriggers();

        }

        

    }
}
