using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Studio29.TheDeliOfDisaster;

namespace Studio29Tests
{
    [TestFixture()]
    public class TheDeliOfDisasterTests : BaseTest
    {

        #region TheDeliOfDisasterHelperFunctions

        protected TurnTakerController deli { get { return FindEnvironment(); } }
        protected bool IsDish(Card card)
        {
            return card != null && card.DoKeywordsContain("dish");
        }

        #endregion

        [Test()]
        public void TestTheDeliOfDisasterWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_Diner_IsDiner([Values("InComesThePutz", "RamblingYenta", "StuckleAndNosher")] string diner)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            GoToPlayCardPhase(deli);

            Card card = PlayCard(diner);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "diner", false);

            Assert.That(FindCardController(card) is DinerCardController);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Dish_IsDish([Values("BitOfAPickle", "KugelConundrum", "LoxxedUp", "SchmaltzStorm", "SchmearCampaign")] string dish)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            GoToPlayCardPhase(deli);

            Card card = PlayCard(dish);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "dish", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NoKeyword_HasNoKeyword([Values("ClosingTime", "EssGezunt", "Gornish", "MindlessMishiga", "OrderUp", "WipeDown", "YoureSmokedMeat")] string keywordLess)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            GoToPlayCardPhase(deli);

            Card card = PlayCard(keywordLess);
            AssertIsInPlay(card);
            Assert.IsFalse(card.Definition.Keywords.Any(), $"{card.Title} has keywords when it shouldn't.");
        }

        [Test()]
        public void TestBitOfAPickle()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card aBitOfAPickle = PlayCard("BitOfAPickle");

            //This card is indestructible.
            DestroyCard(aBitOfAPickle, haka.CharacterCard);
            AssertInPlayArea(deli, aBitOfAPickle);

            DecisionSelectTurnTakers =  new TurnTaker[] { null, haka.TurnTaker };


            //Listed win or loss conditions on cards are not in effect and may not be used to win or lose the game.
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.Trash, leaveSomeCards: 5);
            GoToStartOfTurn(baron);
            AssertNotGameOver();

            MoveAllCards(baron, baron.TurnTaker.Trash, baron.TurnTaker.Deck, leaveSomeCards: 5);

            //At the start of the environment turn, one player may discard their hand to shuffle this card back into the environment deck, ignoring its indestructability.
            QuickShuffleStorage(deli.TurnTaker.Deck);
            GoToStartOfTurn(deli);
            AssertInDeck(aBitOfAPickle);
            QuickShuffleCheck(1);
            MoveAllCards(baron, baron.TurnTaker.Deck, baron.TurnTaker.Trash, leaveSomeCards: 5);
            GoToStartOfTurn(baron);
            AssertGameOver(EndingResult.AlternateDefeat);

        }

        [Test()]
        public void TestClosingTime()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            PlayCard("ClosingTime");

            DecisionSelectCards = new Card[] { GetRandomCardFromHand(ra), null, GetRandomCardFromHand(haka) };

            //At the start of the environment turn, each player may discard a card.
            //Deal any character that does not discard a card 2 energy damage.

            QuickHandStorage(ra, legacy, haka);
            QuickHPStorage(ra, legacy, haka);
            GoToStartOfTurn(deli);
            QuickHPCheck(0, -2, 0);
            QuickHandCheck(-1, 0, -1);

        }

        [Test()]
        public void TestEssGezunt()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            PlayCard("EssGezunt");

            foreach(Card card in GameController.GetAllCards().Where(c => c.IsInPlayAndHasGameText && c.IsTarget))
            {
                SetHitPoints(card, 3);
            }

            //At the end of the environment turn, each target regains 1 HP.
            QuickHPStorage(baron, ra, legacy, haka);
            GoToEndOfTurn(deli);
            QuickHPCheck(1, 1, 1, 1);
        }

        [Test()]
        public void TestGornish()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            PlayCard("Gornish");

            List<Card> topCards = GameController.TurnTakerControllers.Select(ttc => ttc.TurnTaker.Deck.TopCard).ToList();
            System.Console.WriteLine($"Top cards are: {string.Join(", ", topCards.Select(c => c.Title).ToArray())}");

            //At the start of the environment turn, discard the top card of each deck.
            GoToStartOfTurn(deli);

            AssertInTrash(topCards);

            //At the end of the environment turn, shuffle each trash into its deck.
            QuickShuffleStorage(baron.TurnTaker.Deck, ra.TurnTaker.Deck, legacy.TurnTaker.Deck, haka.TurnTaker.Deck, deli.TurnTaker.Deck);
            GoToEndOfTurn(deli);
            QuickShuffleCheck(1, 1, 1, 1, 1);
            AssertNumberOfCardsInTrash(baron, 0);
            AssertNumberOfCardsInTrash(ra, 0);
            AssertNumberOfCardsInTrash(legacy, 0);
            AssertNumberOfCardsInTrash(haka, 0);
            AssertNumberOfCardsInTrash(deli, 0);
        }

        [Test()]
        [Sequential]
        public void TestDiner_DishToDestroy([Values("InComesThePutz", "RamblingYenta", "StuckleAndNosher")] string diner)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            Card dish = PlayCard("LoxxedUp");
            //When this card enters play, destroy 1 dish card. If there are no dish cards are destroyed, this card deals each target 1 fire damage.
            QuickHPStorage(baron, ra, legacy, haka);
            PlayCard(diner);
            AssertInTrash(dish);
            QuickHPCheckZero();
        }

        [Test()]
        [Sequential]
        public void TestDiner_NoDishToDestroy([Values("InComesThePutz", "RamblingYenta", "StuckleAndNosher")] string diner)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            //When this card enters play, destroy 1 dish card. If there are no dish cards are destroyed, this card deals each target 1 fire damage.
            QuickHPStorage(baron, ra, legacy, haka);
            PlayCard(diner);
            QuickHPCheck(-1, -1, -1, -1);
        }

        [Test()]
        public void TestInComesThePutz_EndOfTurn_Damage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card inComesThePutz = PlayCard("InComesThePutz");
            SetHitPoints(inComesThePutz, 2);

            //At the end of the environment turn, this card deals the villain target with the highest HP 4 sonic damage. If damage is taken this way, play the top card of the villain deck and restore this card to its max hp.
            Card battalion = PlayCard("BladeBattalion");
            Card backlashField = PutOnDeck("BacklashField");
            QuickHPStorage(baron.CharacterCard, battalion);
            GoToEndOfTurn(deli);
            QuickHPCheck(-4, 0);
            AssertInPlayArea(baron, backlashField);
            AssertIsAtMaxHP(inComesThePutz);
        }

        [Test()]
        public void TestInComesThePutz_EndOfTurn_NoDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card inComesThePutz = PlayCard("InComesThePutz");
            SetHitPoints(inComesThePutz, 2);
            //At the end of the environment turn, this card deals the villain target with the highest HP 4 sonic damage. If damage is taken this way, play the top card of the villain deck and restore this card to its max hp.
            Card mobileDefensePlatform = PlayCard("MobileDefensePlatform");
            Card backlashField = PutOnDeck("BacklashField");
            QuickHPStorage(baron.CharacterCard, mobileDefensePlatform);
            GoToEndOfTurn(deli);
            QuickHPCheck(0, 0);
            AssertOnTopOfDeck(baron, backlashField);
            AssertHitPoints(inComesThePutz, 2);
        }

        [Test()]
        public void TestKugelConundrum()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            
            Card kugelConundrum = PlayCard("KugelConundrum");
            SetHitPoints(ra, 13);
            SetHitPoints(legacy, 14);
            SetHitPoints(haka, 16);
            DecisionSelectCards = new Card[] { haka.CharacterCard, baron.CharacterCard, GetRandomCardFromHand(haka), null };
            //At the end of the environment turn, each hero character card with an odd number of hp regains 1 hp.
            ///Each hero character card with an even number of hp may take 1 toxic damage from this card to deal 2 toxic damage to another target. 
            //Each character dealt damage by this card must discard 1 card.
            QuickHPStorage(baron, ra, legacy, haka);
            QuickHandStorage(ra, legacy, haka);
            GoToEndOfTurn(deli);
            QuickHandCheck(0, 0, -1);
            QuickHPCheck(-2, 1, 0, -1);

        }
    }
}
