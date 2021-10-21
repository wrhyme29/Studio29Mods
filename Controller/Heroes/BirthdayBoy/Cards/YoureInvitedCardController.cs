using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;

namespace Studio29.BirthdayBoy
{
    public class YoureInvitedCardController : BirthdayBoyCardController
    {

        public YoureInvitedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
			//Any other hero with no presents in play may play an ongoing card, equipment card, or target with no more than a max hp of 5.

			return SelectHeroesToPlayCards(DecisionMaker, 
											new LinqTurnTakerCriteria(tt => tt != TurnTaker && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())),
											new LinqCardCriteria(c => c.IsOngoing || IsEquipment(c) || (c.IsTarget && c.MaximumHitPoints <= 5)),
											GetCardSource());
			
        }


		private IEnumerator SelectHeroesToPlayCards(HeroTurnTakerController hero, LinqTurnTakerCriteria heroCriteria, LinqCardCriteria cardCriteria, CardSource cardSource)
		{
			LinqTurnTakerCriteria linqTurnTakerCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame, "active heroes");
			if (heroCriteria != null)
			{
				linqTurnTakerCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame && tt.ToHero().Hand.Cards.Count(c => cardCriteria.Criteria(c)) > 0 && heroCriteria.Criteria(tt), heroCriteria.Description);
			}
			if (GameController.AllTurnTakers.Where(linqTurnTakerCriteria.Criteria).Count() == 0)
			{
				string messageOnFailure = "None of the active heroes can play any valid cards!";
				
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
			SelectTurnTakersDecision selectTurnTakersDecision = new SelectTurnTakersDecision(gameController, hero, criteria, SelectionType.PlayCard, allowAutoDecide: true, cardSource: cardSource);
			Func<TurnTaker, IEnumerator> actionWithTurnTaker = delegate (TurnTaker tt)
			{
				GameController gameController2 = GameController;
				HeroTurnTakerController hero2 = GameController.FindHeroTurnTakerController(tt.ToHero());
				LinqCardCriteria criteria2 = cardCriteria;

				return gameController2.SelectAndPlayCardsFromHand(hero2, 1, false, requiredCards: 0, cardCriteria: criteria2, cardSource: cardSource);
			};
			IEnumerator coroutine2 = GameController.SelectTurnTakersAndDoAction(selectTurnTakersDecision, actionWithTurnTaker, cardSource: cardSource);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine2);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine2);
			}
		}

		


	}
}