using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.CovenOfWitches
{
	public class CurseOfSybilCardController : CovenOfWitchesCardController
	{

		public CurseOfSybilCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{

		}

		private readonly string CardBlockedKey = "CardBlocked";

		public override void AddTriggers()
		{
			// When a villain card would enter play, you may discard it instead and destroy this card.
			AddTrigger((CardEntersPlayAction c) => c.CardEnteringPlay != Card && IsVillain(c.CardEnteringPlay) && !c.Origin.IsInPlay && GetCardPropertyJournalEntryCard(CardBlockedKey) == null && GameController.IsCardVisibleToCardSource(c.CardEnteringPlay, GetCardSource()), DiscardAndDestroyResponse, new TriggerType[]
			{
				TriggerType.CancelAction
			}, TriggerTiming.Before);

			// When this card is destroyed, this card deals all non-environment targets {H} infernal damage.
			AddWhenDestroyedTrigger((DestroyCardAction dca) => DealDamage(Card, c => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), Game.H, DamageType.Infernal), TriggerType.DealDamage);
		}

		private IEnumerator DiscardAndDestroyResponse(CardEntersPlayAction action)
		{
			Card card = action.CardEnteringPlay;
			List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
			IEnumerator coroutine = GameController.MakeYesNoCardDecision(HeroTurnTakerController, SelectionType.DiscardCard, card, storedResults: storedResults, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
			if (DidPlayerAnswerYes(storedResults))
			{
				AddCardPropertyJournalEntry(CardBlockedKey, card);
				coroutine = CancelAction(action);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
				MoveCardDestination trashDestination = FindCardController(card).GetTrashDestination();
				coroutine = GameController.MoveCard(TurnTakerController, card, trashDestination.Location, toBottom: trashDestination.ToBottom, responsibleTurnTaker: TurnTaker, evenIfIndestructible: true, isDiscard: true, cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
				coroutine = DestroyThisCardResponse(action);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
				if (Card.IsInPlay)
				{
					Card card2 = null;
					AddCardPropertyJournalEntry("CardBlocked", card2);
				}
			}

		}
	}
}