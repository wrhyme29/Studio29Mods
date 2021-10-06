using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Debugger
{
    public class StackADeckCardController : OptionsCardController
	{

        public StackADeckCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
			//Select a deck. Select any number of cards from that deck. Place the selected cards on the top of the deck in any order.
			List<SelectLocationDecision> storedDeck = new List<SelectLocationDecision>();
            IEnumerator coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.Custom, loc => loc.IsDeck, storedDeck, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (!DidSelectLocation(storedDeck))
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
			List<SelectCardsDecision> selectedCards = new List<SelectCardsDecision>();
			Location selectedDeck = GetSelectedLocation(storedDeck);
			Location selectedTrash = FindTrashFromDeck(selectedDeck);
			HeroTurnTakerController httc = selectedDeck.OwnerTurnTaker.IsHero ? FindHeroTurnTakerController(selectedDeck.OwnerTurnTaker.ToHero()) : DecisionMaker;
			Location selectedHand = selectedDeck.OwnerTurnTaker.IsHero ? httc.HeroTurnTaker.Hand : null;

			SelectCardsDecision scd = new SelectCardsDecision(GameController, httc, (Card c) => selectedDeck.HasCard(c) || selectedTrash.HasCard(c) || (selectedDeck.IsHero ? selectedHand.HasCard(c) : false), SelectionType.MoveCardOnDeck, numberOfCards: null, requiredDecisions: 0, eliminateOptions: true, cardSource: GetCardSource());
			selectedCards.Add(scd);
			coroutine = GameController.SelectCardsAndDoAction(scd, (SelectCardDecision card) => GameController.MoveCard(TurnTakerController, card.SelectedCard, selectedDeck, cardSource: GetCardSource()));
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
			return new CustomDecisionText($"Select a deck to stack the top cards of",
											"They are selecting a deck to stack the top cards of",
											"Vote for a deck to stack the top cards of",
											"selecting a deck to stack the top cards of");

		}


	}
}