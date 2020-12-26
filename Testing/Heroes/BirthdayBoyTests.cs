using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Studio29.BirthdayBoy;
using System.Collections.Generic;
using System.Linq;

namespace Studio29Tests
{
    [TestFixture()]
    public class BirthdayBoyTests : BaseTest
    {
        #region BirthdayBoyHelperFunctions
        protected HeroTurnTakerController birthday { get { return FindHero("BirthdayBoy"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(birthday.CharacterCard, 1);
            DealDamage(villain, birthday, 2, DamageType.Melee);
        }

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
        public void TestBirthdayBoyLoads()
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Megalopolis");
            StartGame();
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(birthday);
            Assert.IsInstanceOf(typeof(BirthdayBoyCharacterCardController), birthday.CharacterCardController);

            Assert.AreEqual(29, birthday.CharacterCard.HitPoints);
        }

        [Test()]
        [Sequential]
        public void DecklistTestOneShot_IsOneShot([Values("AnotherYearOlder", "Blowout", "GiftReceipt", "ItsMyParty", "ItsTheThoughtThatCounts", "Mixer", "PartyTilDawn", "SocialLadder", "TrashTheVenue", "YoureInvited")] string oneshot)
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(birthday);

            Card card = PlayCard(oneshot);
            AssertInTrash(card);
            AssertCardHasKeyword(card, "one-shot", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTestSurprise_IsSurprise([Values("DontGetGreedy")] string surprise)
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(birthday);

            Card card = GetCard(surprise);
            AssertCardHasKeyword(card, "surprise", false);
        }

        [Test()]
        public void TestBirthdayBoyPower_SurgeOfStrength()
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Haka", "Legacy", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(birthday);
            PlayCard("DecoyProjection");
            PlayCard("Dominion");
            Card surge = PlayCard("SurgeOfStrength");
            AssertInPlayArea(legacy, surge);

            GoToUsePowerPhase(birthday);
            DecisionSelectCard = surge;
            UsePower(birthday.CharacterCard);
            AssertInPlayArea(birthday, surge);
            AssertCardHasKeyword(surge, "present", false);

            //should be increased by 1 because of stolen card
            QuickHPStorage(baron);
            DealDamage(birthday, baron, 2, DamageType.Radiant);
            QuickHPCheck(-3);
   
        }

        [Test()]
        public void TestBirthdayBoyPower_Mere()
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Haka", "Legacy", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(birthday);
            PlayCard("DecoyProjection");
            Card mere = PlayCard("Mere");
            PlayCard("SurgeOfStrength");
            AssertInPlayArea(haka, mere);

            GoToUsePowerPhase(birthday);
            DecisionSelectCard = mere;
            UsePower(birthday.CharacterCard);
            AssertInPlayArea(birthday, mere);
            AssertCardHasKeyword(mere, "present", false);

            //AssertNotDamageSource(haka.CharacterCard);
            //AssertNextDecisionMaker(birthday);
            bool skipped;
            AssertNextPowerDecisionChoices(included: new Card[] { mere });
            SelectAndUsePower(birthday, out skipped);

            AssertNextPowerDecisionChoices(notIncluded: new Card[] { mere });
            SelectAndUsePower(haka, out skipped);


        }

    }
}
