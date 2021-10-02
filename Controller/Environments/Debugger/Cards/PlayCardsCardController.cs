using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Debugger
{
    public class PlayCardsCardController : CardController
    {

        public PlayCardsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override IEnumerator Play()
		{
			//Select a deck. Play any number of cards from that deck or trash. If a hero deck was selected, you may also select cards from that hero's hand.
			List<SelectLocationDecision> storedDeck = new List<SelectLocationDecision>();
			IEnumerator coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.Custom, loc => loc != TurnTaker.Deck, storedDeck, cardSource: GetCardSource());
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

			SelectCardsDecision scd = new SelectCardsDecision(GameController, httc, (Card c) => selectedDeck.HasCard(c) || selectedTrash.HasCard(c) || (selectedDeck.IsHero ? selectedHand.HasCard(c) : false), SelectionType.PlayCard, numberOfCards: null, requiredDecisions: 0, eliminateOptions: true, cardSource: GetCardSource());
			selectedCards.Add(scd);
			coroutine = GameController.SelectCardsAndDoAction(scd, (SelectCardDecision card) => GameController.PlayCard(httc, card.SelectedCard, cardSource: GetCardSource()));
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
			return new CustomDecisionText($"Select a deck to play cards from",
											"They are selecting a deck to play cards from",
											"Vote for a deck to play cards from",
											"selecting a deck to play cards from");

		}


	}
}