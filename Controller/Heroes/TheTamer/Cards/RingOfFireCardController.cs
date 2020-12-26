using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class RingOfFireCardController : TheTamerCardController
    {

        public RingOfFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Until the start of your next turn, increase damage dealt by Lions to non-hero targets by 1.
            IncreaseDamageStatusEffect effect = new IncreaseDamageStatusEffect(1);
            effect.SourceCriteria.HasAnyOfTheseKeywords = new List<string>() { "lion" };
            effect.TargetCriteria.IsHero = false;
            effect.TargetCriteria.IsTarget = true;
            effect.UntilStartOfNextTurn(base.TurnTaker);
            IEnumerator coroutine = AddStatusEffect(effect);
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