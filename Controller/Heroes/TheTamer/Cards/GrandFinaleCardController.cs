using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class GrandFinaleCardController : TheTamerCardController
    {

        public GrandFinaleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //If damage dealt by a Lion would reduce {TheTamer} to 0 HP, {TheTamer} may first deal one non-hero target 10 irreducible energy damage.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.DamageSource != null & IsLion(dd.DamageSource.Card) && !dd.IsPretend && dd.Amount >= dd.Target.HitPoints, DealDamageResponse, new TriggerType[]
                {
                    TriggerType.WouldBeDealtDamage,
                    TriggerType.DealDamage
                }, TriggerTiming.Before) ;

        }

        private IEnumerator DealDamageResponse(DealDamageAction arg)
        {
            //{TheTamer} may first deal one non-hero target 10 irreducible energy damage.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 10, DamageType.Energy, 1, false, 0, additionalCriteria: (Card c) => !c.IsHero && c.IsTarget, cardSource: GetCardSource());
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