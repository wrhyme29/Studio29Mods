using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class GildeasTheGoodCardController : LionCardController
    {

        public GildeasTheGoodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DrawCard)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //draw a card.
            return DrawCard(HeroTurnTaker);
        }
    }
}