using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class MoritzTheMorbidCardController : LionCardController
    {

        public MoritzTheMorbidCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DestroyCard)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //destroy 1 non-hero target with 2 or fewer HP.
            return GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria((Card c) => !c.IsHero && c.IsTarget && c.HitPoints <= 2, "non-hero targets with 2 or fewer HP", useCardsSuffix: false), false, cardSource: GetCardSource());
        }


    }
}