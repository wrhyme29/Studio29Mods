using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class WipeDownCardController : CardController
    {

        public WipeDownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => c.IsDish(), "dish"));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, destroy this card.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => DestroyThisCardResponse(pca), TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, destroy all dish cards. This card deals each target X + 1 energy damage, where X is the number of dishes destroyed this way.
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsDish(), "dish"), autoDecide: true, storedResults: storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            int X = GetNumberOfCardsDestroyed(storedResults);
            coroutine = DealDamage(Card, c => c.IsTarget, X + 1, DamageType.Energy);
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