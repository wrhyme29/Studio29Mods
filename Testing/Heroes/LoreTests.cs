using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Studio29.Lore;
using System.Collections.Generic;
using System.Linq;

namespace Studio29Tests
{
    [TestFixture()]
    public class LoreTests : CustomBaseTest
    {
        #region LoreHelperFunctions
        protected HeroTurnTakerController lore { get { return FindHero("Lore"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(lore.CharacterCard, 1);
            DealDamage(villain, lore, 2, DamageType.Melee);
        }

        #endregion

        [Test()]
        public void TestLoreLoads()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Megalopolis");
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(lore);
            Assert.IsInstanceOf(typeof(LoreCharacterCardController), lore.CharacterCardController);

            Assert.AreEqual(25, lore.CharacterCard.HitPoints);

            StartGame();

        }

        [Test()]
        public void TestLoreInnatePower()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(lore);
            Card story = PutOnDeck("BlessingOfTheFifteenthStone");
            Card nonstory1 = PutOnDeck("RallyingCry");
            Card nonstory2 = PutOnDeck("Cliffhanger");
            Card nonstory3 = PutOnDeck("Honeymoon");
            QuickShuffleStorage(lore.TurnTaker.Deck);

            //Reveal cards from the top of {Lore}'s deck until a Story card is revealed. Put that card into play. Shuffle the other revealed cards into {Lore}'s deck."
            UsePower(lore.CharacterCard);

            AssertInPlayArea(lore, story);
            AssertInDeck(nonstory1);
            AssertInDeck(nonstory2);
            AssertInDeck(nonstory3);
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestLoreIncap1()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Legacy", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetupIncap(baron);
            AssertIncapacitated(lore);
            GoToUseIncapacitatedAbilityPhase(lore);
            Card discardCard = GetRandomCardFromHand(haka);

            //One player discards a card. Each other player may draw a card.
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker };
            DecisionSelectCards = new Card[] { discardCard };
            QuickHandStorage(haka, ra, legacy);
            UseIncapacitatedAbility(lore, 0);
            QuickHandCheck(-1, 1, 1);
        }

        [Test()]
        public void TestLoreIncap2()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Legacy", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetupIncap(baron);
            AssertIncapacitated(lore);
            GoToUseIncapacitatedAbilityPhase(lore);
            Card mere = PutInHand("Mere");
            Card ring = PutInTrash("TheLegacyRing");
            Card staff = PutInTrash("TheStaffOfRa");
            Card taiaha = PutInTrash("Taiaha");
            Card solar = PutInTrash("SolarFlare");


            //One player discards a card. Move two cards with a matching keyword from a trash into their owner's hand.
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker };
            DecisionSelectCards = new Card[] { mere, ring, staff };
            UseIncapacitatedAbility(lore, 1);
            AssertInHand(ring, staff);
            AssertInTrash(taiaha, mere, solar);
        }

        [Test()]
        public void TestLoreIncap3()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetupIncap(baron);
            AssertIncapacitated(lore);
            GoToUseIncapacitatedAbilityPhase(lore);

            //One player discards a card. Reveal cards from the top of their deck until a card with a matching keyword is revealed. Put that card into play. Shuffle the other revealed cards back into the deck.
            UseIncapacitatedAbility(lore, 2);
        }

        [Test()]
        public void TestAStoryOfFire()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(lore);

            PrintSeparator("Playing A Story of Fire");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, haka.CharacterCard };
            QuickHPStorage(baron, lore, ra, haka);

            //{Lore} deals one target 2 fire damage. Lore deals a second target 2 fire damage
            PlayCard("AStoryOfFire");
            QuickHPCheck(-2, 0, 0, -2);

        }

        [Test()]
        public void TestAStoryOfIce()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(haka, 20);
            SetHitPoints(ra, 20);
            SetHitPoints(lore, 20);
            SetHitPoints(baron, 20);

            GoToPlayCardPhase(lore);

            PrintSeparator("Playing A Story of Ice");

            DecisionSelectCards = new Card[] { baron.CharacterCard, haka.CharacterCard };
            QuickHPStorage(baron, lore, ra, haka);

            //{Lore} deals one target 2 cold damage. One target gains 2 hp
            PlayCard("AStoryOfIce");
            QuickHPCheck(-2, 0, 0, 2);

        }

        [Test()]
        public void TestAStoryOfSpeed()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            DiscardAllCards(haka);

            GoToPlayCardPhase(lore);

            PrintSeparator("Playing A Story of Speed");

            DecisionSelectCards = new Card[] { baron.CharacterCard };
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker };
            QuickHPStorage(baron, lore, ra, haka);
            QuickShuffleStorage(haka.TurnTaker.Deck);

            //{Lore} deals one target 2 sonic damage. You may shuffle one trash into its deck.
            PlayCard("AStoryOfSpeed");
            QuickHPCheck(-2, 0, 0, 0);
            AssertNumberOfCardsInTrash(haka, 0);
            QuickShuffleCheck(1);

        }

        [Test()]
        public void TestAStoryOfTriumph()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(haka, 20);
            SetHitPoints(ra, 20);
            SetHitPoints(lore, 20);
            SetHitPoints(baron, 20);

            GoToPlayCardPhase(lore);


            DecisionSelectCards = new Card[] { baron.CharacterCard };
            Card triumph = PutInTrash("AStoryOfTriumph");
            QuickHPStorage(baron, lore, ra, haka);
            QuickHandStorage(lore, ra, haka);

            PrintSeparator("Playing A Story of Triumph");

            //{Lore} deals one target 2 radiant damage. You may draw a card.
            PlayCard(triumph);
            QuickHPCheck(-2, 0, 0, 0);
            QuickHandCheck(1, 0, 0);


        }

        [Test()]
        public void TestBlessingOfTheFifteenthStone()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Unity", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToStartOfTurn(lore);
            Card battalion = PlayCard("BladeBattalion");
            SetHitPoints(haka, 5);
            SetHitPoints(unity, 5);
            SetHitPoints(lore, 5);
            SetHitPoints(baron, 5);

            Card otherMyth = PlayCard("Epilogue");
            Card raptorBot = PlayCard("RaptorBot");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Blessing of the Fifteenth Stone");

            //Return all other myth cards in play to your hand.
            Card stone = PlayCard("BlessingOfTheFifteenthStone");
            AssertInHand(otherMyth);

            //At the end of your turn, select one target other than {Lore} with 5 or fewer hp. Until the start of your next turn, that target is indestructible.
            DecisionSelectCards = new Card[] { raptorBot, baron.CharacterCard, haka.CharacterCard };
            AssertNextDecisionChoices(included: new List<Card>() { baron.CharacterCard, battalion, unity.CharacterCard, raptorBot, haka.CharacterCard }, notIncluded: new List<Card>() { lore.CharacterCard });
            GoToEndOfTurn(lore);

            DestroyCard(raptorBot, baron.CharacterCard);
            AssertInPlayArea(unity, raptorBot);

            GoToStartOfTurn(lore);
            DestroyCard(raptorBot, baron.CharacterCard);
            AssertInTrash(unity, raptorBot);

        }

        [Test()]
        public void TestCallback()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card mere = PutInTrash("Mere");
            Card tornado = PutInTrash("BlazingTornado");
            Card otherMystery = PlayCard("PageTurner");
          
            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Callback");

            //Return all other mystery cards in play to your hand.
            Card callback = PlayCard("Callback");
            AssertInHand(otherMystery);


            //One player other than Lore may take a card from their trash and put on top of their deck.
            //One player may draw a card.

            DecisionSelectCards = new Card[] { tornado };
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker };
            QuickHandStorage(lore, ra, haka);

            GoToUsePowerPhase(lore);
            PrintSeparator("Using Power on Callback");

            UsePower(callback);
            AssertOnTopOfDeck(tornado);
            AssertInTrash(mere);
            QuickHandCheck(0, 0, 1);

        }

        [Test()]
        public void TestCliffhanger()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card env1 = PlayCard("ImpendingCasualty");
            Card env2 = PlayCard("TrafficPileup");
            Card envToPlay = PutOnDeck("PoliceBackup");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Cliffhanger");

            //You may destroy one Environment card
            //Play the top card of the environment deck

            DecisionSelectCards = new Card[] { env2 };
            PlayCard("Cliffhanger");
            AssertInTrash(env2);
            AssertInPlayArea(env, env1);
            AssertInPlayArea(env, envToPlay);

        }

        [Test()]
        public void TestDeweyTheDecimal()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card storyToSearchFor = PutInDeck("BlessingOfTheFifteenthStone");
            Card dewey = PutInTrash("DeweyTheDecimal");
            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Dewey the Decimal");

            //Search your deck for a Story and put it into your hand. Shuffle your deck. 
            //You may draw a card.

            QuickHandStorage(lore);
            QuickShuffleStorage(lore.TurnTaker.Deck);
            DecisionSelectCards = new Card[] { storyToSearchFor };
            PlayCard(dewey);
            QuickHandCheck(2);
            QuickShuffleCheck(1);
            AssertInHand(storyToSearchFor);

        }

        [Test()]
        public void TestDoubleAgent()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card battalion = PlayCard("BladeBattalion");
            Card turret = PlayCard("PoweredRemoteTurret");
           
            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Double Agent");

            //{Lore} may deal one non-character villain target 2 psychic damage. 
            //A target dealt damage that way now deals a character card in its play area 2 irreducible melee damage.

            DecisionSelectTargets = new Card[] { battalion, baron.CharacterCard };
            QuickHPStorage(baron.CharacterCard, battalion, turret);
            PlayCard("DoubleAgent");
            QuickHPCheck(-2, -2, 0);

        }

        [Test()]
        public void TestEpilogue()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Unity", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card battalion = PutInTrash("BladeBattalion");
            Card turret = PutInTrash("PoweredRemoteTurret");
            Card raptorBot = PutInTrash("RaptorBot");
            Card platformBot = PutInTrash("PlatformBot");

            Card otherMyth = PlayCard("BlessingOfTheFifteenthStone");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Epilogue");

            Card epilogue = PlayCard("Epilogue");
            AssertInHand(otherMyth);

            //Select a trash. You may search that trash for a target and put that target into play.
            GoToUsePowerPhase(lore);
            PrintSeparator("Using Power of Epilogue");
            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(unity.TurnTaker.Trash)};
            DecisionSelectCards = new Card[] { raptorBot };
            UsePower(epilogue);
            AssertInPlayArea(unity, raptorBot);

        }

        [Test()]
        public void TestEvenTheScore_MythInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(lore, 19);
            SetHitPoints(ra, 14);
            SetHitPoints(haka, 17);

            Card myth = PlayCard("BlessingOfTheFifteenthStone");
            Card score = PutInTrash("EvenTheScore");

            GoToPlayCardPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Playing Even the Score");
            //If there is a myth card in play, any hero with an odd number of HP regains 1 hp.
            QuickHPStorage(lore, ra, haka);
            PlayCard(score);
            QuickHPCheck(1, 0, 1);
            QuickHandCheck(0);

        }

        [Test()]
        public void TestEvenTheScore_NoMythInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(lore, 19);
            SetHitPoints(ra, 14);
            SetHitPoints(haka, 17);

            Card score = PutInTrash("EvenTheScore");

            GoToPlayCardPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Playing Even the Score");
            //If there is not a myth card in play, draw a card.
            QuickHPStorage(lore, ra, haka);
            PlayCard(score);
            QuickHPCheck(0, 0, 0);
            QuickHandCheck(1);

        }

        [Test()]
        public void TestFourFiftyOne()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card otherEpic = PlayCard("SpeedReader");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing 451");

            Card fourFiftyOne = PlayCard("FourFiftyOne");
            AssertInHand(otherEpic);

            Card myth = PlayCard("Epilogue");
            Card romance = PlayCard("Whirlwinding");
            Card action = PlayCard("SaveYourself");
            Card mystery = PlayCard("PageTurner");

            GoToUsePowerPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Using Power of 451");

            //Destroy any number of Story cards. Draw X cards, where X is the number of cards destroyed this way plus 1.
            DecisionSelectCards = new Card[] { romance, action, mystery, null };

            UsePower(fourFiftyOne);
            AssertInTrash(romance, action, mystery);
            AssertIsInPlay(myth);
            QuickHandCheck(4);

        }

        [Test()]
        public void TestHoneymoon_NoRomanceInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card honeymoon = PutInTrash("Honeymoon");

            GoToPlayCardPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Playing Honeymoon");
            //If there is not a romance card in play, draw a card.
            DecisionSelectCards = new Card[] { haka.CharacterCard };
            PlayCard(honeymoon);
            QuickHandCheck(1);

            //first damage not prevented
            QuickHPStorage(haka);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //second damage not prevented
            QuickHPUpdate();
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestHoneymoon_RomanceInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card honeymoon = PutInTrash("Honeymoon");
            Card romance = PlayCard("LoveTriangle");

            GoToPlayCardPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Playing Honeymoon");
            //If there is a romance card in play, select a target. Prevent the first damage dealt to that target each turn until the start of your next turn.
            DecisionSelectCards = new Card[] { haka.CharacterCard };
            PlayCard(honeymoon);
            QuickHandCheck(0);

            //first damage prevented
            QuickHPStorage(haka);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(0);

            //second damage not
            QuickHPUpdate();
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            GoToNextTurn();
            
            //resets next turn
            QuickHPUpdate();
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(0);

            //second damage not prevented
            QuickHPUpdate();
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            GoToStartOfTurn(lore);

            //expires at start of turn
            QuickHPUpdate();
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestHubris()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Hubris");
            //{Lore} deals one target 4 energy damage and himself 1 psychic damage.
            DecisionSelectTargets = new Card[] { baron.CharacterCard };
            QuickHPStorage(baron, lore, ra, haka);
            PlayCard("Hubris");
            QuickHPCheck(-4,-1,0,0);

        }

        [Test()]
        public void TestLoveTriangle()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(lore, 19);
            SetHitPoints(ra, 14);
            SetHitPoints(haka, 17);
            SetHitPoints(baron, 21);

            Card otherRomance = PlayCard("Whirlwinding");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Love Triangle");
            Card triangle = PlayCard("LoveTriangle");
            AssertInHand(otherRomance);

            //{Lore} deals himself 2 psychic damage. Two character cards other than {Lore} regain 2 hp.
            PrintSeparator("Using Power of Love Triangle");
            DecisionSelectCards = new Card[] { ra.CharacterCard, haka.CharacterCard };
            QuickHPStorage(baron, lore, ra, haka);
            UsePower(triangle);
            QuickHPCheck(0, -2, 2, 2);

        }

        [Test()]
        public void TestPageTurner()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card otherMystery = PlayCard("Callback");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Page Turner");
            Card pageTurner = PlayCard("PageTurner");
            AssertInHand(otherMystery);

            Card raTop = ra.TurnTaker.Deck.TopCard;
            Card baronTop = baron.TurnTaker.Deck.TopCard;

            //At the end of your turn, reveal the top cards of 2 decks. You may replace or discard each card.
            PrintSeparator("Triggering End of Turn effect of Page Turner");
            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(ra.TurnTaker.Deck), new LocationChoice(baron.TurnTaker.Deck) };
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(ra.TurnTaker.Deck), new MoveCardDestination(baron.TurnTaker.Trash) };
            GoToEndOfTurn(lore);
            AssertOnTopOfDeck(raTop);
            AssertInTrash(baronTop);
            AssertNumberOfCardsInRevealed(ra, 0);
            AssertNumberOfCardsInRevealed(baron, 0);

        }

        [Test()]
        public void TestRallyingCry_NoActionInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card cry = PutInTrash("RallyingCry");

            GoToPlayCardPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Playing Rallying Cry");
            //If there is not a action card in play, draw a card.
            PlayCard(cry);
            QuickHandCheck(1);

            QuickHPStorage(baron);
            DealDamage(haka, baron, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(haka);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestRallyingCry_ActionInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("SaveYourself");

            Card cry = PutInTrash("RallyingCry");

            GoToPlayCardPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Playing Rallying Cry");
            //If there is an action card in play, increase damage dealt by hero targets by 1 until the start of your next turn.            
            PlayCard(cry);
            QuickHandCheck(0);

            //hero damage increased
            QuickHPStorage(baron);
            DealDamage(haka, baron, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //villain damage not increased
            QuickHPStorage(haka);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            GoToStartOfTurn(lore);

            //expires at start of turn
            QuickHPStorage(baron);
            DealDamage(haka, baron, 2, DamageType.Melee);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestRedHerring_NoMysteryInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "TheEnclaveOfTheEndlings");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card red = PutInTrash("RedHerring");

            GoToPlayCardPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Playing Red Herring");
            //If there is not a mystery card in play, draw a card.
            DecisionSelectCards = new Card[] { haka.CharacterCard };
            PlayCard(red);
            QuickHandCheck(1);

            QuickHPStorage(haka);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //baahsto is the lowest
            Card baahsto = PlayCard("Baahsto");
            Card gruum = PlayCard("Gruum");

            //no redirection should happen
            QuickHPStorage(haka.CharacterCard, baahsto, gruum);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0);

        }

        [Test()]
        public void TestRedHerring_MysteryInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "TheEnclaveOfTheEndlings");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card red = PutInTrash("RedHerring");
            PlayCard("PageTurner");


            GoToPlayCardPhase(lore);
            QuickHandStorage(lore);
            PrintSeparator("Playing Red Herring");
            //If there is a mystery card in play, select a target. Redirect all damage dealt to that target to the environment target with the lowest hp until the start of your next turn.            
            DecisionSelectCards = new Card[] { haka.CharacterCard };
            PlayCard(red);
            QuickHandCheck(0);

            QuickHPStorage(haka);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //baahsto is the lowest
            Card baahsto = PlayCard("Baahsto");
            Card gruum = PlayCard("Gruum");

            QuickHPStorage(haka.CharacterCard, baahsto, gruum);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);

            QuickHPStorage(ra.CharacterCard, baahsto, gruum);
            DealDamage(baron, ra, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0);

            //expires at start of turn
            GoToStartOfTurn(lore);
            QuickHPStorage(haka.CharacterCard, baahsto, gruum);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0);

        }

        [Test()]
        public void TestRevengement()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(baron, 9);
            SetHitPoints(haka, 8);
            SetHitPoints(ra, 11);

            Card otherAction = PlayCard("SaveYourself");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Revengement");
            Card revengement = PlayCard("Revengement");
            AssertInHand(otherAction);

            //Whenever a hero target with fewer than 10 hp is dealt damage, Lore may deal one target 1 cold damage.
            PrintSeparator("Triggering effect of Revengement");
            DecisionSelectTarget = baron.CharacterCard;

            //should trigger for hero under 10
            QuickHPStorage(baron, haka);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-1, -2);

            //should not trigger for villain under 10
            QuickHPStorage(baron);
            DealDamage(haka, baron, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //should not trigger for hero who starts above 10
            QuickHPStorage(baron, ra);
            DealDamage(baron, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2);

        }

        [Test()]
        public void TestRhetoricalBoost()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Rhetorical Boost");
            Card rhetorical = PlayCard("RhetoricalBoost");

            //Increase damage dealt by {Lore} by 1.
            PrintSeparator("Checking damage boost by Rhetorical Boost");

            //boost Lore
            QuickHPStorage(baron);
            DealDamage(lore, baron, 2, DamageType.Toxic);
            QuickHPCheck(-3);

            //don't boost other heroes
            QuickHPUpdate();
            DealDamage(ra, baron, 2, DamageType.Toxic);
            QuickHPCheck(-2);

            //Whenever a story card enters play, draw a card
            PrintSeparator("Checking draw when story card is played by Rhetorical Boost");

            QuickHandStorage(lore);
            //use power to discover a story
            UsePower(lore);
            QuickHandCheck(1);

        }

        [Test()]
        public void TestRhetoricalBoost_ReduceWhenNegative()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Rhetorical Boost");
            Card rhetorical = PlayCard("RhetoricalBoost");

            PlayCard("BlessingOfTheFifteenthStone");

            DecisionSelectCards = new Card[] { rhetorical };
            GoToEndOfTurn(lore);

            //If this card has negative HP, reduce damage dealt to {Lore} by 1 for each HP below zero.
            DealDamage(baron, rhetorical, 5, DamageType.Fire);
            AssertHitPoints(rhetorical, -1);
            QuickHPStorage(lore);
            DealDamage(baron, lore, 4, DamageType.Melee);
            QuickHPCheck(-3);

            DealDamage(baron, rhetorical, 2, DamageType.Fire);
            AssertHitPoints(rhetorical, -3);
            QuickHPStorage(lore);
            DealDamage(baron, lore, 4, DamageType.Melee);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestSaveYourself()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(baron, 9);
            SetHitPoints(haka, 8);
            SetHitPoints(ra, 11);

            Card otherAction = PlayCard("Revengement");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Save Yourself");
            Card saveYourself = PlayCard("SaveYourself");
            AssertInHand(otherAction);

            PrintSeparator("Triggering effect of Save Yourself");
            //The first time each turn a hero target would deal itself damage, you may redirect that damage to any target. If you do, discard a card or destroy this card. If this card is destroyed this way, it first deals Lore 2 toxic damage.
            Card discard = GetRandomCardFromHand(lore);
            DecisionSelectCards = new Card[] { baron.CharacterCard, discard, baron.CharacterCard, null };

            //first damage should be redirected
            QuickHPStorage(baron, haka);
            DealDamage(haka, haka, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);

            AssertInTrash(discard);

            //second damage should not trigger option
            QuickHPUpdate();
            DealDamage(haka, haka, 2, DamageType.Melee);
            QuickHPCheck(0, -2);

            //should reset next turn
            GoToNextTurn();
            QuickHPStorage(baron, haka, lore);
            DealDamage(haka, haka, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, -2);
            AssertInTrash(saveYourself);

        }

        [Test()]
        public void TestSpeedReader()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card otherEpic = PlayCard("FourFiftyOne");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Speed Reader");

            Card speedReader = PlayCard("SpeedReader");
            AssertInHand(otherEpic);

            //At the end of your turn, you may destroy one Story and play one Story. If a Story is destroyed this way, {Lore} deals one target 1 sonic damage. If a Story is played this way, {Lore} deals himself 1 irreducible sonic damage. You may then play another Story. If you do, destroy this card.
            Card storyInPlay = PlayCard("LoveTriangle");
            Card storyInHand1 = PutInHand("PageTurner");
            Card storyInHand2 = PutInHand("SaveYourself");

            DecisionSelectCards = new Card[] { storyInPlay, storyInHand1, baron.CharacterCard, storyInHand2 };
            QuickHPStorage(baron, lore);
            PrintSeparator("Triggering End of Turn Effeect on Speed Reader");
            GoToEndOfTurn(lore);
            AssertInTrash(storyInPlay, speedReader);
            QuickHPCheck(-1, -1);
            AssertIsInPlay(storyInHand1, storyInHand2);

        }

        [Test()]
        public void TestWhirlwinding()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(lore, 19);
            SetHitPoints(ra, 14);
            SetHitPoints(haka, 17);
            SetHitPoints(baron, 21);

            Card otherRomance = PlayCard("LoveTriangle");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Whirlwinding");
            Card whirlwinding = PlayCard("Whirlwinding");
            AssertInHand(otherRomance);

            //Select one hero character card. Until the start of your next turn, whenever that hero is dealt damage, Lore regains 1 hp and deals one target 2 psychic damage.
            PrintSeparator("Using Power of Whirlwinding");
            DecisionSelectCards = new Card[] { ra.CharacterCard, baron.CharacterCard };
            UsePower(whirlwinding);

            QuickHPStorage(baron, lore, ra, haka);
            DealDamage(baron, ra, 3, DamageType.Cold);
            QuickHPCheck(-2, 1, -3, 0);

            QuickHPUpdate();
            DealDamage(baron, ra, 3, DamageType.Cold);
            QuickHPCheck(-2, 1, -3, 0);

            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(baron, ra, 3, DamageType.Cold);
            QuickHPCheck(-2, 1, -3, 0);

            GoToStartOfTurn(lore);
            QuickHPUpdate();
            DealDamage(baron, ra, 3, DamageType.Cold);
            QuickHPCheck(0, 0, -3, 0);

        }

        [Test()]
        public void TestReadingGlasses()
        {
            SetupGameController("BaronBlade", "Studio29.Lore", "Ra", "Haka", "Unity", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card epilogue = PlayCard("Epilogue");
            Card callback = PutOnDeck("Callback");

            Card stealthBot = PutInTrash("StealthBot");
            Card raptorBot = PutInTrash("RaptorBot");

            GoToPlayCardPhase(lore);
            PrintSeparator("Playing Reading Glasses");
            PlayCard("ReadingGlasses");

            //an additional power this power phase
            GoToUsePowerPhase(lore);
            AssertPhaseActionCount(2);
            DecisionSelectPowers = new Card[] { epilogue, lore.CharacterCard };
            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(unity.TurnTaker.Trash) };
            DecisionSelectCards = new Card[] { stealthBot };
            RunActiveTurnPhase();
            AssertInPlayArea(lore, callback);
            AssertInPlayArea(unity, stealthBot);


        }

        [Test()]
        public void TestDefenderLoreLoads()
        {
            SetupGameController("BaronBlade", "Studio29.Lore/DefenderLoreCharacter", "Megalopolis");
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(lore);
            Assert.IsInstanceOf(typeof(DefenderLoreCharacterCardController), lore.CharacterCardController);

            Assert.AreEqual(27, lore.CharacterCard.HitPoints);

            StartGame();

        }

        [Test()]
        [Sequential()]
        public void TestDefenderLoreInnatePower([Values(0,1,2,3)] int numStories)
        {
            SetupGameController("BaronBlade", "Studio29.Lore/DefenderLoreCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            IEnumerable<Card> allStories = FindCardsWhere((Card c) => c.DoKeywordsContain("story"));
            IEnumerable<Card> stories = allStories.Where(c => c.GetKeywords() != null).GroupBy(c => c.GetKeywords().Where(s => s != "story").FirstOrDefault()).Select(grp => grp.FirstOrDefault()).Take(numStories);
            PlayCards(stories);
            DestroyNonCharacterVillainCards();

            GoToUsePowerPhase(lore);

            //{Lore} deals 1 target X projectile damage, where x = the number of your story cards in play plus 1.
            DecisionSelectTargets = new Card[] { baron.CharacterCard };
            QuickHPStorage(baron);
            UsePower(lore);
            QuickHPCheck(-numStories - 1);

        }
    }
}
