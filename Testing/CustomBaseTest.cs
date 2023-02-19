﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Studio29Tests
{
    public class CustomBaseTest : BaseTest
    {
        //heroes
        protected HeroTurnTakerController tamer { get { return FindHero("TheTamer"); } }
        protected HeroTurnTakerController lore { get { return FindHero("Lore"); } }
        protected HeroTurnTakerController birthdayBoy { get { return FindHero("BirthdayBoy"); } }


        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotDealDamageTrigger(TurnTakerController ttc, Card specificCard)
        {
            CannotDealDamageStatusEffect cannotDealDamageEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageEffect.SourceCriteria.IsSpecificCard = specificCard;
            cannotDealDamageEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotDealNextDamageTrigger(TurnTakerController ttc, Card card)
        {
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.NumberOfUses = 1;
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = card;
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotPlayCardsStatusEffect(TurnTakerController ttc, bool heroesCannotPlay, bool villainsCannotPlay)
        {
            CannotPlayCardsStatusEffect effect = new CannotPlayCardsStatusEffect();
            effect.CardCriteria.IsHero = new bool?(heroesCannotPlay);
            effect.CardCriteria.IsVillain = new bool?(villainsCannotPlay);
            effect.CardCriteria.IsEnvironment = false;
            effect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(effect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddImmuneToNextDamageEffect(TurnTakerController ttc, bool villains, bool heroes)
        {
            ImmuneToDamageStatusEffect effect = new ImmuneToDamageStatusEffect();
            effect.TargetCriteria.IsVillain = villains;
            effect.TargetCriteria.IsHero = heroes;
            effect.NumberOfUses = 1;
            RunCoroutine(GameController.AddStatusEffect(effect, true, ttc.CharacterCardController.GetCardSource()));
        }

        protected void AddReduceDamageTrigger(TurnTakerController ttc, bool heroesReduce, bool villainsReduce, int amount)
        {
            ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(amount);
            effect.TargetCriteria.IsHero = new bool?(heroesReduce);
            effect.TargetCriteria.IsVillain = new bool?(villainsReduce);
            effect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(effect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddReduceDamageOfDamageTypeTrigger(HeroTurnTakerController httc, DamageType damageType, int amount)
        {
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);
            reduceDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
            reduceDamageStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(reduceDamageStatusEffect, true, new CardSource(httc.CharacterCardController)));
        }

        protected void AddIncreaseDamageOfDamageTypeTrigger(HeroTurnTakerController httc, DamageType damageType, int amount)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(amount);
            increaseDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
            increaseDamageStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(increaseDamageStatusEffect, true, new CardSource(httc.CharacterCardController)));
        }

        protected void AddMakeCardIndestructibleToTheNextDestructionTrigger(Card cardToMakeIndestructible, CardSource cardSource = null)
        {
            cardSource = cardSource ?? FindCardController(cardToMakeIndestructible).GetCardSource();

            MakeIndestructibleStatusEffect makeIndestructibleStatusEffect = new MakeIndestructibleStatusEffect();
            makeIndestructibleStatusEffect.CardsToMakeIndestructible.IsSpecificCard = cardToMakeIndestructible;
            makeIndestructibleStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(makeIndestructibleStatusEffect, true, cardSource));

        }

        protected void PreventEndOfTurnEffects(TurnTakerController ttc, Card cardToPrevent)
        {
            PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect();
            preventPhaseEffectStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            preventPhaseEffectStatusEffect.CardCriteria.IsSpecificCard = cardToPrevent;
            RunCoroutine(base.GameController.AddStatusEffect(preventPhaseEffectStatusEffect, showMessage: true, ttc.CharacterCardController.GetCardSource()));
        }

        protected void PreventStartOfTurnEffects(TurnTakerController ttc, Card cardToPrevent)
        {
            PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect(Phase.Start);
            preventPhaseEffectStatusEffect.UntilEndOfNextTurn(ttc.TurnTaker);
            preventPhaseEffectStatusEffect.CardCriteria.IsSpecificCard = cardToPrevent;
            RunCoroutine(base.GameController.AddStatusEffect(preventPhaseEffectStatusEffect, showMessage: true, ttc.CharacterCardController.GetCardSource()));
        }

        protected void AssertCardConfiguration(string identifier, string[] keywords = null, int hitpoints = 0)
        {
            Card card = GetCard(identifier);
            if (keywords != null)
            {
                foreach (string keyword in keywords)
                {
                    AssertCardHasKeyword(card, keyword, false);
                }
            }
            if (hitpoints > 0)
            {
                AssertMaximumHitPoints(card, hitpoints);
            }
        }

        protected void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertCardHasKeyword(GetCard(id), keyword, false);
            }
        }

        protected void AssertHasAbility(string abilityKey, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                int number = card.GetNumberOfActivatableAbilities(abilityKey);
                Assert.GreaterOrEqual(number, 1);
            }
        }


        protected void AssertDamageTypeChanged(HeroTurnTakerController httc, Card source, Card target, int amount, DamageType initialDamageType, DamageType expectedDamageType)
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            this.RunCoroutine(this.GameController.DealDamage(httc, source, (Card c) => c == target, amount, initialDamageType, false, false, storedResults, null, null, false, null, null, false, false, new CardSource(GetCardController(source))));

            if (storedResults != null)
            {
                DealDamageAction dd = storedResults.FirstOrDefault<DealDamageAction>();
                DamageType actualDamageType = dd.DamageType;
                Assert.AreEqual(expectedDamageType, actualDamageType, $"Expected damage type: {expectedDamageType}. Actual damage type: {actualDamageType}");
            }
            else
            {
                Assert.Fail("storedResults was null");
            }

        }

        protected void AddDestroyEnvironmentCardCounterAttackTrigger(HeroTurnTakerController decisionMaker, Card target, CardSource cardsource)
        {
            Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.Target == target;
            Func<DealDamageAction, IEnumerator> response = (DealDamageAction dd) => this.GameController.DestroyCards(decisionMaker, new LinqCardCriteria(c => c.IsEnvironment, "environment"), autoDecide: true, cardSource: cardsource);
            GameController.AddTrigger<DealDamageAction>(new Trigger<DealDamageAction>(this.GameController, criteria, response, new TriggerType[] { TriggerType.DestroyCard }, TriggerTiming.After, cardsource));
        }

        protected GameController ReplayGameFromPath(string path)
        {
            try
            {
                var savedGame = LoadGamePath(path);

                if (savedGame != null)
                {
                    var newGame = MakeReplayableGame(savedGame);
                    SetupGameController(newGame);

                    Console.WriteLine("Successfully created game to replay...");

                    StartGame();
                    this.ReplayingGame = true;

                    // Keep moving the game forward until we have reached the stopping point.
                    int sanity = 1000;
                    while (this.ReplayingGame)
                    {
                        RunActiveTurnPhase();
                        EnterNextTurnPhase();
                        sanity--;

                        if (sanity == 0)
                        {
                            Log.Error("Save game never seemed to end: " + path);
                            this.ReplayingGame = false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed to load and replay game.");
                }
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to load and replay game. Reason: " + e.Message);
                throw;
            }

            return this.GameController;
        }

    }
}
