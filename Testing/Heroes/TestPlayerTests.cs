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
    public class DummyPlayerTests : CustomBaseTest
    {
        #region BirthdayBoyHelperFunctions
        protected HeroTurnTakerController dummy { get { return FindHero("DummyPlayer"); } }
       

        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune, bool charactersImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsCharacter = new bool?(charactersImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }


        #endregion

        [Test()]
        public void TestDummyPlayerLoads()
        {
            SetupGameController("BaronBlade", "Studio29.DummyPlayer", "Haka", "Ra", "Megalopolis");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(dummy);

            AssertIncapacitated(dummy);
        }

        

    }
}
