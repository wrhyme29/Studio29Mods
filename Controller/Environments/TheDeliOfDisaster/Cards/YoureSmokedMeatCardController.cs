using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class YoureSmokedMeatCardController : TheDeliOfDisasterCardController
    {

        public YoureSmokedMeatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase all fire damage by 2.
            AddIncreaseDamageTrigger(dd => dd.DamageType == DamageType.Fire, 2);

            //At the end of the environment turn, this card deals each target 1 fire damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c.IsTarget, TargetType.All, 1, DamageType.Fire);
        }

    }
}