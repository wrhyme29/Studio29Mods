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
			SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.IsLion(), "lion"));
        }

        public override IEnumerator Play()
        {
			//Search your deck for one Lion and put it into your hand.

			IEnumerator coroutine;
			if (FindCardsWhere((Card c) => c.IsLion() && TurnTaker.Deck.HasCard(c)).Any())
			{
				coroutine = SearchForCards(DecisionMaker, searchDeck: true, searchTrash: false, 1, 1, new LinqCardCriteria((Card c) => c.IsLion(), "lion"), putIntoPlay: false, putInHand: true, putOnDeck: false);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
			}
			else
			{
				coroutine = GameController.SendMessageAction("There are no Lions in the deck.", Priority.Low, GetCardSource(), showCardSource: true);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
			}

			// You may play a card...

			coroutine = GameController.SelectAndPlayCardFromHand(HeroTurnTakerController, true, cardSource: GetCardSource());
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