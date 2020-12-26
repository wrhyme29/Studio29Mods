using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class MotherOfThePackCardController : TheTamerCardController
    {

        public MotherOfThePackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Redirect all Lion damage dealt to {TheTamer} to this card.
            AddRedirectDamageTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.DamageSource != null && IsLion(dd.DamageSource.Card), () => base.Card);
        }
    }
}