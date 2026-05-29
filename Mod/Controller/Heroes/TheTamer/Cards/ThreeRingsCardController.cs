using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class ThreeRingsCardController : TheTamerCardController
    {

        public ThreeRingsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SetCardProperty("TriggerPlayAndDestroy", false);
        }

        public override void AddTriggers()
        {
            //When a Lion is destroyed you may return it to your hand. You may play a Lion. Destroy this card.
            AddTrigger((DestroyCardAction destroy) => destroy.CardToDestroy != null && destroy.CardToDestroy.CanBeDestroyed && destroy.WasCardDestroyed && destroy.CardToDestroy.Card.IsLion() && destroy.PostDestroyDestinationCanBeChanged , ReturnToHandResponse, new TriggerType[]
            {
            TriggerType.MoveCard,
            TriggerType.ChangePostDestroyDestination
            }, TriggerTiming.After);

            AddTrigger<GameAction>((GameAction action) => IsPropertyTrue("TriggerPlayAndDestroy"), PlayLionAnDestroyResponse, new TriggerType[]
            {
                TriggerType.PlayCard,
                TriggerType.DestroySelf
            }, TriggerTiming.After);
        }

        private IEnumerator PlayLionAnDestroyResponse(GameAction arg)
        {
            //You may play a Lion. Destroy this card.
            SetCardProperty("TriggerPlayAndDestroy", false);
            IEnumerator coroutine = GameController.SelectAndPlayCardFromHand(HeroTurnTakerController, true, cardCriteria: new LinqCardCriteria((Card c) => c.IsLion(), "lion"), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DestroyThisCardResponse(arg);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

        private IEnumerator ReturnToHandResponse(DestroyCardAction destroyCard)
        {
            //you may return it to your hand
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(HeroTurnTakerController, SelectionType.MoveCardToHand, destroyCard.CardToDestroy.Card,storedResults: storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (DidPlayerAnswerYes(storedResults))
            {
                destroyCard.SetPostDestroyDestination(HeroTurnTaker.Hand, decisionSources: storedResults.CastEnumerable<YesNoCardDecision, IDecision>(), cardSource: GetCardSource());
            }

            SetCardPropertyToTrueIfRealAction("TriggerPlayAndDestroy");

            yield break;

        }
    }
}