using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public abstract class LionCardController : TheTamerCardController
    {

        public LionCardController(Card card, TurnTakerController turnTakerController, TriggerType triggerType) : base(card, turnTakerController)
        {
            exactlyOneTriggerType = triggerType;
        }

        TriggerType exactlyOneTriggerType = TriggerType.Other;
        protected abstract IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd);

        public override void AddTriggers()
        {
            //When this card is dealt exactly 1 damage, do something special
            AddTrigger((DealDamageAction dd) => dd.Target == Card && dd.Amount == 1, DealtExactlyOneDamageResponse, exactlyOneTriggerType, TriggerTiming.After);
            //When this card is dealt more than 1 damage, it deals each non-lion target 1 melee damage.
            AddTrigger((DealDamageAction dd) => dd.Target == Card && dd.Amount > 1, DealtMoreThanOneDamageResponse, TriggerType.DealDamage, TriggerTiming.After);

        }

        private IEnumerator DealtMoreThanOneDamageResponse(DealDamageAction dd)
        {
            //it deals each non-lion target 1 melee damage.
            return DealDamage(Card, c => !IsLion(c), 1, DamageType.Melee);
        }
    }
}