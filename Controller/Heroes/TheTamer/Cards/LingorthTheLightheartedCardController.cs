using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class LingorthTheLightheartedCardController : TheTamerCardController
    {

        public LingorthTheLightheartedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //When this card is dealt exactly 1 damage, it deals one non-hero target 4 melee damage.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.Card && dd.Amount == 1, DealDamageExactly1Response, TriggerType.DealDamage, TriggerTiming.After);
            //When this card is dealt more than 1 damage, it deals {TheTamer} 2 melee damage"
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.Card && dd.Amount > 1, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);

        }

        private IEnumerator DealDamageResponse(DealDamageAction arg)
        {
            //When this card is dealt more than 1 damage, it deals {TheTamer} 2 melee damage"

            IEnumerator coroutine = DealDamage(base.Card, base.CharacterCard, 2, DamageType.Melee, cardSource: GetCardSource());
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

        private IEnumerator DealDamageExactly1Response(DealDamageAction dd)
        {
            //it deals one non-hero target 4 melee damage.
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(GameController, base.Card), 4, DamageType.Melee, new int?(1), false, new int?(1), additionalCriteria: (Card c) => !c.IsHero && c.IsTarget, cardSource: GetCardSource()) ;
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