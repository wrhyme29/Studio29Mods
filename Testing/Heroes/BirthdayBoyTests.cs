using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using Studio29.BirthdayBoy;
using System.Linq;

namespace Studio29Tests
{
    [TestFixture()]
    public class BirthdayBoyTests : CustomBaseTest
    {
        [Test()]
        public void TestBirthdayBoyLoads()
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Megalopolis");
            StartGame();
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(birthdayBoy);
            Assert.IsInstanceOf(typeof(BirthdayBoyCharacterCardController), birthdayBoy.CharacterCardController);

            Assert.AreEqual(29, birthdayBoy.CharacterCard.HitPoints);
        }

        [Test()]
        [Sequential]
        public void DecklistTestOneShot_IsOneShot([Values("AnotherYearOlder", "Blowout", "GiftReceipt", "ItsTheThoughtThatCounts", "Mixer", "PartyTilDawn", "SocialLadder", "TrashTheVenue", "YoureInvited")] string oneshot)
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(birthdayBoy);

            Card card = GetCard(oneshot);
            AssertCardHasKeyword(card, "one-shot", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTestSurprise_IsSurprise([Values("DontGetGreedy")] string surprise)
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(birthdayBoy);

            Card card = GetCard(surprise);
            AssertCardHasKeyword(card, "surprise", false);
        }

        [Test()]
        public void TestBirthdayBoyPower_SurgeOfStrength()
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Haka", "Legacy", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(birthdayBoy);
            PlayCard("DecoyProjection");
            PlayCard("Dominion");
            Card surge = PlayCard("SurgeOfStrength");
            AssertInPlayArea(legacy, surge);

            GoToUsePowerPhase(birthdayBoy);
            DecisionSelectCard = surge;
            UsePower(birthdayBoy.CharacterCard);
            AssertInPlayArea(birthdayBoy, surge);
            AssertCardHasKeyword(surge, "present", false);

            //should be increased by 1 because of stolen card
            QuickHPStorage(baron);
            DealDamage(birthdayBoy, baron, 2, DamageType.Radiant);
            QuickHPCheck(-3);
   
        }

        [Test()]
        public void TestBirthdayBoyPower_Mere()
        {
            SetupGameController("BaronBlade", "Studio29.BirthdayBoy", "Haka", "Legacy", "TheVisionary", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(birthdayBoy);
            PlayCard("DecoyProjection");
            Card mere = PlayCard("Mere");
            PlayCard("SurgeOfStrength");
            AssertInPlayArea(haka, mere);

            GoToUsePowerPhase(birthdayBoy);
            DecisionSelectCard = mere;
            UsePower(birthdayBoy.CharacterCard);
            AssertInPlayArea(birthdayBoy, mere);
            AssertCardHasKeyword(mere, "present", false);

            AssertNotDamageSource(haka.CharacterCard);
            AssertNextDecisionMaker(birthdayBoy);
            bool skipped;
            AssertNextPowerDecisionChoices(included: new Card[] { mere });
            SelectAndUsePower(birthdayBoy, out skipped);

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

            GoToUsePowerPhase(birthdayBoy);
            DecisionSelectCards = new Card[] { mere, battalion };
            UsePower(birthdayBoy.CharacterCard);

            
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
            AssertInHand(birthdayBoy, present1);
            AssertInHand(birthdayBoy, present2);


        }

    }
}
