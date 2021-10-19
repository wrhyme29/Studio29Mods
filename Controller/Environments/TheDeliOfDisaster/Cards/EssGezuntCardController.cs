using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class EssGezuntCardController : TheDeliOfDisasterCardController
    {

        public EssGezuntCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, each target regains 2 HP.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.GainHP(DecisionMaker, c => c.IsTarget, 2, cardSource: GetCardSource()), TriggerType.GainHP);
        }

    }
}