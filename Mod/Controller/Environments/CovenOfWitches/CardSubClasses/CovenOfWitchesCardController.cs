using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.CovenOfWitches
{
    public class CovenOfWitchesCardController : CardController
    {

        public CovenOfWitchesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public readonly string CurseKeyword = "curse";
        public readonly string WitchKeyword = "witch";
        protected bool IsCurse(Card card)
        {
            return card != null && card.DoKeywordsContain(CurseKeyword);
        }
        protected bool IsWitch(Card card)
        {
            return card != null && card.DoKeywordsContain(WitchKeyword);
        }


    }
}