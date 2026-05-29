using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.Lore
{
    public class DeweyTheDecimalCardController : LoreCardController
    {

        public DeweyTheDecimalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Search your deck for a Story and put it into your hand. Shuffle your deck. 
            IEnumerator coroutine = SearchForCards(HeroTurnTakerController, searchDeck: true, searchTrash: false, 1, 1, new LinqCardCriteria((Card c) => IsStory(c), "story"), putIntoPlay: false, putInHand: true, putOnDeck: false, shuffleAfterwards: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may draw a card.
            coroutine = DrawCard(optional: true);
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