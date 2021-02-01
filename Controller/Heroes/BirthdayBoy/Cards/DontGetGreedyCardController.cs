using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class DontGetGreedyCardController : BirthdayBoyCardController
    {

        public DontGetGreedyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
            AddInhibitorException((GameAction ga) => ga is PlayCardAction && Card.Location.IsHand);
        }

		private IEnumerator PlayFromHandResponse()
		{
			IEnumerator coroutine = base.GameController.SendMessageAction(base.Card.Title + " puts itself into play.", Priority.High, GetCardSource(), null, showCardSource: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = base.GameController.PlayCard(base.TurnTakerController, base.Card, isPutIntoPlay: true, null, optional: false, null, null, evenIfAlreadyInPlay: false, null, null, null, associateCardSource: false, fromBottom: false, canBeCancelled: true, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override IEnumerator PerformEnteringGameResponse()
		{
			IEnumerator coroutine = ((!base.Card.IsInHand) ? base.PerformEnteringGameResponse() : PlayFromHandResponse());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override void AddStartOfGameTriggers()
		{
			AddTrigger((DrawCardAction d) => d.DrawnCard == base.Card, (DrawCardAction d) => PlayFromHandResponse(), new TriggerType[2]
			{
			TriggerType.PutIntoPlay,
			TriggerType.Hidden
			}, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
			AddTrigger((MoveCardAction m) => m.Destination == base.HeroTurnTaker.Hand && m.CardToMove == base.Card, (MoveCardAction m) => PlayFromHandResponse(), new TriggerType[2]
			{
			TriggerType.PutIntoPlay,
			TriggerType.Hidden
			}, TriggerTiming.After, null, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: true);
		}
		public override IEnumerator Play()
        {
			IEnumerator coroutine;
			//All heroes deal Birthday Boy 1 irreducible psychic damage for each present from them currently in play.
			IEnumerable<Card> presentList = GetPresentsInPlay();
			IEnumerable<TurnTaker> heroList = FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && presentList.Where(present => GetOriginalOwner(present) == tt).Any());
			if(heroList.Any())
            {

				foreach(TurnTaker tt in heroList)
                {
					HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
					Card heroCharacter = null;
					if(httc.HasMultipleCharacterCards)
                    {
						List<Card> storedResults = new List<Card>();
						coroutine = SelectActiveHeroCharacterCardToDealDamage(storedResults, 1, DamageType.Psychic, true, tt);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						heroCharacter = storedResults.FirstOrDefault();
					
					} else
                    {
						heroCharacter = httc.CharacterCard;
                    }

					if(heroCharacter != null)
                    {
						for(int i=0; i<presentList.Where(c => GetOriginalOwner(c) == tt).Count(); i++)
                        {
							coroutine = DealDamage(heroCharacter, CharacterCard, 1, DamageType.Psychic, isIrreducible: true, cardSource: GetCardSource());
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

					
				}
			}

			//Then, destroy this card and draw a card.
			coroutine = GameController.DestroyCard(HeroTurnTakerController, Card, postDestroyAction: () => DrawCard(), cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		protected IEnumerator SelectActiveHeroCharacterCardToDealDamage(List<Card> storedResults, int damageAmount, DamageType damageType, bool irreducible, TurnTaker tt)
		{
			List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
			DealDamageAction gameAction = new DealDamageAction(GetCardSource(), null, null, damageAmount, damageType, isIrreducible: irreducible);
			IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.HeroToDealDamage, new LinqCardCriteria((Card c) => c.Owner == tt && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "active heroes"), storedDecision, optional: false, gameAction: gameAction, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			SelectCardDecision selectCardDecision = storedDecision.FirstOrDefault();
			if (selectCardDecision != null)
			{
				storedResults.Add(selectCardDecision.SelectedCard);
			}
		}


	}
}