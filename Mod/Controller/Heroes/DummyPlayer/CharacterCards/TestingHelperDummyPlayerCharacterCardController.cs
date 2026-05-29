using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.DummyPlayer
{
    public class TestingHelperDummyPlayerCharacterCardController : TestPlayerUtilityCharacterCardController
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
                        IEnumerator coroutine2 = base.GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.MoveCardToHand, (Card c) => c.IsHero && (c.IsInDeck || c.IsInTrash), Game.H, storedResults: selectedCards, false, requiredDecisions: 0, cardSource: GetCardSource(), ignoreBattleZone: true);

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
				case 3:
                    {
						IEnumerator coroutine = GameController.SelectHeroToSelectTargetAndDealDamage(DecisionMaker, 10000, DamageType.Radiant, cardSource: GetCardSource());
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
				case 4:
                    {
						List<SelectTurnTakerDecision> storedResults3 = new List<SelectTurnTakerDecision>();
						IEnumerator coroutine4 = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.MoveDeckToTrash, storedResults3, optional: false, allowAutoDecide: false, null, null, null, checkExtraTurnTakersInstead: false, canBeCancelled: true, ignoreBattleZone: false, GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine4);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine4);
						}
						SelectTurnTakerDecision selectTurnTakerDecision2 = storedResults3.FirstOrDefault();
						if (selectTurnTakerDecision2 != null)
						{
							coroutine4 = base.GameController.BulkMoveCards(base.TurnTakerController, selectTurnTakerDecision2.SelectedTurnTaker.Deck.Cards, selectTurnTakerDecision2.SelectedTurnTaker.Trash, toBottom: false, performBeforeDestroyActions: true, DecisionMaker.TurnTaker, isDiscard: false, GetCardSource());
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine4);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine4);
							}
						}
						break;
					}
				case 5:
                    {

						IEnumerator coroutine = SelectAndRemoveEnvironment();
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
				case 6:
                    {

						List<SelectLocationDecision> locationResults = new List<SelectLocationDecision>();
						IEnumerator coroutine = GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealCardsFromDeck,
							location => location.IsDeck && location.IsRealDeck && GameController.IsLocationVisibleToSource(location, GetCardSource()), locationResults);

						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						
						if(!DidSelectLocation(locationResults))
                        {
							break;
                        }

						Location deck = GetSelectedLocation(locationResults);

						coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(DecisionMaker, deck, deck.NumberOfCards, new LinqCardCriteria(c => true));
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
				case 7:
                    {

                        List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
                        IEnumerator coroutine = GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.HeroToDealDamage, c => c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame, 1, storedResults, false, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						if(!DidSelectCards(storedResults))
                        {
							break;
                        }
						Card selectedSource = GetSelectedCards(storedResults).First();
						coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, selectedSource), 2, DamageType.Melee, 1, false, 1, cardSource: GetCardSource());
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
				case 8:
                    {
						IEnumerator coroutine = DoActionToEachTurnTakerInTurnOrder(ttc => !ttc.IsIncapacitatedOrOutOfGame, ttc => DiscardCardsFromTopOfDeck(ttc, 5, showMessage: true));
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
		}

		public IEnumerator EnvironmentRemoval(TurnTakerController env)
		{
			IEnumerator coroutine = base.GameController.RemoveAllCardsFromGame(env, base.TurnTakerController);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			IEnumerable<Card> cardsToMove = from c in env.TurnTaker.PlayArea.Cards.Concat(env.TurnTaker.Deck.Cards).Concat(env.TurnTaker.Trash.Cards)
											where c.Owner != env.TurnTaker
											select c;
			coroutine = base.GameController.MoveCards(base.TurnTakerController, cardsToMove, delegate (Card c)
			{
				MoveCardDestination trashDestination = FindCardController(c).GetTrashDestination();
				if (trashDestination.Location == env.TurnTaker.Trash)
				{
					trashDestination.Location = (c.Definition.ParentDeck.IsSubDeck ? c.Owner.FindSubTrash(c.Definition.ParentDeck.Identifier) : c.Owner.Trash);
				}
				return trashDestination;
			}, toBottom: false, isPutIntoPlay: false, playIfMovingToPlayArea: true, base.TurnTaker, showIndividualMessages: false, isDiscard: false, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (base.TurnTakerController.BattleZone != env.BattleZone)
			{
				coroutine = base.GameController.SwitchBattleZone(base.TurnTakerController, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			string message = $"{env.Name} has been removed from existence by Dummy Player!";
			coroutine = base.GameController.SendMessageAction(message, Priority.High, GetCardSource(), null, showCardSource: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (base.Game.ExtraTurnTakers.Where((TurnTaker tt) => tt.IsEnvironment).Any())
			{
				List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
				coroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.EnvironmentDeck, storedResults, optional: false, allowAutoDecide: false, null, null, null, checkExtraTurnTakersInstead: true, canBeCancelled: true, ignoreBattleZone: true, GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				SelectTurnTakerDecision selectTurnTakerDecision = storedResults.FirstOrDefault();
				if (selectTurnTakerDecision != null && selectTurnTakerDecision.SelectedTurnTaker != null)
				{
					coroutine = base.GameController.ReplaceTurnTaker(env.TurnTaker, selectTurnTakerDecision.SelectedTurnTaker, keepTurnTakerController: false, keepCards: false, null, GetCardSource());
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
			else
			{
				coroutine = base.GameController.GameOver(EndingResult.AlternateDefeat, "The Multiverse has been destroyed.", showEndingTextAsMessage: true, null, null, GetCardSource());
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

		private IEnumerator SelectAndRemoveEnvironment()
        {
			List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
			IEnumerator coroutine = base.GameController.SelectTurnTaker(DecisionMaker, SelectionType.RemoveEnvironmentFromGame, storedResults, optional: false, allowAutoDecide: false, additionalCriteria: tt => tt.IsEnvironment, null, null, checkExtraTurnTakersInstead: false, canBeCancelled: true, ignoreBattleZone: true, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (!DidSelectTurnTaker(storedResults))
			{
				yield break;
			}

			TurnTakerController selectedEnv = FindTurnTakerController(GetSelectedTurnTaker(storedResults));
			coroutine = EnvironmentRemoval(selectedEnv);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			YesNoDecision yesNo = new YesNoDecision(GameController, DecisionMaker, SelectionType.Custom, cardSource: GetCardSource());
			coroutine = GameController.MakeDecisionAction(yesNo);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if(!DidPlayerAnswerYes(yesNo))
            {
				yield break;
            }

			coroutine = SelectAndRemoveEnvironment();
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

			return new CustomDecisionText("Do you want to remove another environment?", "Should they remove another environment?", "Vote for if they should remove another environment?", "remove another environment");

		}

	}
}
