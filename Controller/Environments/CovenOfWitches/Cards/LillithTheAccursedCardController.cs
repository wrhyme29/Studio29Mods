using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.CovenOfWitches
{
    public class LillithTheAccursedCardController : WitchCardController
    {

        public LillithTheAccursedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "CurseOfLillith")
        {

        }

        public override void AddTriggers()
        {
            // At the start of the Environment turn, each player discards 1 card.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.EachPlayerDiscardsCards(1, 1, cardSource: GetCardSource()), TriggerType.DiscardCard);

            // At the end of the Environment turn, play the top card of the Environment deck.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse, TriggerType.PlayCard);
        }
       
    }
}