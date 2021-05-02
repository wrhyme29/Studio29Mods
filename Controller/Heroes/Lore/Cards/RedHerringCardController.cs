using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class RedHerringCardController : LoreCardController
    {

        public RedHerringCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //If there is a mystery card in play, select a target. Redirect all damage dealt to that target to the environment target with the lowest hp until the start of your next turn.            
            bool mysteryCardInPlay = FindCardsWhere(c => c.IsInPlayAndHasGameText && IsMystery(c)).Any();
            IEnumerator coroutine;
            if (mysteryCardInPlay)
            {
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.Custom, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, false, cardSource: GetCardSource());
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
                OnDealDamageStatusEffect onDealDamageStatusEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(RedirectDamageResponse), "Redirect all damage dealt to " + target.Title + " to the environment target with the lowest HP.", new TriggerType[]
                    {
                        TriggerType.RedirectDamage
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
                //If there is not a mystery card in play, draw a card.
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

        public IEnumerator RedirectDamageResponse(DealDamageAction action, HeroTurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {

            if(!base.FindCardsWhere((Card c) => c.IsEnvironmentTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource())).Any())
            {
                yield break;
            }

            //...redirect damage dealt to the selected target to the environment target with the lowest HP.
            IEnumerator coroutine = base.RedirectDamage(action, TargetType.LowestHP, (Card c) => c.IsEnvironmentTarget && c.IsInPlayAndHasGameText);
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Select a target to have all damage dealt to them redirected to the environment target with the lowest HP.", "Select a target to have all damage dealt to them redirected to the environment target with the lowest HP.", "Vote for which target to have all damage dealt to them redirected to the environment target with the lowest HP.", "target to have all damage dealt to them redirected to the environment target with the lowest HP.");

        }

    }
}