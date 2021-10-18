using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class GiftRegistryCardController : BirthdayBoyCardController
    {

        public GiftRegistryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsUnderCard(Card);
			AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);
		}

		private const string PrimedKey = "Primed";

		private bool IsPotentialEmptierAction(GameAction ga)
		{
			if (ga is PlayCardAction pc)
			{
				return pc.Origin == Card.UnderLocation;
			}
			if (ga is MoveCardAction mc)
			{
				return mc.Origin == Card.UnderLocation;
			}
			if (ga is BulkMoveCardsAction bmc)
			{
				return bmc.CardsToMove.Any((Card c) => bmc.FindOriginForCard(c) == Card.UnderLocation);
			}
			if (ga is CompletedCardPlayAction ccp)
			{
				return ccp.CardPlayed == Card;
			}
			return false;
		}


		public override IEnumerator PerformEnteringGameResponse()
        {
			SetCardProperty(PrimedKey, false);
			return DoNothing();
        }

		public override void AddTriggers()
		{
			//At the end of your turn, put a card from beneath this one into play.
			AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, TriggerType.PutIntoPlay);

		   //When there are no cards beneath this one, destroy this card.
		   Func<GameAction, bool> destroyCriteria = (GameAction action) => GetCardPropertyJournalEntryBoolean(PrimedKey).Value && Card.UnderLocation.Cards.Count() == 0 && IsPotentialEmptierAction(action);
			AddTrigger(destroyCriteria, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);

			//If this card is destroyed, move all cards under it into the trash
			base.AddBeforeLeavesPlayAction(new Func<GameAction, IEnumerator>(MoveCardsUnderThisCardToTrash), TriggerType.MoveCard);
		}

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
			IEnumerator coroutine = GameController.SelectAndPlayCard(DecisionMaker, (Card c) => Card.UnderLocation.HasCard(c), isPutIntoPlay: true, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

		}

		public override IEnumerator Play()
		{
			//When this card enters play, each other player selects an ongoing, equipment, or target with max 5 HP from their deck.

			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = SelectHeroesToSelectCardsFromLocationsAndMoveThemAndStoreResults(DecisionMaker,
																						new LinqTurnTakerCriteria(tt => tt != TurnTaker && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())),
																						htt => htt.Deck,
																						htt => new List<MoveCardDestination> { new MoveCardDestination(Card.UnderLocation) },
																						1,
																						new LinqCardCriteria(c => c.IsOngoing || IsEquipment(c) || (c.IsTarget && c.MaximumHitPoints <= 5)),
																						storedResults: storedResults);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//Each card now belongs to {BirthdayBoy} and gains the Present keyword.
			foreach(SelectCardDecision scd in storedResults)
            {
				Card selectedCard = scd.SelectedCard;
				Log.Debug("Old owner: " + selectedCard.Owner.Identifier);
				GameController.AddCardPropertyJournalEntry(selectedCard, "OverrideTurnTaker", new string[] { selectedCard.Owner.QualifiedIdentifier, selectedCard.Identifier });
				GameController.ChangeCardOwnership(selectedCard, TurnTaker);

				Log.Debug("New owner: " + selectedCard.Owner.Identifier);
				Log.Debug("Original owner: " + GetOriginalOwner(selectedCard).Identifier);

				coroutine = GameController.ModifyKeywords("present", addingOrRemoving: true, affectedCards: selectedCard.ToEnumerable().ToList(), cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				coroutine = GameController.SendMessageAction($"{selectedCard.Title} is now a Present belonging to { base.Card.AlternateTitleOrTitle}", Priority.High, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

			}

			//this card has cards under it, so mark as primed so that if any future actions result in 0 cards under this one, it is destroyed
			SetCardProperty(PrimedKey, true);
			yield break;
		}

		private IEnumerator SelectHeroesToSelectCardsFromLocationsAndMoveThemAndStoreResults(HeroTurnTakerController hero, LinqTurnTakerCriteria heroCriteria, Func<HeroTurnTaker, Location> sourceLocationBasedOnHero, Func<HeroTurnTaker, IEnumerable<MoveCardDestination>> destinationsBasedOnHero, int numberOfCards, LinqCardCriteria cardCriteria, List<SelectCardDecision> storedResults = null, string messageOnFailure = null)
		{
			LinqTurnTakerCriteria linqTurnTakerCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame, "active heroes");
			if (heroCriteria != null)
			{
				linqTurnTakerCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame && sourceLocationBasedOnHero(tt.ToHero()).Cards.Count() > 0 && heroCriteria.Criteria(tt), heroCriteria.Description);
			}
			if (GameController.AllTurnTakers.Where(linqTurnTakerCriteria.Criteria).Count() == 0)
			{
				if (messageOnFailure == null)
				{
					messageOnFailure = "None of the active heroes can move any cards!";
				}
				IEnumerator coroutine = GameController.SendMessageAction(messageOnFailure, Priority.High, GetCardSource(), null, showCardSource: true);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
				yield break;
			}
			GameController gameController = GameController;
			LinqTurnTakerCriteria criteria = linqTurnTakerCriteria;
			int? numberOfCards2 = numberOfCards;
			int? requiredDecisions = 0;
			CardSource cardSource = GetCardSource();
			SelectTurnTakersDecision selectTurnTakersDecision = new SelectTurnTakersDecision(gameController, hero, criteria, SelectionType.MoveCard, null, isOptional: false, requiredDecisions, eliminateOptions: true, allowAutoDecide: true, numberOfCards2, null, null, cardSource);
			Func<TurnTaker, IEnumerator> actionWithTurnTaker = delegate (TurnTaker tt)
			{
				GameController gameController2 = GameController;
				HeroTurnTakerController hero2 = GameController.FindHeroTurnTakerController(tt.ToHero());
				Location location = sourceLocationBasedOnHero(tt.ToHero());
				int? minNumberOfCards = numberOfCards;
				int maxNumberOfCards = numberOfCards;
				LinqCardCriteria criteria2 = cardCriteria;
				IEnumerable<MoveCardDestination> possibleDestinations = destinationsBasedOnHero(tt.ToHero());
				CardSource cardSource2 = GetCardSource();
				return gameController2.SelectCardsFromLocationAndMoveThem(hero2, location, minNumberOfCards, maxNumberOfCards, criteria2, possibleDestinations, isPutIntoPlay: false, playIfMovingToPlayArea: true, shuffleAfterwards: false, optional: false, storedResults: storedResults, null, autoDecideCard: false, flipFaceDown: false, showOutput: false, null, isDiscardIfMovingToTrash: false, allowAutoDecide: false, null, null, cardSource2);
			};
			IEnumerator coroutine2 = GameController.SelectTurnTakersAndDoAction(selectTurnTakersDecision, actionWithTurnTaker, null, GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine2);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine2);
			}
		}

		private IEnumerator MoveCardsUnderThisCardToTrash(GameAction ga)
		{
			//Move all cards under this to the trash
			IEnumerator coroutine = GameController.MoveCards(TurnTakerController, Card.UnderLocation.Cards, TurnTaker.Trash, cardSource: GetCardSource());
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

	}
}