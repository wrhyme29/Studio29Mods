using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.CovenOfWitches
{
    public class SybilTheClairvoyantCardController : WitchCardController
    {

        public SybilTheClairvoyantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "CurseOfSybil")
        {

        }

        public override void AddTriggers()
        {
            // At the start of the Environment turn, play the top card of the Villain deck.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, PlayTheTopCardOfTheVillainDeckWithMessageResponse, TriggerType.PlayCard);

            // At the end of the Environment turn, reveal the top 2 cards of the Villain deck. Put 1 on the top and 1 on the bottom of the Villain deck.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, TriggerType.RevealCard);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
			List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
			IEnumerator coroutine = FindVillainDeck(DecisionMaker, SelectionType.RevealCardsFromDeck, storedResults, (Location l) => true);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			if(!DidSelectLocation(storedResults))
            {
				yield break;
            }

			Location deck = GetSelectedLocation(storedResults);
			List<Card> revealedCards = new List<Card>();
			coroutine = RevealCardsFromTopOfDeck_PutOnTopAndOnBottom(DecisionMaker, TurnTakerController, deck, 2, 1, 1, revealedCards);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
			List<Location> list = new List<Location>();
			list.Add(deck.OwnerTurnTaker.Revealed);
			IEnumerator coroutine2 = CleanupCardsAtLocations(list, deck, cardsInList: revealedCards);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine2);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine2);
			}
			
		}
    }
}