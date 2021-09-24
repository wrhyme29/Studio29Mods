using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Studio29.TheDeliOfDisaster;
using System;

namespace Studio29Tests
{
    [TestFixture()]
    public class TheDeliOfDisasterTests : CustomBaseTest
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

        [Test()]
        public void TestLoxxedUp()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);
            Card battalion = PlayCard("BladeBattalion");
            Card loxxedUp = PlayCard("LoxxedUp");

            //At the start of the environment turn, the villain target with the lowest HP deals the hero target with the highest HP 2 toxic damage.
            //lowest battalion, highest haka
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, tachyon.CharacterCard, haka.CharacterCard);
            AssertNextDamageSource(battalion);
            PrintSpecialStringsForCard(loxxedUp);
            GoToStartOfTurn(deli);
            QuickHPCheck(0, 0, 0, 0, -2);

            //At the end of the environment turn, the hero target with the lowest HP deals the villain target with the highest HP 2 energy damage.
            //lowest tachyon, highest baron
            QuickHPUpdate();
            AssertNextDamageSource(tachyon.CharacterCard);
            GoToEndOfTurn(deli);
            QuickHPCheck(-2, 0, 0, 0, 0);
        }

        [Test()]
        public void TestMindlessMishiga()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(deli);
            Card battalion = PlayCard("BladeBattalion");
            Card poweredRemoteTurret = MoveCard(baron, "PoweredRemoteTurret", baron.TurnTaker.Trash);
            SetHitPoints(battalion, 2);
            SetHitPoints(tachyon.CharacterCard, 10);
            DecisionSelectTurnTaker = ra.TurnTaker;

            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, tachyon.CharacterCard, haka.CharacterCard);
            QuickHandStorage(ra, tachyon, haka);
            QuickShuffleStorage(baron.TurnTaker.Trash);
            AssertNextDamageSource(battalion);

            PlayCard("MindlessMishiga");

            //"The villain and hero target with the lowest HP regains 1 HP.",
            //"One player draws a card.",
            //"Shuffle the Villain trash and reveal cards from the top until a Target is revealed. Put it into play. Put the other revealed cards back into the Villain Trash.",
            //"The villain target with the lowest HP deals the villain target with the highest HP 3 fire damage."
            int baronDamage = mdp.IsInPlayAndHasGameText ? 0 : -3;
            QuickHPCheck(baronDamage, 1, 0, 1, 0);
            QuickHandCheck(1, 0, 0);
            QuickShuffleCheck(1);
            AssertAnyInPlayArea(baron, new Card[] { poweredRemoteTurret, mdp });

        }

        [Test()]
        public void TestOrderUp()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            DiscardAllCards(ra, tachyon);

            Card dishCard = MoveCard(deli, "LoxxedUp", deli.TurnTaker.Deck);
            Card nonDishCard = MoveCard(deli, "MindlessMishiga", deli.TurnTaker.Deck);

            PlayCard("OrderUp");

            //At the start of the environment turn, reveal cards from the environment deck until a dish card is revealed. Put the dish card into play. Shuffle the remaining cards back into the environment deck.
            QuickShuffleStorage(deli.TurnTaker.Deck);
            GoToStartOfTurn(deli);
            AssertInPlayArea(deli, dishCard);
            AssertInDeck(deli, nonDishCard);
            AssertNumberOfCardsInRevealed(deli, 0);
            QuickShuffleCheck(1);

            //At the end of the environment turn, any player with fewer than 4 cards in their hand may draw a card.
            QuickHandStorage(ra, tachyon, haka);
            GoToEndOfTurn(deli);
            QuickHandCheck(1, 1, 0);
        }

        [Test()]
        public void TestRamblingYenta_DestroysDish()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card dishCard = PlayCard(deli, "LoxxedUp");

            GoToPlayCardPhase(deli);

            //When this card enters play, destroy 1 dish card. If no dish cards are destroyed this way, this card deals each target 1 fire damage.
            QuickHPStorage(baron, ra, tachyon, haka);
            Card ramblingYenta = PlayCard("RamblingYenta");
            QuickHPCheckZero();
            AssertInTrash(dishCard);


            Card pushTheLimits = PlayCard("PushingTheLimits");
            Card dominion = PlayCard("Dominion");
            DecisionSelectCards = new Card[] { pushTheLimits, null };
            DecisionAutoDecideIfAble = true;

            //At the end of the environment turn, this card deals 2 sonic damage to any hero with ongoing cards in play.
            //Each player dealt damage this way may destroy an ongoing card. 
            //If they do not, this card deals them an additional 2 sonic damage.
            QuickHPStorage(ra, tachyon, haka);
            GoToEndOfTurn(deli);
            QuickHPCheck(0, -2, -4);
            AssertInTrash(pushTheLimits);
            AssertInPlayArea(haka, dominion);

        }

        [Test()]
        public void TestRamblingYenta_NoDishDestroyed()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            GoToPlayCardPhase(deli);

            //When this card enters play, destroy 1 dish card. If no dish cards are destroyed this way, this card deals each target 1 fire damage.
            QuickHPStorage(baron, ra, tachyon, haka);
            Card ramblingYenta = GetCard("RamblingYenta");
            AssertNextDamageSource(ramblingYenta);
            AssertNextDamageType(DamageType.Fire);
            PlayCard(ramblingYenta);
            QuickHPCheck(-1,-1,-1,-1);


            Card pushTheLimits = PlayCard("PushingTheLimits");
            Card dominion = PlayCard("Dominion");
            DecisionSelectCards = new Card[] { pushTheLimits, null};
            DecisionAutoDecideIfAble = true;

            //At the end of the environment turn, this card deals 2 sonic damage to any hero with ongoing cards in play. Each player dealt damage this way may destroy an ongoing card. If they do not, this card deals them an additional 2 sonic damage.
            QuickHPStorage(ra, tachyon, haka);
            GoToEndOfTurn(deli);
            QuickHPCheck(0, -2, -4);
            AssertInTrash(pushTheLimits);
            AssertInPlayArea(haka, dominion);
        }

        [Test()]
        public void TestRamblingYenta_Sentinels()
        {
            SetupGameController("BaronBlade", "Ra", "TheSentinels", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            GoToPlayCardPhase(deli);

            PlayCard("RamblingYenta");

            Card sentinelTactics = PlayCard("SentinelTactics");
            Card dominion = PlayCard("Dominion");
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker, null };
            DecisionSelectCards = new Card[] { dominion, null,  idealist };
            DecisionAutoDecideIfAble = true;

            //At the end of the environment turn, this card deals 2 sonic damage to any hero with ongoing cards in play. Each player dealt damage this way may destroy an ongoing card. If they do not, this card deals them an additional 2 sonic damage.
            QuickHPStorage(ra.CharacterCard, medico, idealist, mainstay, writhe, haka.CharacterCard);
            GoToEndOfTurn(deli);
            QuickHPCheck(0, -2, -4,-2,-2,-2);
            AssertInTrash(dominion);
            AssertInPlayArea(sentinels, sentinelTactics);
        }

        [Test()]
        public void TestSchmaltzStorm_EveryoneDiscards()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card schmaltzStorm = PlayCard("SchmaltzStorm");

            //Reduce all damage by 1.
            QuickHPStorage(baron, ra, tachyon, haka);
            DealDamage(baron, c => c.IsHeroCharacterCard, 5, DamageType.Projectile);
            DealDamage(ra, baron, 5, DamageType.Fire);
            QuickHPCheck(-4,-4,-4,-4);

            //Reduce all hp recovery by 1.
            QuickHPUpdate();
            foreach(Card card in GameController.TurnTakerControllers.Where(ttc => ttc.IsHero || ttc.IsVillain).Select(ttc => ttc.CharacterCard))
            {
                GainHP(card, 3, card);
            }
            QuickHPCheck(2,2,2,2);

            //At the start of the environment turn, each player may discard 1 card to destroy this card.
            DecisionYesNo = true;
            QuickHandStorage(ra, tachyon, haka);
            GoToStartOfTurn(deli);
            QuickHandCheck(-1, -1, -1);
            AssertInTrash(schmaltzStorm);
        }

        [Test()]
        public void TestSchmaltzStorm_NoDiscards()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card schmaltzStorm = PlayCard("SchmaltzStorm");

            //Reduce all damage by 1.
            QuickHPStorage(baron, ra, tachyon, haka);
            DealDamage(baron, c => c.IsHeroCharacterCard, 5, DamageType.Projectile);
            DealDamage(ra, baron, 5, DamageType.Fire);
            QuickHPCheck(-4,-4,-4,-4);

            //Reduce all hp recovery by 1.
            QuickHPUpdate();
            foreach (Card card in GameController.TurnTakerControllers.Where(ttc => ttc.IsHero || ttc.IsVillain).Select(ttc => ttc.CharacterCard))
            {
                GainHP(card, 3, card);
            }
            QuickHPCheck(2,2,2,2);

            //At the start of the environment turn, each player may discard 1 card to destroy this card.
            DecisionYesNo = false;
            QuickHandStorage(ra, tachyon, haka);
            GoToStartOfTurn(deli);
            QuickHandCheckZero();
            AssertInPlayArea(deli, schmaltzStorm);
        }

        [Test()]
        public void TestSchmearCampaign_DealsInitialDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card schmearCampaign = PlayCard("SchmearCampaign");

            Card cardToDiscard = GetRandomCardFromHand(haka);

            DecisionSelectCards = new Card[] { tachyon.CharacterCard, cardToDiscard };
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker };
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(baron.TurnTaker.Deck) };


            //At the start of the environment turn, this card deals one hero character card 3 energy damage. If no damage is taken this way, destroy this card.
            QuickHPStorage(ra, tachyon, haka);
            AssertNextDamageSource(schmearCampaign);
            AssertNextDamageType(DamageType.Energy);
            GoToStartOfTurn(deli);
            QuickHPCheck(0, -3, 0);
            AssertInPlayArea(deli, schmearCampaign);

            //At the start of the villain turn, one player may discard a card to reveal the top card of the villain deck. They may discard or replace the revealed card.
            Card topCard = baron.TurnTaker.Deck.TopCard;
            GoToStartOfTurn(baron);
            AssertInTrash(cardToDiscard);
            AssertOnTopOfDeck(baron, topCard);
        }

        [Test()]
        public void TestSchmearCampaign_DestroysItself()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card schmearCampaign = PlayCard("SchmearCampaign");
            Card cardToDiscard = GetRandomCardFromHand(haka);

            DecisionSelectCards = new Card[] { tachyon.CharacterCard, cardToDiscard };
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker };
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(baron.TurnTaker.Trash) };

            AddImmuneToDamageTrigger(tachyon, heroesImmune: true, villainsImmune: false);

            //At the start of the environment turn, this card deals one hero character card 3 energy damage. If no damage is taken this way, destroy this card.

            QuickHPStorage(ra, tachyon, haka);
            GoToStartOfTurn(deli);
            QuickHPCheckZero();
            AssertInTrash(deli, schmearCampaign);

            GoToPlayCardPhase(deli);
            PlayCard(schmearCampaign);

            //At the start of the villain turn, one player may discard a card to reveal the top card of the villain deck. They may discard or replace the revealed card.
            Card topCard = baron.TurnTaker.Deck.TopCard;
            GoToStartOfTurn(baron);
            AssertInTrash(cardToDiscard);
            AssertInTrash(topCard);
        }

        [Test()]
        public void TestStuckleAndNosher_DiscardsSufficientCount()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card battalion = PlayCard("BladeBattalion");

            SetHitPoints(baron.CharacterCard, 10);
            SetHitPoints(battalion, 2);
            SetHitPoints(ra.CharacterCard, 10);
            SetHitPoints(tachyon.CharacterCard, 10);
            SetHitPoints(haka.CharacterCard, 10);

            GoToPlayCardPhase(deli);
            Card stuckleAndNosher = PlayCard("StuckleAndNosher");

            //At the end of the environment turn, each character card gains 2 hp.
            //Each player may discard 1 card. If fewer than {H - 1} cards are discarded this way, destroy this card.
            DecisionSelectTurnTakers = new TurnTaker[] { ra.TurnTaker, haka.TurnTaker, null };
            DecisionSelectCards = new Card[] { GetRandomCardFromHand(ra), GetRandomCardFromHand(haka) };

            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, tachyon.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(deli);
            QuickHPCheck(2, 0, 2, 2, 2);
            AssertInPlayArea(deli, stuckleAndNosher);
        }

        [Test()]
        public void TestStuckleAndNosher_DestroyItself()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(haka);

            Card battalion = PlayCard("BladeBattalion");

            SetHitPoints(baron.CharacterCard, 10);
            SetHitPoints(battalion, 2);
            SetHitPoints(ra.CharacterCard, 10);
            SetHitPoints(tachyon.CharacterCard, 10);
            SetHitPoints(haka.CharacterCard, 10);

            GoToPlayCardPhase(deli);
            Card stuckleAndNosher = PlayCard("StuckleAndNosher");

            //At the end of the environment turn, each character card gains 2 hp.
            //Each player may discard 1 card. If fewer than {H - 1} cards are discarded this way, destroy this card.
            DecisionSelectTurnTakers = new TurnTaker[] { ra.TurnTaker, null};
            DecisionSelectCards = new Card[] { GetRandomCardFromHand(ra) };

            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, tachyon.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(deli);
            QuickHPCheck(2, 0, 2, 2, 2);
            AssertInTrash(deli, stuckleAndNosher);
        }

        [Test()]
        public void TestWipeDown()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(deli);
            Card dish1 = PlayCard("LoxxedUp");
            Card dish2 = PlayCard("KugelConundrum");

            //When this card enters play, destroy all dish cards. This card deals each target X + 1 energy damage, where X is the number of dishes destroyed this way.
            QuickHPStorage(baron, ra, tachyon, haka);
            Card wipeDown = PlayCard("WipeDown");
            QuickHPCheck(-3, -3, -3, -3);
            AssertInTrash(dish1);
            AssertInTrash(dish2);

            //At the end of the environment turn, destroy this card.
            GoToEndOfTurn(deli);
            AssertInTrash(wipeDown);
        }

        [Test()]
        public void TestYoureSmokedMeat()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Studio29.TheDeliOfDisaster");
            StartGame();

            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(deli);

            PlayCard("YoureSmokedMeat");
            //Increase all fire damage by 2.
            int expectedDamage;
            foreach (DamageType dt in Enum.GetValues(typeof(DamageType)))
            {
                foreach (TurnTakerController ttc in GameController.TurnTakerControllers.Where(ttc => ttc.IsVillain || ttc.IsHero))
                {
                    QuickHPStorage(baron);
                    expectedDamage = dt == DamageType.Fire  ? -3 : -1;
                    DealDamage(ttc, baron, 1, dt, isIrreducible: true);
                    QuickHPCheck(expectedDamage);
                    SetAllTargetsToMaxHP();
                }

            }

            //At the end of the environment turn, this card deals each target 1 fire damage.
            QuickHPStorage(baron, ra, tachyon, haka);
            GoToEndOfTurn(deli);
            QuickHPCheck(-3, -3, -3, -3);

        }
    }
}
