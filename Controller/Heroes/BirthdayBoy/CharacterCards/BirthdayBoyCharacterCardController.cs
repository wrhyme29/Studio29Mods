using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.BirthdayBoy
{
	public class BirthdayBoyCharacterCardController : BirthdayBoyUtilityCharacterCardController
	{
		public override bool AllowFastCoroutinesDuringPretend => false;
		public override bool UseDecisionMakerAsCardOwner => true;

		private bool _checkForReplacements = false;
		public BirthdayBoyCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);
		}

		public override void AddTriggers()
		{
			AddThisCardControllerToList(CardControllerListType.ReplacesCards);
			AddThisCardControllerToList(CardControllerListType.ReplacesTurnTakerController);
			AddTrigger((GameAction ga) => ga.CardSource != null && IsPresent(ga.CardSource.Card), AddThisAsAssociatedCardSource, TriggerType.Hidden, TriggerTiming.Before);			
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//{BirthdayBoy} may move any hero ongoing, hero equipment, or hero target with max 5hp or fewer in play to your play area. Any card moved this way now belongs to {BirthdayBoy} (when it is destroyed, shuffle into the deck of {BirthdayBoy}). Any card moved this way gains the keyword “Present”.
			IEnumerator coroutine = MoveCardsToOwnPlayArea();
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

        private IEnumerator MoveCardsToOwnPlayArea()
        {
			//{BirthdayBoy} may move any hero ongoing, hero equipment, or hero target with max 5hp or fewer in play to your play area.
			LinqCardCriteria criteria = new LinqCardCriteria((Card c) => c.Owner != base.TurnTaker && c.IsInPlayAndHasGameText && c.IsHero && (c.IsOngoing || IsEquipment(c) || (c.IsTarget && c.MaximumHitPoints <= 5)), "hero ongoing, hero equipment, or hero target with max 5hp");
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.MoveCards(base.HeroTurnTakerController, criteria, (Card c) => base.TurnTaker.PlayArea,numberOfCards: new int?(1), requiredDecisions: new int?(0), playIfMovingToPlayArea: false, storedResults: storedResults, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if(DidSelectCard(storedResults))
            {
				//Any card moved this way now belongs to { BirthdayBoy} (when it is destroyed, shuffle into the deck of { BirthdayBoy}).
				Card movedCard = GetSelectedCard(storedResults);
				Log.Debug("Old owner: " + movedCard.Owner.Identifier);
				movedCard.SetNewOwner(base.TurnTaker);
				Log.Debug("New owner: " + movedCard.Owner.Identifier);
				Log.Debug("Original owner: " + GetOriginalOwner(movedCard).Identifier);

				coroutine = GameController.SendMessageAction($"{movedCard.Title} is now a Present belonging to { base.Card.AlternateTitleOrTitle}", Priority.High, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				CardController cardController = FindCardController(movedCard);
				if (!cardController.DoesHaveActivePlayMethod)
				{
					coroutine = cardController.Play();
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
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

		public override bool AskIfCardContainsKeyword(Card card, string keyword, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
		{
			if (card.Owner == base.TurnTaker && card.Owner != GetOriginalOwner(card) && keyword == "present")
			{
				return true;
			}
			return base.AskIfCardContainsKeyword(card, keyword, evenIfUnderCard, evenIfFaceDown);
		}

		private IEnumerator AddThisAsAssociatedCardSource(GameAction ga)
		{
			ga.CardSource.AddAssociatedCardSource(GetCardSource());
			yield return null;
		}

		public override Card AskIfCardIsReplaced(Card card, CardSource cardSource)
		{
			if (!_checkForReplacements && cardSource != null && cardSource.AllowReplacements)
			{
				_checkForReplacements = true;
				Card cardWithoutReplacements = cardSource.CardController.CardWithoutReplacements;
				if ( cardWithoutReplacements != null && IsPresent(cardWithoutReplacements) && GetOriginalOwner(cardWithoutReplacements).CharacterCards.Contains(card))
				{
					Card result = base.CharacterCard;
					_checkForReplacements = false;
					return result;
				}
				_checkForReplacements = false ;

			}
			return null;
		}

		public override TurnTakerController AskIfTurnTakerControllerIsReplaced(TurnTakerController ttc, CardSource cardSource)
		{
			if (!_checkForReplacements && cardSource != null && cardSource.Card.Owner != base.Card.Owner && cardSource.AllowReplacements)
			{
				_checkForReplacements = true;
				Card cardWithoutReplacements = cardSource.CardController.CardWithoutReplacements;
				HeroTurnTakerController heroTurnTakerController = cardSource.FindMostRecentDecisionMaker();
				bool flag = heroTurnTakerController == null || heroTurnTakerController == base.TurnTakerControllerWithoutReplacements;
				if (cardWithoutReplacements != null && ttc.TurnTaker == GetOriginalOwner(cardWithoutReplacements) && flag)
				{
					if (IsPresent(cardWithoutReplacements))
					{
						TurnTakerController result = base.TurnTakerController;
						_checkForReplacements = false;
						return base.TurnTakerController;
					}
					if (cardSource.CardSourceChain.Any((CardSource cs) => cs.CardController == this))
					{
						TurnTakerController result = base.TurnTakerController;
						_checkForReplacements = false;
						return base.TurnTakerController;
					}
				}

				_checkForReplacements = false;
			}
			return null;
		}



	}
}
