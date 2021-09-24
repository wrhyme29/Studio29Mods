using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class MindlessMishigaCardController : TheDeliOfDisasterCardController
    {

        public MindlessMishigaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP();
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override IEnumerator Play()
        {
            //The villain and hero target with the lowest HP regains 1 HP.
            IEnumerator coroutine = LowestHeroAndVillainGainHP();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //One player draws a card.
            coroutine = GameController.SelectHeroToDrawCards(DecisionMaker, 1, optionalDrawCards: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle the Villain trash and reveal cards from the top until a Target is revealed. Put it into play. Put the other revealed cards back into the Villain Trash.
            coroutine = BringTargetBackResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //The villain target with the lowest HP deals the villain target with the highest HP 3 fire damage.
            coroutine = LowestVillainDealsHighestVillainDamage();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

        }

        private IEnumerator LowestVillainDealsHighestVillainDamage()
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => IsVillainTarget(c), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card card = storedResults.FirstOrDefault();
            if (card is null)
            {
                yield break;
            }

            IEnumerator coroutine2 = DealDamageToHighestHP(card, 1, (Card c) => IsVillainTarget(c), (Card c) => 3, DamageType.Fire);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }

        }

        private IEnumerator LowestHeroAndVillainGainHP()
        {
            List<Card> lowestHeroAndVillain = new List<Card>();
            List<Card> storedLowestHero = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsHero, storedLowestHero, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card card = storedLowestHero.FirstOrDefault();
            if (card != null)
            {
                lowestHeroAndVillain.Add(card);
            }

            List<Card> storedLowestVillain = new List<Card>();
            coroutine = GameController.FindTargetWithLowestHitPoints(1, (Card c) => IsVillainTarget(c), storedLowestVillain, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            card = storedLowestVillain.FirstOrDefault();
            if (card != null)
            {
                lowestHeroAndVillain.Add(card);
            }

            coroutine = GameController.GainHP(DecisionMaker, c => lowestHeroAndVillain.Contains(c), 1, numberOfCardsToHeal: 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator BringTargetBackResponse()
        {
            IEnumerable<Card> source = FindCardsWhere((Card c) => c.IsVillainTarget && c.Location.IsVillain && c.Location.IsTrash);
            if (source.Count() == 1)
            {
                string message = $"{Card.Title} puts {source.First().Title} from the villain trash into play.";
                IEnumerator coroutine = GameController.SendMessageAction(message, Priority.Low, GetCardSource(), null, showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            IEnumerator coroutine2 = FindVillainDeck(DecisionMaker, SelectionType.PutIntoPlay, storedResults, (Location l) => FindTrashFromDeck(l) != null && FindTrashFromDeck(l).Cards.Any((Card c) => c.IsTarget));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            Location selectedLocation = GetSelectedLocation(storedResults);
            if (selectedLocation != null)
            {
                Location trash = FindTrashFromDeck(selectedLocation);
                coroutine2 = ReviveCardFromTurnTakerTrash(FindTurnTakerController(trash.OwnerTurnTaker), c => IsVillainTarget(c));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            else
            {
                coroutine2 = base.GameController.SendMessageAction("There are no villain targets in any villain trash to put into play.", Priority.Low, GetCardSource(), null, showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
        }

        private IEnumerator ReviveCardFromTurnTakerTrash(TurnTakerController revealingTurnTaker, Func<Card, bool> revealUntil)
        {
            IEnumerator coroutine = GameController.ShuffleLocation(revealingTurnTaker.TurnTaker.Trash, null, GetCardSource());
            List<RevealCardsAction> revealActions = new List<RevealCardsAction>();
            IEnumerator revealE = GameController.RevealCards(revealingTurnTaker, revealingTurnTaker.TurnTaker.Trash, revealUntil, 1, revealActions, RevealedCardDisplay.None, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
                yield return GameController.StartCoroutine(revealE);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
                GameController.ExhaustCoroutine(revealE);
            }
            RevealCardsAction action = revealActions.FirstOrDefault();
            if (action != null)
            {
                if (action.FoundMatchingCards)
                {
                    IEnumerator coroutine2 = GameController.PlayCard(TurnTakerController, action.MatchingCards.First(), isPutIntoPlay: true, null, optional: false, null, null, evenIfAlreadyInPlay: false, null, null, null, associateCardSource: false, fromBottom: false, canBeCancelled: true, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine2);
                    }
                }
                else
                {
                    Log.Debug("There are no appropriate cards in the trash to bring into play.");
                }
                if (action.NonMatchingCards.Count() > 0)
                {
                    IEnumerator coroutine3 = GameController.BulkMoveCards(TurnTakerController, action.NonMatchingCards, revealingTurnTaker.TurnTaker.Trash, toBottom: false, performBeforeDestroyActions: true, null, isDiscard: false, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine3);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine3);
                    }
                }
            }
            List<Location> list = new List<Location>();
            list.Add(TurnTaker.Revealed);
            List<Card> cardsInList = revealActions.SelectMany((RevealCardsAction ra) => ra.RevealedCards).ToList();
            IEnumerator coroutine4 = CleanupCardsAtLocations(list, revealingTurnTaker.TurnTaker.Trash, toBottom: false, addInhibitorException: true, shuffleAfterwards: false, sendMessage: false, isDiscard: false, isReturnedToOriginalLocation: true, cardsInList);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine4);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine4);
            }

        }
    }
}