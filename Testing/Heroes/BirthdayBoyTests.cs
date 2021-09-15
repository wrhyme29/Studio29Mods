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
        public void DecklistTestOneShot_IsOneShot([Values("AnotherYearOlder", "Blowout", "GiftReceipt", "ItsTheThoughtThatCounts", "Mixer", "PartyTilDawn", "SocialLadder", "TrashTheVenue", "YoureInvited")] string oneshot)
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(birthday);

            Card card = GetCard(oneshot);
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

            AssertNotDamageSource(haka.CharacterCard);
            AssertNextDecisionMaker(birthday);
            bool skipped;
            AssertNextPowerDecisionChoices(included: new Card[] { mere });
            SelectAndUsePower(birthday, out skipped);

            AssertNextPowerDecisionChoices(notIncluded: new Card[] { mere });
            SelectAndUsePower(haka, out skipped);


        }

        [Test()]
        public void TestBlowout()
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Haka", "Legacy", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 4);
            Card battalion = PlayCard("BladeBattalion");
            PlayCard("DecoyProjection");
            Card mere = PlayCard("Mere");
            PlayCard("SurgeOfStrength");

            GoToUsePowerPhase(birthday);
            DecisionSelectCards = new Card[] { mere, battalion };
            UsePower(birthday.CharacterCard);

            
            Card blowout = PlayCard("Blowout");
            AssertOutOfGame(battalion);
            AssertOutOfGame(mere);
            AssertOutOfGame(blowout);

        }

        [Test()]
        public void TestTrashTheVenue()
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Haka", "Legacy", "TheVisionary", "Megalopolis");
            StartGame();


            Card env1 = PlayCard("PoliceBackup");
            Card env2 = PlayCard("TrafficPileup");

            Card present1 = PlayCard("TheLegacyRing");
            Card present2 = PlayCard("Mere");

            //Destroy up to 2 environment cards. 
            //For each card destroyed this way, move 1 hero ongoing, hero equipment, or hero target with max 5 HP or fewer belonging to another hero in play to your hand.
            DecisionSelectCards = new Card[] { env1, env2, present1, present2 };

            PlayCard("TrashTheVenue");

            AssertInTrash(env1, env2);
            AssertInHand(birthday, present1);
            AssertInHand(birthday, present2);


        }

    }
}
