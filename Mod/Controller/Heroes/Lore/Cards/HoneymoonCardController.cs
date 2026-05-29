using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class HoneymoonCardController : LoreCardController
    {

        public HoneymoonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public readonly string FirstTimeDealtDamageKey = "FirstTimeDealtDamage";

        public override IEnumerator Play()
        {
            //If there is a romance card in play, select a target. Prevent the first damage dealt to that target each turn until the start of your next turn.
            bool romanceCardInPlay = FindCardsWhere(c => c.IsInPlayAndHasGameText && IsRomance(c)).Any();
            IEnumerator coroutine;
            if (romanceCardInPlay)
            {
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.PreventFirstDamageEachTurn, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, false, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (!DidSelectCard(storedResults))
                {
                    yield break;
                }

                Card target = GetSelectedCard(storedResults);
                OnDealDamageStatusEffect onDealDamageStatusEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(PreventFirstDamagePerTurn), "Prevent the first damage dealt to " + target.Title + " each turn.", new TriggerType[]
                    {
                        TriggerType.CancelAction
                    }, null, Card);
                onDealDamageStatusEffect.BeforeOrAfter = BeforeOrAfter.Before;
                onDealDamageStatusEffect.TargetCriteria.IsSpecificCard = target;
                onDealDamageStatusEffect.UntilStartOfNextTurn(TurnTaker);
                onDealDamageStatusEffect.UntilTargetLeavesPlay(target);
                coroutine = AddStatusEffect(onDealDamageStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                //If there is not a romance card in play, draw a card.
                coroutine = DrawCard();
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

        public IEnumerator PreventFirstDamagePerTurn(DealDamageAction dd, HeroTurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            string key = GeneratePerTargetKey(FirstTimeDealtDamageKey, dd.Target);
            if (!HasBeenSetToTrueThisTurn(key))
            {
                SetCardPropertyToTrueIfRealAction(key);
                IEnumerator coroutine = CancelAction(dd);
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