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
						//One player discards a card. Each other player may draw a card.
						List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
						IEnumerator coroutine = GameController.SelectHeroToDiscardCard(DecisionMaker, storedResultsTurnTaker: storedResults, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}

						if(!DidSelectTurnTaker(storedResults))
                        {
							yield break;
                        }

						TurnTaker discardTurnTaker = GetSelectedTurnTaker(storedResults);
						coroutine = EachPlayerDrawsACard((HeroTurnTaker tt) => tt != discardTurnTaker.ToHero());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}

						break;
					}
				case 1:
					{
						//One player discards a card. Move two cards with a matching keyword from a trash into their owner's hand.
						List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
						IEnumerator coroutine = GameController.SelectHeroToDiscardCard(DecisionMaker, storedResultsDiscard: storedResults, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}

						if (!DidDiscardCards(storedResults))
						{
							yield break;
						}

						Card discardedCard = storedResults.First().CardToDiscard;
						IEnumerable<string> keywords = discardedCard.GetKeywords();
						coroutine = GameController.MoveCards(DecisionMaker, new LinqCardCriteria(c => c.IsInTrash && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && c.GetKeywords().Intersect(keywords).Any(), keywords.ToRecursiveString()), c => c.Owner.ToHero().Hand, numberOfCards: 2, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 2:
					{
						//One player discards a card. Reveal cards from the top of their deck until a card with a matching keyword is revealed. Put that card into play. Shuffle the other revealed cards back into the deck.
						List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
						IEnumerator coroutine = GameController.SelectHeroToDiscardCard(DecisionMaker, storedResultsDiscard: storedResults, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}

						if (!DidDiscardCards(storedResults))
						{
							yield break;
						}

						Card discardedCard = storedResults.First().CardToDiscard;
						IEnumerable<string> keywords = discardedCard.GetKeywords();
						coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(FindTurnTakerController(discardedCard.Owner), discardedCard.Owner.Deck, playMatchingCards: true, putMatchingCardsIntoPlay: true, moveMatchingCardsToHand: false, cardCriteria: new LinqCardCriteria(c => c.GetKeywords().Intersect(keywords).Any()), numberOfMatches: 1);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
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
