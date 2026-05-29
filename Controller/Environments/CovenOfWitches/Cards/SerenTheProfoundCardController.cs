using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.CovenOfWitches
{
    public class SerenTheProfoundCardController : WitchCardController
    {

        public SerenTheProfoundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "CurseOfSeren")
        {

        }

        public override void AddTriggers()
        {
            // At the start of the Environment turn, 1 Player may discard 2 cards. If no cards were discarded this way, restore this card to full HP.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnResponse, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.GainHP
            });

            // At the end of the Environment turn, select a target. That target cannot deal damage or be damaged until the end of the next Environment turn.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.ImmuneToDamage,
                TriggerType.CreateStatusEffect
            });
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction arg)
        {
            // 1 Player may discard 2 cards. If no cards were discarded this way, restore this card to full HP.
            IEnumerator coroutine = OnePlayerMayDiscardTwoCardsToPreventRestoreHPResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction arg)
        {
            //select a target.

            List<SelectCardDecision> selectCardResults = new List<SelectCardDecision>();

            IEnumerator selectCardRoutine = base.GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.SelectTargetNoDamage, cardCriteria:
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlayAndHasGameText, "targets in play", false), selectCardResults, optional: false, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectCardRoutine);
            }

            if (!DidSelectCard(selectCardResults))
            {
                yield break;
            }

            Card selectedCard = GetSelectedCard(selectCardResults);

            // Apply cannot deal damage status effect to chosen card
            CannotDealDamageStatusEffect cannotDealDamage = new CannotDealDamageStatusEffect();
            cannotDealDamage.CardSource = Card;
            cannotDealDamage.SourceCriteria.IsSpecificCard = selectedCard;
            cannotDealDamage.UntilEndOfNextTurn(TurnTaker);
            cannotDealDamage.UntilTargetLeavesPlay(selectedCard);

            // Apply immune to damage status effect to chosen card
            ImmuneToDamageStatusEffect immuneToDamage = new ImmuneToDamageStatusEffect();
            immuneToDamage.CardSource = Card;
            immuneToDamage.TargetCriteria.IsSpecificCard = selectedCard;
            immuneToDamage.UntilEndOfNextTurn(TurnTaker);
            cannotDealDamage.UntilTargetLeavesPlay(selectedCard);

            IEnumerator cannotDealDamageRoutine = AddStatusEffect(cannotDealDamage);
            IEnumerator immuneToDamageRoutine = AddStatusEffect(immuneToDamage);

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(cannotDealDamageRoutine);
                yield return GameController.StartCoroutine(immuneToDamageRoutine);

            }
            else
            {
                GameController.ExhaustCoroutine(cannotDealDamageRoutine);
                GameController.ExhaustCoroutine(immuneToDamageRoutine);

            }

        }

        private IEnumerator OnePlayerMayDiscardTwoCardsToPreventRestoreHPResponse()
        {
            if (GameController.AllHeroes.Any((HeroTurnTaker hero) => hero.Hand.Cards.Count() >= 2))
            {
                Log.Debug("1 player may discard 2 cards to prevent " + Card.Title + " from restoring it's HP.");
                List<SelectTurnTakerDecision> selectHero = new List<SelectTurnTakerDecision>();
                IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.Custom, optional: true, allowAutoDecide: false, storedResults: selectHero, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && tt.ToHero().NumberOfCardsInHand >= 2, "heroes with 2 or more cards in hand"), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if (DidSelectTurnTaker(selectHero))
                {
                    
                
                TurnTaker selectedTurnTaker = GetSelectedTurnTaker(selectHero);
                    if (selectedTurnTaker.IsHero)
                    {
                        HeroTurnTakerController httc = FindHeroTurnTakerController(selectedTurnTaker.ToHero());
                        List<DiscardCardAction> storedDiscards = new List<DiscardCardAction>();
                        IEnumerator coroutine2 = GameController.SelectAndDiscardCards(httc, 2, optional: false, requiredDiscards: 2, storedResults: storedDiscards, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }

                        if (DidDiscardCards(storedDiscards, numberExpected: 2))
                        {

                            yield break;
                        }
                    }
                }
            }
            else
            {
                Log.Debug("No player has enough cards in hand to prevent " + Card.Title + " from gaining HP.");
                IEnumerator coroutine4 = GameController.SendMessageAction("No player has enough cards in hand to prevent " + Card.Title + " from gainin HP.", Priority.High, GetCardSource(), new Card[]
                {
                    Card
                });
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine4);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine4);
                }
            }

            IEnumerator coroutine3 = GameController.SetHP(Card, Card.MaximumHitPoints.Value, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine3);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine3);
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Select a player to discard 2 cards", "Select a player to discard 2 cards", "Vote for a player to discard 2 cards", "player to discard 2 cards");

        }
    }
}