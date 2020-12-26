using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Studio29.TheTamer;
using System.Collections.Generic;
using System.Linq;

namespace Studio29Tests
{
    [TestFixture()]
    public class TheTamerTests : BaseTest
    {
        #region TheTamerHelperFunctions
        protected HeroTurnTakerController tamer { get { return FindHero("TheTamer"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(tamer.CharacterCard, 1);
            DealDamage(villain, tamer, 2, DamageType.Melee);
        }

        private bool IsLion(Card card)
        {
            return card.DoKeywordsContain("lion");
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
        public void TestTheTamerLoads()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(tamer);
            Assert.IsInstanceOf(typeof(TheTamerCharacterCardController), tamer.CharacterCardController);

            Assert.AreEqual(28, tamer.CharacterCard.HitPoints);
        }
        [Test()]
        [Sequential]
        public void DecklistTestEquipment_IsEquipment([Values("ChairOfConfusion", "NemeanCoat")] string equipment)
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);

            Card card = PlayCard(equipment);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "equipment", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTestLion_IsLion([Values("PhileonTheDestroyer", "LingorthTheLighthearted", "MotherOfThePack", "GildeasTheGood")] string lion)
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);

            Card card = PlayCard(lion);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "lion", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTestOngoing_IsOngoing([Values("GrandFinale", "ThreeRings", "KittyGloves")] string ongoing)
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);

            Card card = PlayCard(ongoing);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "ongoing", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTestLimited_IsLimited([Values("ChairOfConfusion", "GrandFinale", "ThreeRings", "NemeanCoat")] string limited)
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);

            Card card = PlayCard(limited);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "limited", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTestOneShot_IsOneShot([Values("ElementalWhip", "TapOut", "SendInTheClowns", "HereKittyKitty", "RingOfFire", "Catsnack", "BalancingAct", "WhippingWhiskers")] string oneshot)
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);

            Card card = PlayCard(oneshot);
            AssertInTrash(card);
            AssertCardHasKeyword(card, "one-shot", false);
        }

        [Test()]
        public void TestTheTamerPower()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            Card phileon = PlayCard("PhileonTheDestroyer");
            Card gildeas = PlayCard("GildeasTheGood");
            GoToUsePowerPhase(tamer);
            //Until the start of your next turn, the first time a Lion is destroyed, it deals 1 target 3 melee damage. You may return that Lion to your hand.
            UsePower(tamer);

            DecisionYesNo = true;
            DecisionSelectTarget = ra.CharacterCard;

            QuickHPStorage(ra);
            DestroyCard(phileon, baron.CharacterCard);
            QuickHPCheck(-3);
            AssertInHand(phileon);

            //should only be the first lion destroyed

            DestroyCard(gildeas, baron.CharacterCard);
            QuickHPCheck(0);
            AssertInTrash(gildeas);


        }

        [Test()]
        public void TestTheTamerPower_ExpiresStartOfTurn()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            Card phileon = PlayCard("PhileonTheDestroyer");
            Card gildeas = PlayCard("GildeasTheGood");
            GoToUsePowerPhase(tamer);
            //Until the start of your next turn, the first time a Lion is destroyed, it deals 1 target 3 melee damage. You may return that Lion to your hand.
            UsePower(tamer);

            DecisionYesNo = true;
            DecisionSelectTarget = ra.CharacterCard;
            GoToStartOfTurn(tamer);
            QuickHPStorage(ra);
            DestroyCard(gildeas, baron.CharacterCard);
            QuickHPCheck(0);
            AssertInTrash(gildeas);


        }

        [Test()]
        public void TestTheTamerPower_OptionalMove()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            Card phileon = PlayCard("PhileonTheDestroyer");
            GoToUsePowerPhase(tamer);
            //Until the start of your next turn, the first time a Lion is destroyed, it deals 1 target 3 melee damage. You may return that Lion to your hand.
            UsePower(tamer);

            DecisionYesNo = false;
            DecisionSelectTarget = ra.CharacterCard;

            QuickHPStorage(ra);
            DestroyCard(phileon, baron.CharacterCard);
            QuickHPCheck(-3);
            AssertInTrash(phileon);

        }

        [Test()]
        public void TestTheTamerIncap1()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Unity", "CaptainCosmic", "Megalopolis");
            StartGame();

            DestroyNonCharacterVillainCards();


            SetupIncap(baron);
            AssertIncapacitated(tamer);

            Card crest = PlayCard("CosmicCrest");
            Card raptor = PlayCard("RaptorBot");
            //One non-character hero target deals 1 target 2 radiant damage.
            GoToUseIncapacitatedAbilityPhase(tamer);
            DecisionSelectCards = new Card[] { raptor, baron.CharacterCard };
            QuickHPStorage(baron);
            UseIncapacitatedAbility(tamer, 0);
            QuickHPCheck(-2);
            

        }

        [Test()]
        public void TestBalancingAct()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            IEnumerable<Card> lions = FindCardsWhere((Card c) => IsLion(c)).Take(3);
            PlayCards(lions);

            //If there are more Lions than villain targets in play, each Lion may deal one target 1 melee damage
            //One target dealt damage this way deals one Lion 2 melee damage

            DecisionSelectCards = new Card[] { lions.ElementAt(0), baron.CharacterCard, lions.ElementAt(1), ra.CharacterCard, baron.CharacterCard, baron.CharacterCard,  lions.ElementAt(2)};
            DecisionYesNo = true;
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, lions.ElementAt(2));
            PlayCard("BalancingAct");
            QuickHPCheck(-2, -1, -2);



        }

        [Test()]
        public void TestBalancingAct_LessThanVillains()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            IEnumerable<Card> lions = FindCardsWhere((Card c) => IsLion(c)).Take(1);
            PlayCards(lions);

            //If there are more Lions than villain targets in play, each Lion may deal one target 1 melee damage
            //One target dealt damage this way deals one Lion 2 melee damage

            DecisionYesNo = true;
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, lions.ElementAt(0));
            PlayCard("BalancingAct");
            QuickHPCheck(0,0,0);

        }

        [Test()]
        public void TestCatsnack_Discard()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            Card catsnack = PutInTrash("Catsnack");
            IEnumerable<Card> lions = FindCardsWhere((Card c) => IsLion(c)).Take(3);
            PlayCards(lions);
            //Discard a card. If you do, each Lion in play deals 1 target 1 melee damage.
            DecisionSelectCard = tamer.HeroTurnTaker.Hand.TopCard;
            DecisionSelectTarget = haka.CharacterCard;
            DecisionAutoDecide = SelectionType.CardToDealDamage;
            QuickHPStorage(baron, tamer, haka, ra);
            PlayCard(catsnack);
            QuickHPCheck(0, 0, -3, 0);
            
        }


        [Test()]
        public void TestCatsnack_NoDiscard()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            Card catsnack = PutInTrash("Catsnack");
            IEnumerable<Card> lions = FindCardsWhere((Card c) => IsLion(c)).Take(3);
            PlayCards(lions);
            //Discard a card. If you do, each Lion in play deals 1 target 1 melee damage.
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            DecisionSelectTarget = haka.CharacterCard;
            QuickHPStorage(baron, tamer, haka, ra);
            PlayCard(catsnack);
            QuickHPCheck(0, 0, 0, 0);

        }

        [Test()]
        public void TestChairOfConfusion()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            Card chair = PlayCard("ChairOfConfusion");
            Card phileon = PlayCard("PhileonTheDestroyer");

            //Reduce damage dealt to {TheTamer} by Lions by 1.
            QuickHPStorage(tamer);
            DealDamage(phileon, tamer, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //check only for the tamer
            QuickHPStorage(ra);
            DealDamage(phileon, ra, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //check only from lions
            QuickHPStorage(tamer);
            DealDamage(ra, tamer, 2, DamageType.Fire);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestElementalWhip()
        {
            SetupGameController(new string[] { "BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis" });
            StartGame();

            GoToPlayCardPhase(tamer);
            IEnumerable<Card> lions = FindCardsWhere((Card c) => IsLion(c)).Take(3);
            PlayCards(lions);

            AddImmuneToDamageTrigger(tamer, true, false, true);

            //Deal each Lion card in play 1 energy damage.
            QuickHPStorage(lions.Concat(FindCardsWhere((Card c) => c.IsCharacter && !c.IsIncapacitatedOrOutOfGame)).ToArray());
            PlayCard("ElementalWhip");
            QuickHPCheck(-1, -1, -1, 0, 0, 0, 0);
           

        }

        [Test()]
        public void TestGildeasTheGood()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            
            Card gildeas = PlayCard("GildeasTheGood");

            //When this card is dealt damage, it deals {TheTamer} 1 melee damage. Then, draw a card.

            QuickHPStorage(baron, tamer, haka, ra);
            QuickHandStorage(tamer);
            DealDamage(baron, gildeas, 2, DamageType.Fire);
            QuickHPCheck(0, -1, 0, 0);
            QuickHandCheck(1);


        }

        [Test()]
        public void TestGrandFinale()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            int hp = tamer.CharacterCard.HitPoints.Value;

            PlayCard("GrandFinale");
            DecisionYesNo = true;
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron);
            DealDamage(baron, tamer, hp, DamageType.Fire);
            QuickHPCheck(-10);

        }

        [Test()]
        public void TestHereKittyKitty()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            Card lionToPlay = FindCardsWhere((Card c) => IsLion(c) && tamer.TurnTaker.Deck.HasCard(c)).First();
            Card handCard = PutInHand("GildeasTheGood");
            DecisionSelectCards = new Card[] { lionToPlay, handCard };
            PlayCard("HereKittyKitty");
            AssertInPlayArea(tamer, lionToPlay);
            AssertNotInHand(handCard);
          

        }

        [Test()]
        public void TestHereKittyKitty_OptionalPlay()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            Card lionToPlay = FindCardsWhere((Card c) => IsLion(c) && tamer.TurnTaker.Deck.HasCard(c)).First();
            Card handCard = PutInHand("GildeasTheGood");
            DecisionSelectCards = new Card[] { lionToPlay, null };
            PlayCard("HereKittyKitty");
            AssertInPlayArea(tamer, lionToPlay);
            AssertInHand(handCard);


        }

        [Test()]
        public void TestHereKittyKitty_NoLionsInDeck()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            IEnumerable<Card> lionsToTrash = FindCardsWhere((Card c) => IsLion(c) && tamer.TurnTaker.Deck.HasCard(c));
            PutInTrash(lionsToTrash);
            Card handCard = PutInHand("GildeasTheGood");
            DecisionSelectCard = handCard;
            PlayCard("HereKittyKitty");
            AssertInPlayArea(tamer, handCard);
            

        }

        [Test()]
        public void TestKittyGloves()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            //Whenever a Lion enters play, reduce damage dealt to that Lion by 1 until the start of your next turn.
            PlayCard("KittyGloves");
            Card lion = PlayCard("MotherOfThePack");
            QuickHPStorage(lion);
            DealDamage(baron, lion, 2, DamageType.Cold);
            QuickHPCheck(-1);

            //check that it expires at start of next turn
            GoToStartOfTurn(tamer);
            QuickHPStorage(lion);
            DealDamage(baron, lion, 2, DamageType.Cold);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestLingorthTheLighthearted_1DamageDealt()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);
            // When this card is dealt exactly 1 damage, it deals one non-hero target 4 melee damage.
           //When this card is dealt more than 1 damage, it deals {TheTamer} 2 melee damage
            Card lion = PlayCard("LingorthTheLighthearted");

            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(lion, tamer.CharacterCard, baron.CharacterCard);
            DealDamage(baron, lion, 1, DamageType.Cold);
            QuickHPCheck(-1, 0, -4);
        }

        [Test()]
        public void TestLingorthTheLighthearted_0DamageDealt()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);
            // When this card is dealt exactly 1 damage, it deals one non-hero target 4 melee damage.
            //When this card is dealt more than 1 damage, it deals {TheTamer} 2 melee damage
            Card lion = PlayCard("LingorthTheLighthearted");
            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(lion, tamer.CharacterCard, baron.CharacterCard);
            DealDamage(baron, lion, 0, DamageType.Cold);
            QuickHPCheck(0, 0, 0);
        }

        [Test()]
        public void TestLingorthTheLighthearted_2DamageDealt()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);
            // When this card is dealt exactly 1 damage, it deals one non-hero target 4 melee damage.
            //When this card is dealt more than 1 damage, it deals {TheTamer} 2 melee damage
            Card lion = PlayCard("LingorthTheLighthearted");
            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(lion, tamer.CharacterCard, baron.CharacterCard);
            DealDamage(baron, lion, 2, DamageType.Cold);
            QuickHPCheck(-2, -2, 0);
        }

        [Test()]
        public void TestMotherOfThePack()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            //Redirect all Lion damage dealt to {TheTamer} to this card.
            Card mother = PlayCard("MotherOfThePack");
            Card lion = PlayCard("LingorthTheLighthearted");
            QuickHPStorage(mother, tamer.CharacterCard);
            DealDamage(lion, tamer, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);

            //only from lions
            QuickHPUpdate();
            DealDamage(baron, tamer, 2, DamageType.Melee);
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestNemeanCoat()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            //Until the start of your next turn, {TheTamer} is immune to melee damage.
            Card coat = PlayCard("NemeanCoat");
            GoToUsePowerPhase(tamer);
            UsePower(coat);

            QuickHPStorage(tamer);
            DealDamage(baron, tamer, 5, DamageType.Melee);
            QuickHPCheckZero();

            //only melee
            QuickHPUpdate();
            DealDamage(baron, tamer, 5, DamageType.Toxic);
            QuickHPCheck(-5);

            //only tamer
            QuickHPStorage(haka);
            DealDamage(baron, haka, 5, DamageType.Melee);
            QuickHPCheck(-5);

            //expires at start of next turn
            GoToStartOfTurn(tamer);
            QuickHPStorage(tamer);
            DealDamage(baron, tamer, 5, DamageType.Melee);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestPhileonTheDestroyer()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);
            //When this card is dealt damage, it deals { TheTamer} 1 melee damage and another target 2 melee damage.

            Card lion = PlayCard("PhileonTheDestroyer");
            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(lion, tamer.CharacterCard, baron.CharacterCard);
            DealDamage(baron, lion, 2, DamageType.Cold);
            QuickHPCheck(-2, -1, -2);
        }

        [Test()]
        public void TestPhileonTheDestroyer_0Damage()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);
            //When this card is dealt damage, it deals { TheTamer} 1 melee damage and another target 2 melee damage.

            Card lion = PlayCard("PhileonTheDestroyer");
            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(lion, tamer.CharacterCard, baron.CharacterCard);
            DealDamage(baron, lion, 0, DamageType.Cold);
            QuickHPCheck(0, 0, 0);
        }

        [Test()]
        public void TestRingOfFire()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            //Until the start of your next turn, increase damage dealt by Lions to non-hero targets by 1.

            Card target = PlayCard("BladeBattalion");
            //used to collect throw away damage
            SelectCardsForNextDecision(target);

            Card lion = PlayCard("LingorthTheLighthearted");
            PlayCard("RingOfFire");
            QuickHPStorage(lion, tamer.CharacterCard, baron.CharacterCard);
            DealDamage(lion, baron.CharacterCard, 1, DamageType.Melee);
            DealDamage(lion, tamer.CharacterCard, 1, DamageType.Melee);
            DealDamage(lion, lion, 1, DamageType.Melee);
            QuickHPCheck(-1, -1, -2);

            //check only lions
            QuickHPStorage(baron);
            DealDamage(tamer, baron, 2, DamageType.Projectile);
            QuickHPCheck(-2);

            //expires at the start of the turn
            GoToStartOfTurn(tamer);
            QuickHPUpdate();
            DealDamage(lion, baron, 2, DamageType.Projectile);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestSendInTheClowns_Draw()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            Card clowns = PutInHand("SendInTheClowns");
            //Draw 2 cards. You may play a Lion.
            QuickHandStorage(tamer);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            PlayCard(clowns);
            //-1 from playing card, +2 from draw
            QuickHandCheck(1);
        }

        [Test()]
        public void TestSendInTheClowns_Play()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            Card clowns = PutInHand("SendInTheClowns");
            Card lion = PutInHand("MotherOfThePack");
            //Draw 2 cards. You may play a Lion.
            DecisionSelectCard = lion;
            PlayCard(clowns);
            AssertInPlayArea(tamer, lion);
        }

        [Test()]
        public void TestTapOut_Destroy3()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            SetHitPoints(tamer, 10);

            IEnumerable<Card> lions = FindCardsWhere((Card c) => IsLion(c) && tamer.TurnTaker.Deck.HasCard(c)).Take(3);
            PlayCards(lions);

            //Destroy all Lions in play. {TheTamer} regains 1 HP for each Lion destroyed this way.
            QuickHPStorage(tamer);
            PlayCard("TapOut");
            QuickHPCheck(3);
            AssertInTrash(lions);
        }

        [Test()]
        public void TestTapOut_Destroy0()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            SetHitPoints(tamer, 10);

            //Destroy all Lions in play. {TheTamer} regains 1 HP for each Lion destroyed this way.
            QuickHPStorage(tamer);
            PlayCard("TapOut");
            QuickHPCheck(0);
        }

        [Test()]
        public void TestThreeRings()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            Card threeRings = PlayCard("ThreeRings");
            Card lion1 = PlayCard("MotherOfThePack");
            Card lion2 = PutInHand("LingorthTheLighthearted");

            DecisionYesNo = true;
            DecisionSelectCard = lion2;
            DestroyCard(lion1, baron.CharacterCard);
            AssertInPlayArea(tamer, lion2);
            AssertInHand(lion1);
            AssertInTrash(threeRings);
        }

        [Test()]
        public void TestThreeRings_MoveOptional()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            Card threeRings = PlayCard("ThreeRings");
            Card lion1 = PlayCard("MotherOfThePack");
            Card lion2 = PutInHand("LingorthTheLighthearted");

            DecisionYesNo = false;
            DecisionSelectCard = lion2;
            DestroyCard(lion1, baron.CharacterCard);
            AssertInPlayArea(tamer, lion2);
            AssertInTrash(lion1);
            AssertInTrash(threeRings);
        }
        [Test()]
        public void TestThreeRings_PlayOptional()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(tamer);

            Card threeRings = PlayCard("ThreeRings");
            Card lion1 = PlayCard("MotherOfThePack");
            Card lion2 = PutInHand("LingorthTheLighthearted");

            DecisionYesNo = true;
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            DestroyCard(lion1, baron.CharacterCard);
            AssertInHand(lion2);
            AssertInHand(lion1);
            AssertInTrash(threeRings);
        }

        [Test()]
        public void TestWhippingWhiskers()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            GoToPlayCardPhase(tamer);

            Card lion1 = PlayCard("LingorthTheLighthearted");
            Card lion2 = PlayCard("MotherOfThePack");
            //One Lion deals one other Lion 2 melee damage. A Lion dealt damage this way deals all non-hero targets 1 energy damage.
            QuickHPStorage(baron.CharacterCard, battalion, tamer.CharacterCard, haka.CharacterCard, ra.CharacterCard, lion1, lion2);
            PlayCard("WhippingWhiskers");
            QuickHPCheck(-1, -1, 0, 0, 0, 0, -2);


        }

        [Test()]
        public void TestWhippingWhiskers_NoDamageDealt()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer", "Haka", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            GoToPlayCardPhase(tamer);

            Card lion1 = PlayCard("LingorthTheLighthearted");
            Card lion2 = PlayCard("MotherOfThePack");
            //One Lion deals one other Lion 2 melee damage. A Lion dealt damage this way deals all non-hero targets 1 energy damage.
            QuickHPStorage(baron.CharacterCard, battalion, tamer.CharacterCard, haka.CharacterCard, ra.CharacterCard, lion1, lion2);
            AddImmuneToDamageTrigger(tamer, true, false, false);
            PlayCard("WhippingWhiskers");
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0);


        }

        [Test()]
        public void TestGrandMasterTamerLoads()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer/GrandMasterTamerCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(tamer);
            Assert.IsInstanceOf(typeof(GrandMasterTamerCharacterCardController), tamer.CharacterCardController);

            Assert.AreEqual(25, tamer.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestGrandMasterTamerPower_LionsInPlay_DamageDealt()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer/GrandMasterTamerCharacter", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            Card mother = PlayCard("MotherOfThePack");
            GoToUsePowerPhase(tamer);
            // Deal 1 Lion 1 sonic damage. If no damage is dealt this way, draw a card.
            QuickHPStorage(mother);
            QuickHandStorage(tamer);
            UsePower(tamer);
            QuickHPCheck(-1);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestGrandMasterTamerPower_LionsInPlay_DamageDealtNotDealt()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer/GrandMasterTamerCharacter", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            Card mother = PlayCard("MotherOfThePack");
            GoToUsePowerPhase(tamer);
            // Deal 1 Lion 1 sonic damage. If no damage is dealt this way, draw a card.
            QuickHPStorage(mother);
            AddImmuneToDamageTrigger(tamer, true, false, false);
            QuickHandStorage(tamer);
            UsePower(tamer);
            QuickHPCheck(0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestGrandMasterTamerPower_LionsNotInPlay()
        {
            SetupGameController("BaronBlade", "Studio29.TheTamer/GrandMasterTamerCharacter", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tamer);
            GoToUsePowerPhase(tamer);
            // Deal 1 Lion 1 sonic damage. If no damage is dealt this way, draw a card.
            QuickHandStorage(tamer);
            UsePower(tamer);
            QuickHandCheck(1);
        }

    }
}
