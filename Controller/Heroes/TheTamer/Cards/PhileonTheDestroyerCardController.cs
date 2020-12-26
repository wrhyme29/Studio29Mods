using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class PhileonTheDestroyerCardController : TheTamerCardController
    {

        public PhileonTheDestroyerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //When this card is dealt damage, it deals { TheTamer} 1 melee damage and another target 2 melee damage.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.Card && dd.DidDealDamage, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);

        }

        private IEnumerator DealDamageResponse(DealDamageAction arg)
        {
            //it deals { TheTamer} 1 melee damage and another target 2 melee damage.
            IEnumerator coroutine = DealDamage(base.Card, base.CharacterCard, 1, DamageType.Melee, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(GameController, base.Card), 2, DamageType.Melee, new int?(1), false, new int?(1), additionalCriteria: (Card c) => c != base.CharacterCard, cardSource: GetCardSource());
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