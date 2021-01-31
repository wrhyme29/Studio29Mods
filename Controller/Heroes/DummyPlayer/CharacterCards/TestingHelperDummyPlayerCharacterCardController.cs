using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.DummyPlayer
{
	public class TestingHelperDummyPlayerCharacterCardController : DummyPlayerUtilityCharacterCardController
	{
		public TestingHelperDummyPlayerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}


		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						List<SelectCardsDecision> selectedCards = new List<SelectCardsDecision>();
						IEnumerator coroutine2 = base.GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.MoveCard, (Card c) => IsVillain(c) && (c.IsInDeck || c.IsInTrash), Game.H, storedResults: selectedCards, false, requiredDecisions: 0, cardSource: GetCardSource());

						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine2);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine2);
						}

						if (DidSelectCards(selectedCards))
						{
							IEnumerable<Card> cards = GetSelectedCards(selectedCards);
							coroutine2 = GameController.MoveCards(TurnTakerController, cards, (Card c) => new MoveCardDestination(c.Owner.Deck), cardSource: GetCardSource());
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine2);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine2);
							}
						}
						break;
					}
				case 1:
					{
						List<SelectCardsDecision> selectedCards = new List<SelectCardsDecision>();
                        IEnumerator coroutine2 = base.GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.MoveCardToHand, (Card c) => c.IsHero && (c.IsInDeck || c.IsInTrash), Game.H, storedResults: selectedCards, false, requiredDecisions: 0, cardSource: GetCardSource());

						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine2);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine2);
						}

						if(DidSelectCards(selectedCards))
                        {
							IEnumerable<Card> cards = GetSelectedCards(selectedCards);
							coroutine2 = GameController.MoveCards(TurnTakerController, cards, (Card c) => new MoveCardDestination(c.Owner.ToHero().Hand), cardSource: GetCardSource());
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine2);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine2);
							}
						}
						break;
					}
				case 2:
					{
						List<SelectCardsDecision> selectedCards = new List<SelectCardsDecision>();
						IEnumerator coroutine2 = base.GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.MoveCard, (Card c) => c.IsEnvironment && (c.IsInDeck || c.IsInTrash), Game.H, storedResults: selectedCards, false, requiredDecisions: 0, cardSource: GetCardSource());

						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine2);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine2);
						}

						if (DidSelectCards(selectedCards))
						{
							IEnumerable<Card> cards = GetSelectedCards(selectedCards);
							coroutine2 = GameController.MoveCards(TurnTakerController, cards, (Card c) => new MoveCardDestination(c.Owner.Deck), cardSource: GetCardSource());
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine2);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine2);
							}
						}
						break;
					}
			}
		}


	}
}
