using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.CovenOfWitches
{
    public class CurseOfArwenCardController : TiedCursesCardController
    {
        public CurseOfArwenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "CurseOfLillith")
        {
            SpecialStringMaker.ShowSpecialString(() => $"{Game.ActiveTurnTaker.NameRespectingVariant} cannot deal damage this turn.", showInEffectsList: () => true).Condition = () => Game.ActiveTurnTaker.IsHero && Card.IsInPlayAndHasGameText;
        }


      
        public override void AddTriggers()
        {
            // At the start of the Environment turn, 1 Player may discard 2 cards to destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, OnePlayerMayDiscardTwoCardsToDestroyThisCardResponse, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DestroySelf
            });

            //  Hero characters may not deal damage on their own turn.
            AddCannotDealDamageTrigger((Card c) => c.IsHeroCharacterCard && c.Owner == Game.ActiveTurnTaker);

            // Add TiedCurses triggers
            base.AddTriggers();
        }

		private IEnumerator OnePlayerMayDiscardTwoCardsToDestroyThisCardResponse(PhaseChangeAction phaseChange)
		{
			if (GameController.AllHeroes.Any((HeroTurnTaker hero) => hero.Hand.Cards.Count() >= 2))
			{
				Log.Debug("1 player may discard 2 cards to destroy " + Card.Title + ".");
				List<SelectTurnTakerDecision> selectHero = new List<SelectTurnTakerDecision>();
				IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.Custom, optional: true, allowAutoDecide: false, storedResults: selectHero, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && tt.ToHero().NumberOfCardsInHand >= 2, "heroes with 2 or more cards in hand"), cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
				if (!DidSelectTurnTaker(selectHero))
				{
					yield break;
				}
				TurnTaker selectedTurnTaker = GetSelectedTurnTaker(selectHero);
				if (selectedTurnTaker.IsHero)
				{
					HeroTurnTakerController httc = FindHeroTurnTakerController(selectedTurnTaker.ToHero());
                    List<DiscardCardAction> storedDiscards = new List<DiscardCardAction>();
                    IEnumerator coroutine2 = GameController.SelectAndDiscardCards(httc, 2, optional: false, requiredDiscards: 2, storedResults: storedDiscards, cardSource: GetCardSource());
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(coroutine2);
					}
					else
					{
						GameController.ExhaustCoroutine(coroutine2);
					}

					if (DidDiscardCards(storedDiscards, numberExpected: 2))
					{

						IEnumerator coroutine3 = DestroyThisCardResponse(phaseChange);
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine3);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine3);
						}
					}
				}
			}
			else
			{
				Log.Debug("No player has enough cards in hand to destroy " + Card.Title);
				IEnumerator coroutine4 = GameController.SendMessageAction("No player has enough cards in hand to destroy " + Card.Title, Priority.High, GetCardSource(), new Card[]
				{
					Card
				});
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine4);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine4);
				}
			}
		}

		public override CustomDecisionText GetCustomDecisionText(IDecision decision)
		{

			return new CustomDecisionText("Select a player to discard 2 cards", "Select a player to discard 2 cards", "Vote for a player to discard 2 cards", "player to discard 2 cards");

		}


	}
}