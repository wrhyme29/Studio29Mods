using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class CallbackCardController : StoryCardController
	{

		public CallbackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "mystery")
		{

		}

		public override IEnumerator UsePower(int index = 0)
		{
			//One player other than Lore may take a card from their trash and put on top of their deck.
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.MoveCardOnDeck, new LinqCardCriteria((Card c) => c.IsInTrash && c.Location.IsHero && c.Owner != TurnTaker, "cards in other hero's trash", useCardsSuffix: false), storedResults, optional: false, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
			if (DidSelectCard(storedResults))
			{
				Card selectedCard = GetSelectedCard(storedResults);
				Location deck = selectedCard.Owner.Deck;
				coroutine = GameController.MoveCard(TurnTakerController, selectedCard, deck, decisionSources: storedResults.Cast<IDecision>(), cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
			}

			//One player may draw a card.
			coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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