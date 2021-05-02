using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.TheTamer
{
	public class RoadShowTamerCharacterCardController : TheTamerSubCharacterCardController
	{
		public RoadShowTamerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Reveal cards from the top of {TheTamer}'s deck until a Lion card is revealed. Put that card into play. Shuffle the other revealed cards into {TheTamer}'s deck."

			IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(HeroTurnTakerController, TurnTaker.Deck, playMatchingCards: true, putMatchingCardsIntoPlay: true, moveMatchingCardsToHand: false, cardCriteria: new LinqCardCriteria(c => IsLion(c), "lion"), numberOfMatches: 1, shuffleSourceAfterwards: true, showMessage: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						
						yield break;
					}
				case 1:
					{
						
						yield break;
					}
				case 2:
					{
						
						yield break;
					}
			}
			yield break;
		}

		
	}
}
