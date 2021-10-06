using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Debugger
{
    public class OptionsCardController : CardController
    {

        public OptionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected Card TheMenu => TurnTaker.FindCard("DebuggerInstructions", realCardsOnly: false);


        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(TheMenu.UnderLocation);
        }


    }
}