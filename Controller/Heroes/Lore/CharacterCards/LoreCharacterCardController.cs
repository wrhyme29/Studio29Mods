using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
	public class LoreCharacterCardController : HeroCharacterCardController
	{
		public LoreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Reveal cards from the top of {Lore}'s deck until a Story card is revealed. Put that card into play. Shuffle the other revealed cards into {Lore}'s deck."

			IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(HeroTurnTakerController, TurnTaker.Deck, playMatchingCards: true, putMatchingCardsIntoPlay: true, moveMatchingCardsToHand: false, cardCriteria: new LinqCardCriteria(c => IsStory(c), "story"), numberOfMatches: 1, shuffleSourceAfterwards: true, showMessage: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}

		
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						
						break;
					}
				case 1:
					{
						
						break;
					}
				case 2:
					{
						
						break;
					}
			}
			yield break;
		}

		public static readonly string StoryKeyword = "story";
	

		protected bool IsStory(Card card)
		{
			return card.DoKeywordsContain(StoryKeyword);
		}

		
	}
}
