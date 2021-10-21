using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class ClemanataTheEldestCardController : LionCardController
    {

        public ClemanataTheEldestCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.RevealCard)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
			//reveal the top 2 cards of the Villain deck. Put 1 of them on top of the villain deck and the other on the bottom.
			List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
			IEnumerator coroutine = FindVillainDeck(DecisionMaker, SelectionType.RevealCardsFromDeck, storedResults, (Location l) => true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Location deck = GetSelectedLocation(storedResults);
			List<Card> storedCards = new List<Card>();
			if (deck != null)
			{
				coroutine = RevealCardsFromTopOfDeck_PutOnTopAndOnBottom(HeroTurnTakerController, TurnTakerController, deck, storedResults: storedCards);
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
}