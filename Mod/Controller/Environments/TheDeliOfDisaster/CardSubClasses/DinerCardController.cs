using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class DinerCardController : CardController
    {

        public DinerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsDish()).Any(), () => "There is a dish card in play.", () => "There is not a dish card in play.");
        }

        public override IEnumerator Play()
        {
            //When this card enters play, destroy 1 dish card.

            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsDish(), "dish"), false, storedResultsAction: storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //If there are no dish cards are destroyed, this card deals each target 1 fire damage.
            if(DidDestroyCard(storedResults))
            {
                yield break;
            }

            coroutine = DealDamage(Card, c => c.IsTarget, 1, DamageType.Fire);
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