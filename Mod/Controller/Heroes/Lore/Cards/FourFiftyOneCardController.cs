using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class FourFiftyOneCardController : StoryCardController
    {

        public FourFiftyOneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "epic")
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Destroy any number of Story cards. Draw X cards, where X is the number of cards destroyed this way plus 1.
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsStory(), "story"), null,  requiredDecisions: 0, storedResultsAction: storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            int amount = storedResults.Where((DestroyCardAction d) => d.CardToDestroy != null && d.WasCardDestroyed).Count() + 1;
            coroutine = DrawCards(HeroTurnTakerController, amount);
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