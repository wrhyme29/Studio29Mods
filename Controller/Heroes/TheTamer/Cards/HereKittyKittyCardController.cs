using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Studio29.TheTamer
{
    public class HereKittyKittyCardController : TheTamerCardController
    {

        public HereKittyKittyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => IsLion(c), "lion"));
        }

        public override IEnumerator Play()
        {
			//Search your deck for one Lion and put it into your hand.

			IEnumerator coroutine;
			if (FindCardsWhere((Card c) => IsLion(c) && TurnTaker.Deck.HasCard(c)).Any())
			{
				coroutine = SearchForCards(DecisionMaker, searchDeck: true, searchTrash: false, 1, 1, new LinqCardCriteria((Card c) => IsLion(c), "lion"), putIntoPlay: false, putInHand: true, putOnDeck: false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			else
			{
				coroutine = GameController.SendMessageAction("There are no Lions in the deck.", Priority.Low, GetCardSource(), showCardSource: true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}

			// You may play a card...

			coroutine = GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardSource: GetCardSource());
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