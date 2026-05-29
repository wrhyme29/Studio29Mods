using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class StuckleAndNosherCardController : DinerCardController
    {

        public StuckleAndNosherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, each character card gains 2 hp.
            //Each player may discard 1 card. If fewer than {H - 1} cards are discarded this way, destroy this card.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, new TriggerType[] { TriggerType.GainHP, TriggerType.DiscardCard, TriggerType.DestroySelf });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //each character card gains 2 hp.
            IEnumerator coroutine = base.GameController.GainHP(DecisionMaker, (Card c) => c.IsCharacter, 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Each player may discard 1 card. If fewer than {H - 1} cards are discarded this way, destroy this card.
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            coroutine = GameController.EachPlayerDiscardsCards(0, 1, storedResultsDiscard: storedResults,  showCounter: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int numberOfCardsDiscarded = GetNumberOfCardsDiscarded(storedResults);
            if(numberOfCardsDiscarded >= Game.H - 1)
            {
                yield break;
            }
            coroutine = DestroyThisCardResponse(pca);
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