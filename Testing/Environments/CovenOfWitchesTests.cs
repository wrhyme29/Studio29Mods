using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Studio29.CovenOfWitches;
using System;
using Handelabra;

namespace Studio29Tests
{
    [TestFixture()]
    public class CovenOfWitchesTests : CustomBaseTest
    {

        #region CovenOfWitchesHelperFunctions

        protected TurnTakerController coven { get { return FindEnvironment(); } }
        protected bool IsCurse(Card card)
        {
            return card != null && card.DoKeywordsContain("curse");
        }

        protected bool IsWard(Card card)
        {
            return card != null && card.DoKeywordsContain("ward");
        }

        #endregion

        [Test()]
        public void TestCovenOfWitchesWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_Curses_IsCurse([Values("CurseOfArwen", "CurseOfLillith", "CurseOfSeren", "CurseOfSybil", "CurseOfEvanora")] string curse)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            GoToPlayCardPhase(coven);

            Card card = PlayCard(curse);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "curse", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Wards_IsWard([Values("TrippedWards")] string wards)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            GoToPlayCardPhase(coven);

            Card card = PlayCard(wards);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "ward", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Witches_IsWitch([Values("SybilTheClairvoyant", "EvanoraTheBargainer")] string witch)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            GoToPlayCardPhase(coven);

            Card card = PlayCard(witch);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "witch", false);
        }

        [Test()]
        public void TestCurseOfArwen_NoDamageOnOwnTurn()
        {

            SetupGameController("BaronBlade", "Ra", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfArwen = PlayCard("CurseOfArwen");

            //  Hero characters may not deal damage on their own turn.

            GoToPlayCardPhase(ra);

            PrintSpecialStringsForCard(curseOfArwen);
            AssertCardSpecialString(curseOfArwen, 0, "Ra cannot deal damage this turn.");

            // ra should not be able to deal fire since its his turn
            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheckZero();

            // haka should be able to deal melee since its not his turn
            QuickHPStorage(baron);
            DealDamage(haka, baron, 3, DamageType.Melee);
            QuickHPCheck(-3);

            GoToPlayCardPhase(haka);

            PrintSpecialStringsForCard(curseOfArwen);
            AssertCardSpecialString(curseOfArwen, 0, "Haka cannot deal damage this turn.");

            // haka should not be able to deal melee since its his turn
            QuickHPStorage(baron);
            DealDamage(haka, baron, 3, DamageType.Melee);
            QuickHPCheckZero();

            // ra should be able to deal fire since its not his turn
            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestCurseOfArwen_NoDamageOnOwnTurn_Sentinels()
        {

            SetupGameController("BaronBlade", "TheSentinels", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfArwen = PlayCard("CurseOfArwen");

            //  Hero characters may not deal damage on their own turn.

            GoToPlayCardPhase(sentinels);

            PrintSpecialStringsForCard(curseOfArwen);
            AssertCardSpecialString(curseOfArwen, 0, "The Sentinels cannot deal damage this turn.");

            QuickHPStorage(baron);
            DealDamage(medico, baron, 3, DamageType.Energy);
            QuickHPCheckZero();

            QuickHPStorage(baron);
            DealDamage(mainstay, baron, 3, DamageType.Melee);
            QuickHPCheckZero();

            QuickHPStorage(baron);
            DealDamage(writhe, baron, 3, DamageType.Infernal);
            QuickHPCheckZero();

            QuickHPStorage(baron);
            DealDamage(idealist, baron, 3, DamageType.Psychic);
            QuickHPCheckZero();

            GoToPlayCardPhase(haka);

            QuickHPStorage(baron);
            DealDamage(medico, baron, 3, DamageType.Energy);
            QuickHPCheck(-3);

            QuickHPStorage(baron);
            DealDamage(mainstay, baron, 3, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(baron);
            DealDamage(writhe, baron, 3, DamageType.Infernal);
            QuickHPCheck(-3);

            QuickHPStorage(baron);
            DealDamage(idealist, baron, 3, DamageType.Psychic);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestCurseOfArwen_NoDamageOnOwnTurn_CharactersOnly()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfArwen = PlayCard("CurseOfArwen");

            //  Hero characters may not deal damage on their own turn.

            GoToUsePowerPhase(unity);

            PrintSpecialStringsForCard(curseOfArwen);
            AssertCardSpecialString(curseOfArwen, 0, "Unity cannot deal damage this turn.");

            PlayCard("PlatformBot");
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            GoToEndOfTurn(unity);
            QuickHPCheck(-3);

            QuickHPStorage(baron);
            DealDamage(unity, baron, 3, DamageType.Energy);
            QuickHPCheckZero();

        }

        [Test()]
        public void TestCurseOfArwen_LillithEntersPlay_DestroySelf()
        {

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            Card curseOfArwen = PlayCard("CurseOfArwen");
            Card curseOfLillith = GetCard("CurseOfLillith");

            //  If Curse of Lillith is ever in play, destroy it or destroy this card

            DecisionSelectCards = new Card[] { curseOfArwen };
            PlayCard(curseOfLillith);

            AssertInPlayArea(coven, curseOfLillith);
            AssertInTrash(coven, curseOfArwen);

        }

        [Test()]
        public void TestCurseOfArwen_LillithEntersPlay_DestroyLillith()
        {

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            Card curseOfArwen = PlayCard("CurseOfArwen");
            Card curseOfLillith = GetCard("CurseOfLillith");

            //  If Curse of Lillith is ever in play, destroy it or destroy this card

            DecisionSelectCards = new Card[] { curseOfLillith };
            PlayCard(curseOfLillith);

            AssertInPlayArea(coven, curseOfArwen);
            AssertInTrash(coven, curseOfLillith);

        }

        [Test()]
        public void TestCurseOfArwen_LillithEntersPlay_StartOfTurn_Discard()
        {

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            Card curseOfArwen = PlayCard("CurseOfArwen");
            DiscardAllCards(haka);

            // At the start of the Environment turn, 1 Player may discard 2 cards to destroy this card.
            IEnumerable<Card> cardsToDiscard = legacy.HeroTurnTaker.Hand.Cards.Take(2);
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker };
            DecisionSelectCards = cardsToDiscard;
            AssertNextDecisionChoices(included: new TurnTaker[] { ra.TurnTaker, legacy.TurnTaker }, notIncluded: new TurnTaker[] { haka.TurnTaker });
            QuickHandStorage(ra, legacy, haka);
            GoToStartOfTurn(coven);
            QuickHandCheck(0, -2, 0);

            AssertInTrash(curseOfArwen);

        }

        [Test()]
        public void TestCurseOfArwen_LillithEntersPlay_StartOfTurn_NoDiscard()
        {

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            Card curseOfArwen = PlayCard("CurseOfArwen");

            // At the start of the Environment turn, 1 Player may discard 2 cards to destroy this card.
            DecisionSelectTurnTakers = new TurnTaker[] { null };

            QuickHandStorage(ra, legacy, haka);
            GoToStartOfTurn(coven);
            QuickHandCheck(0, 0, 0);

            AssertInPlayArea(coven, curseOfArwen);

        }

        [Test()]
        public void TestCurseOfArwen_LillithEntersPlay_StartOfTurn_NotEnoughCards()
        {

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            Card curseOfArwen = PlayCard("CurseOfArwen");
            DiscardAllCards(ra, legacy, haka);
            GoToEndOfTurn(haka);

            // At the start of the Environment turn, 1 Player may discard 2 cards to destroy this card.
            AssertNextMessage("No player has enough cards in hand to destroy " + curseOfArwen.Title);
            GoToStartOfTurn(coven);
            AssertInPlayArea(coven, curseOfArwen);

        }

        [Test()]
        public void TestCurseOfLillith_NoDamageOnOtherTurns()
        {
            SetupGameController("BaronBlade", "Ra", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfLillith = PlayCard("CurseOfLillith");

            //  Hero characters may only deal damage on their own turn.

            GoToPlayCardPhase(ra);

            PrintSpecialStringsForCard(curseOfLillith);
            AssertCardSpecialString(curseOfLillith, 0, "Haka and Legacy cannot deal damage this turn.");

            // ra should  be able to deal fire since its his turn
            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-3);

            // haka should not be able to deal melee since its not his turn
            QuickHPStorage(baron);
            DealDamage(haka, baron, 3, DamageType.Melee);
            QuickHPCheckZero();

            GoToPlayCardPhase(haka);

            PrintSpecialStringsForCard(curseOfLillith);
            AssertCardSpecialString(curseOfLillith, 0, "Ra and Legacy cannot deal damage this turn.");

            // haka should  be able to deal melee since its his turn
            QuickHPStorage(baron);
            DealDamage(haka, baron, 3, DamageType.Melee);
            QuickHPCheck(-3);

            // ra should not be able to deal fire since its not his turn
            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheckZero();

        }

        [Test()]
        public void TestCurseOfLillith_NoDamageOnOtherTurns_CharactersOnly()
        {
            SetupGameController("BaronBlade", "Unity", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfLillith = PlayCard("CurseOfLillith");

            //  Hero characters may only deal damage on their own turn.

            GoToUsePowerPhase(unity);
            Card raptorBot = PlayCard("RaptorBot");

            QuickHPStorage(baron);
            DealDamage(unity, baron, 3, DamageType.Energy);
            QuickHPCheck(-3);

            QuickHPStorage(baron);
            DealDamage(raptorBot, baron, 3, DamageType.Energy);
            QuickHPCheck(-3);

            GoToPlayCardPhase(haka);

            QuickHPStorage(baron);
            DealDamage(unity, baron, 3, DamageType.Energy);
            QuickHPCheckZero();

            QuickHPStorage(baron);
            DealDamage(raptorBot, baron, 3, DamageType.Energy);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestCurseOfLillith_NoDamageOnOtherTurns_Sentinels()
        {
            SetupGameController("BaronBlade", "TheSentinels", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfLillith = PlayCard("CurseOfLillith");

            // Heroes may only deal damage on their own turn.

            GoToPlayCardPhase(sentinels);

            QuickHPStorage(baron);
            DealDamage(medico, baron, 3, DamageType.Energy);
            QuickHPCheck(-3);

            QuickHPStorage(baron);
            DealDamage(mainstay, baron, 3, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(baron);
            DealDamage(writhe, baron, 3, DamageType.Infernal);
            QuickHPCheck(-3);

            QuickHPStorage(baron);
            DealDamage(idealist, baron, 3, DamageType.Psychic);
            QuickHPCheck(-3);

            GoToPlayCardPhase(haka);

            PrintSpecialStringsForCard(curseOfLillith);
            AssertCardSpecialString(curseOfLillith, 0, "The Sentinels and Legacy cannot deal damage this turn.");

            QuickHPStorage(baron);
            DealDamage(medico, baron, 3, DamageType.Energy);
            QuickHPCheckZero();

            QuickHPStorage(baron);
            DealDamage(mainstay, baron, 3, DamageType.Melee);
            QuickHPCheckZero();

            QuickHPStorage(baron);
            DealDamage(writhe, baron, 3, DamageType.Infernal);
            QuickHPCheckZero();

            QuickHPStorage(baron);
            DealDamage(idealist, baron, 3, DamageType.Psychic);
            QuickHPCheckZero();
        }

        [Test()]
        public void TestCurseOfLillith_ArwenEntersPlay_DestroySelf()
        {

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            Card curseOfArwen = GetCard("CurseOfArwen");
            Card curseOfLillith = PlayCard("CurseOfLillith");

            //  If Curse of Arwen is ever in play, destroy it or destroy this card

            DecisionSelectCards = new Card[] { curseOfLillith };
            PlayCard(curseOfArwen);

            AssertInPlayArea(coven, curseOfArwen);
            AssertInTrash(coven, curseOfLillith);

        }

        [Test()]
        public void TestCurseOfLillith_ArwenEntersPlay_DestroyLillith()
        {

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            Card curseOfArwen = GetCard("CurseOfArwen");
            Card curseOfLillith = PlayCard("CurseOfLillith");

            //  If Curse of Arwen is ever in play, destroy it or destroy this card

            DecisionSelectCards = new Card[] { curseOfArwen };
            PlayCard(curseOfArwen);

            AssertInPlayArea(coven, curseOfLillith);
            AssertInTrash(coven, curseOfArwen);

        }

        [Test()]
        public void TestCurseOfLillith_StartOfTurn_EveryoneDiscards()
        {
            SetupGameController("BaronBlade", "Ra", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfLillith = PlayCard("CurseOfLillith");

            // At the start of the Environment turn, each Player may discard 1 card each to destroy this card.

            DecisionYesNo = true;

            QuickHandStorage(ra, haka, legacy);
            GoToStartOfTurn(coven);
            QuickHandCheck(-1, -1, -1);
            AssertInTrash(coven, curseOfLillith);
        }

        [Test()]
        public void TestCurseOfLillith_StartOfTurn_SkipDiscards()
        {
            SetupGameController("BaronBlade", "Ra", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfLillith = PlayCard("CurseOfLillith");
            // At the start of the Environment turn, each Player may discard 1 card each to destroy this card.

            DecisionYesNo = false;

            QuickHandStorage(ra, haka, legacy);
            GoToStartOfTurn(coven);
            QuickHandCheckZero();
            AssertInPlayArea(coven, curseOfLillith);
        }

        [Test()]
        public void TestCurseOfLillith_StartOfTurn_NotEnoughCards()
        {
            SetupGameController("BaronBlade", "Ra", "Haka", "Legacy", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();


            Card curseOfLillith = PlayCard("CurseOfLillith");

            GoToEndOfTurn(legacy);
            DiscardAllCards(haka);

            // At the start of the Environment turn, each Player may discard 1 card each to destroy this card.
            AssertNextMessage("Not all players have enough cards to discard for " + curseOfLillith.Title + ".");
            GoToStartOfTurn(coven);
            AssertInPlayArea(coven, curseOfLillith);
        }

        [Test()]
        public void TestCurseOfSeren_NoDamageTurnPlayed()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfSeren = PlayCard("CurseOfSeren");

            //Targets may not deal damage the turn they enter play.

            Card bladeBattalion = PlayCard("BladeBattalion");

            PrintSpecialStringsForCard(curseOfSeren);
            AssertCardSpecialString(curseOfSeren, 0, $"{bladeBattalion.Title} cannot deal damage this turn.");

            QuickHPStorage(haka);
            DealDamage(bladeBattalion, haka, 5, DamageType.Energy);
            QuickHPCheckZero();

            GoToUsePowerPhase(unity);
            QuickHPStorage(haka);
            DealDamage(bladeBattalion, haka, 5, DamageType.Energy);
            QuickHPCheck(-5);

            Card platformBot = PlayCard("PlatformBot");

            PrintSpecialStringsForCard(curseOfSeren);
            AssertCardSpecialString(curseOfSeren, 0, $"{platformBot.Title} cannot deal damage this turn.");

            QuickHPStorage(baron);
            DealDamage(platformBot, baron, 3, DamageType.Energy);
            QuickHPCheckZero();

            GoToPlayCardPhase(haka);
            QuickHPStorage(baron);
            DealDamage(platformBot, baron, 3, DamageType.Energy);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestCurseOfSeren_StartOfTurn_AllHit()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfSeren = PlayCard("CurseOfSeren");
            GoToEndOfTurn(tempest);

            // At the start of the Environment turn, this cards deals all targets 2 infernal damage. If all targets were dealt damage this way, destroy this card.

            QuickHPStorage(baron, unity, haka, tempest);
            GoToStartOfTurn(coven);
            QuickHPCheck(-2, -2, -2, -2);
            AssertInTrash(coven, curseOfSeren);
        }

        [Test()]
        public void TestCurseOfSeren_StartOfTurn_NotAllHit()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card curseOfSeren = PlayCard("CurseOfSeren");
            GoToEndOfTurn(tempest);

            // At the start of the Environment turn, this cards deals all targets 2 infernal damage. If all targets were dealt damage this way, destroy this card.
            QuickHPStorage(baron.CharacterCard, mdp, unity.CharacterCard, haka.CharacterCard, tempest.CharacterCard);
            GoToStartOfTurn(coven);
            QuickHPCheck(0,-2, -2, -2, -2);
            AssertInPlayArea(coven, curseOfSeren);
        }

        [Test()]
        public void TestCurseOfSybil_SayYes()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfSybil = PlayCard("CurseOfSybil");

            // When a villain card would enter play, you may discard it instead and destroy this card.
            // When this card is destroyed, this card deals all targets {H} infernal damage.

            DecisionYesNo = true;

            QuickHPStorage(baron, unity, haka, tempest);
            Card bladeBattalion = PlayCard("BladeBattalion");
            QuickHPCheck(-3, -3, -3, -3);
            AssertInTrash(coven, curseOfSybil);
            AssertInTrash(baron, bladeBattalion);
        }

        [Test()]
        public void TestCurseOfSybil_SayNo()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfSybil = PlayCard("CurseOfSybil");

            // When a villain card would enter play, you may discard it instead and destroy this card.
            // When this card is destroyed, this card deals all targets {H} infernal damage.

            DecisionYesNo = false;

            QuickHPStorage(baron, unity, haka, tempest);
            Card bladeBattalion = PlayCard("BladeBattalion");
            QuickHPCheckZero();
            AssertInPlayArea(coven, curseOfSybil);
            AssertInPlayArea(baron, bladeBattalion);
        }

        [Test()]
        public void TestCurseOfSybil_OutsideDestruction()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfSybil = PlayCard("CurseOfSybil");

            // When this card is destroyed, this card deals all non-environment targets {H} infernal damage.

            QuickHPStorage(baron, unity, haka, tempest);
            DestroyCard(curseOfSybil, unity.CharacterCard);
            QuickHPCheck(-3, -3, -3, -3);
            AssertInTrash(coven, curseOfSybil);

        }

        [Test()]
        public void TestCurseOfEvanora_DrawCardsDealDamage()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card curseOfEvanora = PlayCard("CurseOfEvanora");
            TokenPool evanoraTokenPool = FindTokenPool(curseOfEvanora.Identifier, "CurseOfEvanoraPool");

            AssertTokenPoolCount(evanoraTokenPool, 0);

            Card unityDiscard = GetRandomCardFromHand(unity);
            Card unityTop = unity.TurnTaker.Deck.TopCard;
            Card hakaTop = haka.TurnTaker.Deck.TopCard;
            Card tempestDiscard = GetRandomCardFromHand(tempest);
            Card tempestTop = tempest.TurnTaker.Deck.TopCard;

            // At the start of any hero's turn, their player may discard a card. If they do, they may draw a card. If a card was drawn this way, put a token on this card.

            DecisionSelectCards = new Card[] { unityDiscard, null, tempestDiscard };
            DecisionSelectTurnTakers = new TurnTaker[] { null };
            DecisionAutoDecideIfAble = true;

            QuickHandStorage(unity);
            GoToStartOfTurn(unity);
            QuickHandCheckZero();
            AssertInTrash(unity, unityDiscard);
            AssertInHand(unity, unityTop);
            AssertTokenPoolCount(evanoraTokenPool, 1);

            QuickHandStorage(haka);
            GoToStartOfTurn(haka);
            QuickHandCheckZero();
            AssertOnTopOfDeck(haka, hakaTop);
            AssertTokenPoolCount(evanoraTokenPool, 1);

            QuickHandStorage(tempest);
            GoToStartOfTurn(tempest);
            QuickHandCheckZero();
            AssertInTrash(tempest, tempestDiscard);
            AssertInHand(tempest, tempestTop);
            AssertTokenPoolCount(evanoraTokenPool, 2);

            // At the end of the environment turn, this card deals each hero target X + 1 infernal damage, where X is the number of tokens on this card.
            QuickHPStorage(baron, unity, haka, tempest);
            GoToEndOfTurn(coven);
            QuickHPCheck(0, -3, -3, -3);

        }

        [Test()]
        public void TestCurseOfEvanora_StartOfTurn_SelectHero()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(tempest);

            Card curseOfEvanora = PlayCard("CurseOfEvanora");

            // At the start of the Environment turn, 1 Hero may discard their hand to destroy this card.

            DecisionSelectTurnTaker = haka.TurnTaker;
            int numCardsInHakaHand = haka.HeroTurnTaker.NumberOfCardsInHand;
            QuickHandStorage(unity, haka, tempest);
            GoToStartOfTurn(coven);
            QuickHandCheck(0, -1 * numCardsInHakaHand, 0);
            AssertInTrash(coven, curseOfEvanora);
            
        }

        [Test()]
        public void TestCurseOfEvanora_StartOfTurn_DontSelectHero()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(tempest);

            Card curseOfEvanora = PlayCard("CurseOfEvanora");

            // At the start of the Environment turn, 1 Hero may discard their hand to destroy this card.

            DecisionSelectTurnTakers = new TurnTaker[] { null };
            QuickHandStorage(unity, haka, tempest);
            GoToStartOfTurn(coven);
            QuickHandCheckZero();
            AssertInPlayArea(coven, curseOfEvanora);

        }

        [Test()]
        [Sequential]
        public void TestTrippedWards_RevealCurses([Values(new string[] { }, new string[] { "Unity" }, new string[] { "Unity", "Tempest" })] string[] extraHeroes)
        {
            List<string> gameSetup = new List<string>() { "BaronBlade", "Ra", "Legacy", "Haka" };
            gameSetup.AddRange(extraHeroes);
            gameSetup.Add("Studio29.CovenOfWitches");
            SetupGameController(gameSetup);
            StartGame();

            // move lillith to the trash to guarantee no MAD with Arwen
            MoveCard(coven, "CurseOfLillith", coven.TurnTaker.Trash);

            // When this card enters play, reveal cards from the top of the Environment deck until {H - 2} curses are revealed. Put them in play. Discard the other revealed cards.
            PlayCard("TrippedWards");

            AssertNumberOfCardsInPlay(c => IsCurse(c) && c.Location == coven.TurnTaker.PlayArea, GameController.Game.H - 2);
        }

        [Test()]
        [Sequential]
        public void TestTrippedWards_StartOfTurn([Values(1,2,3,4)] int numCursesToPlay)
        {

            SetupGameController("BaronBlade", "Ra", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(tempest);

            // move lillith to the trash to guarantee no MAD with Arwen
            MoveCard(coven, "CurseOfLillith", coven.TurnTaker.Trash);

            Card trippedWards = PlayCard("TrippedWards");


            IEnumerable<Card> cursesToPlay = coven.TurnTaker.Deck.Cards.Where(c => IsCurse(c)).TakeRandom(numCursesToPlay - 1, GameController.Game.RNG);
            PlayCards(cursesToPlay);

            // At the start of the Environment turn, destroy this card.
            //When this card is destroyed, this card deals all non-environment targets X infernal damage, where X is the number of curses in play.

            int expectedHealthChange = -1 * (numCursesToPlay + (FindCardsWhere(c => c.Identifier == "CurseOfSeren" && c.IsInPlayAndHasGameText).Any() ? 2 : 0));
            QuickHPStorage(baron, ra, haka, tempest);
            GoToStartOfTurn(coven);
            QuickHPCheck(expectedHealthChange, expectedHealthChange, expectedHealthChange, expectedHealthChange);
            AssertInTrash(trippedWards);
        }

        [Test()]
        public void TestTrippedWards_OutsideDestruction()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();


            Card trippedWards = PlayCard("TrippedWards");

            DestroyCards(c => IsCurse(c));

            PlayCard("CurseOfSybil");
            PlayCard("CurseOfArwen");


            //When this card is destroyed, this card deals all non-environment targets X infernal damage, where X is the number of curses in play.

            QuickHPStorage(baron, unity, haka, tempest);
            DestroyCard(trippedWards, unity.CharacterCard);
            QuickHPCheck(-2, -2, -2, -2);
            AssertInTrash(coven, trippedWards);

        }

        [Test()]
        [Sequential]
        public void TestWitches_PlayRelatedCurses([Values("SybilTheClairvoyant", "EvanoraTheBargainer", "ArwenTheScheming", "SerenTheProfound", "LillithTheAccursed")] string witch,
                                [Values("CurseOfSybil", "CurseOfEvanora", "CurseOfArwen", "CurseOfSeren", "CurseOfLillith")] string curse)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.CovenOfWitches");
            StartGame();

            Card witchCard = PlayCard(witch);
            AssertInPlayArea(coven, witchCard);
            Card curseCard = GetCard(curse);
            AssertInPlayArea(coven, curseCard);
        }

        [Test()]
        public void TestSybilTheClairvoyant_EndOfTurn()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToPlayCardPhase(coven);
            PlayCard("SybilTheClairvoyant");

            Card bladeBattalion = MoveCard(baron, "BladeBattalion", baron.TurnTaker.Deck);
            Card elementalRedistrubitor = MoveCard(baron, "ElementalRedistributor", baron.TurnTaker.Deck);

            // At the end of the Environment turn, reveal the top 2 cards of the Villain deck. Put 1 on the top and 1 on the bottom of the Villain deck.
            DecisionSelectCards = new Card[] { bladeBattalion, elementalRedistrubitor };
            GoToEndOfTurn(coven);
            AssertOnTopOfDeck(baron, bladeBattalion);
            AssertOnBottomOfDeck(baron, elementalRedistrubitor);
            AssertNumberOfCardsInRevealed(baron, 0);
        }

        [Test()]
        public void TestSybilTheClairvoyant_StartOfTurn()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(tempest);
            PlayCard("SybilTheClairvoyant");

            Card bladeBattalion = MoveCard(baron, "BladeBattalion", baron.TurnTaker.Deck);

            // At the start of the Environment turn, play the top card of the Villain deck.
            GoToStartOfTurn(coven);
            AssertInPlayArea(baron, bladeBattalion);
        }


        [Test()]
        public void TestSerenTheProfound_StartOfTurn()
        {

            SetupGameController("BaronBlade", "Unity", "Haka", "Tempest", "Studio29.CovenOfWitches");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToEndOfTurn(tempest);
            Card serenWitch = PlayCard("SerenTheProfound");
            SetHitPoints(serenWitch, 2);

            DecisionSelectTurnTakers = new TurnTaker[] { null };
            GoToStartOfTurn(coven);
            AssertIsAtMaxHP(serenWitch);
        }



    }
}
