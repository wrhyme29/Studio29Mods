using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;
using Handelabra;

namespace Studio29.CovenOfWitches
{
    public class CurseOfSerenCardController : CovenOfWitchesCardController
    {

        public CurseOfSerenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"{Game.Journal.GetCardPropertiesStringList(Card, AffectedTargetsKey).ToCommaList(useWordAnd: true)} cannot deal damage this turn.", showInEffectsList: () => true).Condition = () => Game.Journal.GetCardPropertiesStringList(Card, AffectedTargetsKey) != null && Game.Journal.GetCardPropertiesStringList(Card, AffectedTargetsKey).Count() > 0;
        }

        private readonly string AffectedTargetsKey = "AffectedTargets";

        public override void AddTriggers()
        {
            // At the start of the Environment turn, this cards deals all targets 2 infernal damage. If all targets were dealt damage this way, destroy this card.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            });

            // Targets may not deal damage the turn they enter play.
            AddTargetEntersPlayTrigger((Card c) => c.IsTarget, CannotDealDamageResponse, TriggerType.CreateStatusEffect, TriggerTiming.After);
            AddEndOfTurnTrigger(tt => true, ClearAffectedTargetsListResponse, TriggerType.HiddenLast);

        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {

            List<DealDamageAction> storedResults = new List<DealDamageAction>() ;
            IEnumerator coroutine = DealDamage(Card, c => c.IsTarget && c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 2, DamageType.Infernal, storedResults: storedResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (!storedResults.All(dd => dd.OriginalTarget == dd.Target && dd.Amount > 0))
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

        private IEnumerator CannotDealDamageResponse(Card target)
        {
            List<string> affectedTargets = Game.Journal.GetCardPropertiesStringList(Card, AffectedTargetsKey)?.ToList() ?? new List<string>();

            affectedTargets.Add(target.Title);

            Game.Journal.RecordCardProperties(Card, AffectedTargetsKey, affectedTargets);

            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = target;
            cannotDealDamageStatusEffect.UntilThisTurnIsOver(Game);
            cannotDealDamageStatusEffect.UntilCardLeavesPlay(target);

            return GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, GetCardSource());
        }

        private IEnumerator ClearAffectedTargetsListResponse(PhaseChangeAction pca)
        {
            Game.Journal.RecordCardProperties(Card, AffectedTargetsKey, new List<string>());
            return DoNothing();
        }
    }
}