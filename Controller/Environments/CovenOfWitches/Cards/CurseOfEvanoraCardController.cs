using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.CovenOfWitches
{
    public class CurseOfEvanoraCardController : CovenOfWitchesCardController
	{

        public CurseOfEvanoraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		private readonly string EvanoraPoolIdentifier = "CurseOfEvanoraPool";

		private TokenPool GetEvanoraTokenPool()
		{
			TokenPool elementPool = Card.FindTokenPool(EvanoraPoolIdentifier);
			if (elementPool is null)
			{
				elementPool = CardWithoutReplacements.FindTokenPool(EvanoraPoolIdentifier);
			}

			return elementPool;
		}

		public override void AddTriggers()
        {
			// At the start of any hero's turn, their player may discard a card. If they do, they may draw a card. If a card was drawn this way, put a token on this card.

			AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero, DiscardToUseDrawCardResponse, new TriggerType[]
			{
				TriggerType.DiscardCard,
				TriggerType.DrawCard,
				TriggerType.AddTokensToPool
			});

			// At the start of the Environment turn, 1 Hero may discard their hand to destroy this card.
			AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DiscardHandToDestroy, new TriggerType[]
			{
				TriggerType.DiscardCard,
				TriggerType.DestroySelf
			});

			// At the end of the environment turn, this card deals each hero target X + 1 infernal damage, where X is the number of tokens on this card.
			AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);

			AddWhenDestroyedTrigger((DestroyCardAction dc) => ResetTokenValue(), TriggerType.Hidden);
		}

        private IEnumerator DiscardToUseDrawCardResponse(PhaseChangeAction pca)
		{
			List<DiscardCardAction> storedDiscardResults = new List<DiscardCardAction>();
			HeroTurnTakerController hero = FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());
			IEnumerator coroutine = GameController.SelectAndDiscardCard(hero, optional: true, storedResults: storedDiscardResults, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
			if (!DidDiscardCards(storedDiscardResults))
			{
				yield break;
			}

            List<DrawCardAction> storedDrawResults = new List<DrawCardAction>();
            coroutine = GameController.DrawCard(hero.HeroTurnTaker, storedResults: storedDrawResults, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			if(!DidDrawCards(storedDrawResults))
            {
				yield break;
            }

			TokenPool evanoraPool = GetEvanoraTokenPool();

			coroutine = GameController.AddTokensToPool(evanoraPool, 1, GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

		}

		private IEnumerator DiscardHandToDestroy(PhaseChangeAction pca)
		{
			List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
			IEnumerator coroutine = GameController.SelectHeroToDiscardTheirHand(DecisionMaker, optionalSelectHero: true, optionalDiscardCards: false, storedResultsTurnTaker:  storedResults, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
			if (!DidSelectTurnTaker(storedResults))
			{
				yield break;
			}

			coroutine = DestroyThisCardResponse(pca);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
		}

		private IEnumerator DealDamageResponse(PhaseChangeAction pca)
		{
			int X = GetEvanoraTokenPool().CurrentValue;
			return DealDamage(Card, c => c.IsHero && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), X + 1, DamageType.Infernal);
		}

		public IEnumerator ResetTokenValue()
		{
			GetEvanoraTokenPool().SetToInitialValue();
			yield return DoNothing();
		}

	}
}