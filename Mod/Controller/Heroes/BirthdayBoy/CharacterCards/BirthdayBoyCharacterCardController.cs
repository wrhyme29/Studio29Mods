using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.BirthdayBoy
{
    public class BirthdayBoyCharacterCardController : BirthdayBoyUtilityCharacterCardController
	{
		public override bool AllowFastCoroutinesDuringPretend => false;
		public override bool UseDecisionMakerAsCardOwner => true;
		private string IncapAdjectiveForCustomDecisionText = "";

		private bool _checkForReplacements = false;
		public BirthdayBoyCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowSpecialString(() => $"{TurnTaker.Name} owns {NumberOfCardsBirthdayBoyOwns} cards.");
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
			IEnumerator coroutine;
			if (NumberOfCardsBirthdayBoyOwns < 40)
			{
				coroutine = MoveCardsToOwnPlayArea();
			} else
            {
				coroutine = GameController.SendMessageAction($"{Card.Title} already owns 40 cards! Let's not go overboard with the presents!", Priority.High, GetCardSource(), showCardSource: true);
            }
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
				GameController.AddCardPropertyJournalEntry(movedCard, "OverrideTurnTaker", new string[] { movedCard.Owner.QualifiedIdentifier, movedCard.Identifier });
				GameController.ChangeCardOwnership(movedCard, TurnTaker);

				Log.Debug("New owner: " + movedCard.Owner.Identifier);
				Log.Debug("Original owner: " + GetOriginalOwner(movedCard).Identifier);

				coroutine = GameController.ModifyKeywords("present", addingOrRemoving: true, affectedCards: movedCard.ToEnumerable().ToList(), cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
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
						IncapAdjectiveForCustomDecisionText = "first";
						IEnumerator coroutine = SelectHeroAndUseIncapitatedAbility(0);
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 1:
					{
						IncapAdjectiveForCustomDecisionText = "second";
						IEnumerator coroutine = SelectHeroAndUseIncapitatedAbility(1);
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
				case 2:
					{
						IncapAdjectiveForCustomDecisionText = "third";
						IEnumerator coroutine = SelectHeroAndUseIncapitatedAbility(2);
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine);
						}
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

		public override IEnumerable<string> AskForCardAdditionalKeywords(Card card)
		{
			if (card.Owner == base.TurnTaker && card.Owner != GetOriginalOwner(card))
			{
				string present = "present";
				return present.ToEnumerable();
			}

			return base.AskForCardAdditionalKeywords(card);
		}

		private IEnumerator SelectHeroAndUseIncapitatedAbility(int abilityIndex)
        {
            List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.Custom,  optional: false, allowAutoDecide: false, storedResults: storedResults, heroCriteria: new LinqTurnTakerCriteria(tt => tt !=TurnTaker),  allowIncapacitatedHeroes: true,cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
			TurnTaker selectedTurnTaker = GetSelectedTurnTaker(storedResults);
			if (selectedTurnTaker == null)
			{
				yield break;
			}
			HeroTurnTakerController selectedHero = FindTurnTakerController(selectedTurnTaker).ToHero();
			coroutine = GameController.UseIncapacitatedAbility(selectedHero, abilityIndex);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override CustomDecisionText GetCustomDecisionText(IDecision decision)
		{
			return new CustomDecisionText($"Select a hero to use the {IncapAdjectiveForCustomDecisionText} incapitated ability of.", $"Select a hero to use the {IncapAdjectiveForCustomDecisionText} incapitated ability of.", $"Vote for the hero to use the {IncapAdjectiveForCustomDecisionText} incapitated ability of.", $"hero to use the {IncapAdjectiveForCustomDecisionText} incapitated ability of");

		}

	}
}
