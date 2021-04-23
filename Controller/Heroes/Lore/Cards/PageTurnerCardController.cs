using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Lore
{
    public class PageTurnerCardController : StoryCardController
    {

        public PageTurnerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, MysteryKeyword)
        {

        }

        public override void AddTriggers()
        {
            //At the end of your turn, reveal the top cards of 2 decks. You may replace or discard each card.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, RevealCardsResponse, TriggerType.RevealCard);
        }

        private IEnumerator RevealCardsResponse(PhaseChangeAction action)
        {
            //...reveal the top card of 2 different decks, then replace or discard each card.
            List<SelectLocationDecision> storedResult = new List<SelectLocationDecision>();
            //Pick first deck
            IEnumerator coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location deck) => true, storedResult, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectLocation(storedResult))
            {
                yield break;
            }

            Location selectedDeck = GetSelectedLocation(storedResult);
            coroutine = RevealCard_DiscardItOrPutItOnDeck(DecisionMaker, FindTurnTakerController(selectedDeck.OwnerTurnTaker), selectedDeck, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Second Deck
            List<SelectLocationDecision> storedResult2 = new List<SelectLocationDecision>();
            coroutine = GameController.SelectADeck(HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location deck) => deck != selectedDeck, storedResult2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (!DidSelectLocation(storedResult2))
            {
                yield break;
            }
                
            selectedDeck = GetSelectedLocation(storedResult2);
            coroutine = RevealCard_DiscardItOrPutItOnDeck(DecisionMaker, FindTurnTakerController(selectedDeck.OwnerTurnTaker), selectedDeck, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
        }
    }
}