using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.Lore
{
    public class CliffhangerCardController : LoreCardController
    {

        public CliffhangerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //You may destroy one Environment card
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), optional: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Play the top card of the environment deck
            coroutine = PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(null);
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