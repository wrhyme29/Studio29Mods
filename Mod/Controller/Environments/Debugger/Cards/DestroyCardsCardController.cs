using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.Debugger
{
    public class DestroyCardsCardController : OptionsCardController
    {

        public DestroyCardsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Select any number of cards in play. Destroy the selected cards.
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && c.ParentDeck.Identifier != TurnTaker.Identifier, "in play", useCardsSuffix: false, useCardsPrefix: true), null, optional: false, requiredDecisions: 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DestroyThisCardResponse(null);
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