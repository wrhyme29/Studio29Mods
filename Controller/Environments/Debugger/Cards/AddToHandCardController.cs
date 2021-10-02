using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Debugger
{
    public class AddToHandCardController : CardController
    {

        public AddToHandCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override IEnumerator Play()
        {
			//Select a hero. Select any number of cards from that hero's deck or trash. Move the selected cards into that hero's hand.
			List<SelectTurnTakerDecision> storedTurnTaker = new List<SelectTurnTakerDecision>();
			IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.Custom, false, false, storedTurnTaker, canBeCancelled: false, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if(!DidSelectTurnTaker(storedTurnTaker))
            {
				coroutine = DestroyThisCardResponse(null);
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

			TurnTaker selectedTurnTaker = GetSelectedTurnTaker(storedTurnTaker);
			HeroTurnTakerController selectedHeroTurnTakerController = FindHeroTurnTakerController(selectedTurnTaker.ToHero());

			List<SelectCardsDecision> selectedCards = new List<SelectCardsDecision>();
			SelectCardsDecision scd = new SelectCardsDecision(GameController, selectedHeroTurnTakerController, (Card c) => c.Owner == selectedTurnTaker && (c.IsInDeck || c.IsInTrash), SelectionType.MoveCardToHand, numberOfCards: null, requiredDecisions: 0, eliminateOptions: true, cardSource: GetCardSource());
			selectedCards.Add(scd);
			coroutine = GameController.SelectCardsAndDoAction(scd, (SelectCardDecision card) => DoNothing(), cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (!DidSelectCards(selectedCards))
			{
				coroutine = DestroyThisCardResponse(null);
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
			IEnumerable<Card> cards = GetSelectedCards(selectedCards);
			coroutine = GameController.MoveCards(TurnTakerController, cards, (Card c) => new MoveCardDestination(c.Owner.ToHero().Hand), cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			coroutine = DestroyThisCardResponse(null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

		}

		public override CustomDecisionText GetCustomDecisionText(IDecision decision)
		{
			return new CustomDecisionText(	$"Select a hero to put cards in hand",
											"They are selecting a hero to put cards in hand",
											"Vote for a hero to put cards in hand",
											"selecting a hero to put cards in hand");

		}

	}
}