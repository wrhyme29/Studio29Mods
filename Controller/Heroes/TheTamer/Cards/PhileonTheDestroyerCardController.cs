using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class PhileonTheDestroyerCardController : LionCardController
    {

        public PhileonTheDestroyerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DealDamage)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //it deals 2 non-hero targets 2 melee damage each.
            return GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, Card), 2, DamageType.Melee, 2, false, , additionalCriteria: (Card c) => !c.IsHero && c.IsTarget, cardSource: GetCardSource());
        }
    }
}