using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class DinerCardController : TheDeliOfDisasterCardController
    {

        public DinerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, destroy 1 dish card.

            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => IsDish(c), "dish"), false, storedResultsAction: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If there are no dish cards are destroyed, this card deals each target 1 fire damage.
            if(DidDestroyCard(storedResults))
            {
                yield break;
            }

            coroutine = DealDamage(Card, c => c.IsTarget, 1, DamageType.Fire);
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