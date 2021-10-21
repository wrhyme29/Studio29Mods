using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class DavilTheDashingCardController : LionCardController
    {

        public DavilTheDashingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DiscardCard)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //you may discard a card to play a card.
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>() ;
            IEnumerator coroutine = GameController.SelectAndDiscardCards(HeroTurnTakerController, 1, false, 0, storedResults: storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidDiscardCards(storedResults))
            {
                yield break;
            }

            coroutine = GameController.SelectAndPlayCardsFromHand(HeroTurnTakerController, 1, false, 0, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }


    }
}