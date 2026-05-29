using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.CovenOfWitches
{
    public class ArwenTheSchemingCardController : WitchCardController
    {

        public ArwenTheSchemingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "CurseOfArwen")
        {

        }

        public override void AddTriggers()
        {
            // At the start of the Environment turn, each player draws a card.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => EachPlayerDrawsACard(), TriggerType.DrawCard);

            // At the end of the Environment turn, this card deals the {H - 2} with the lowest HP X infernal damage, where X is number of hero cards discarded this turn.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction arg)
        {
            //this card deals the {H - 2} hero character cards with the lowest HP X infernal damage, where X is number of hero cards discarded this turn.

            int X = Game.Journal.DiscardCardEntriesThisTurn().Count(entry => entry.ResponsibleTurnTaker.IsHero);
            IEnumerator coroutine = DealDamageToLowestHP(Card, 1, c => c.IsHeroCharacterCard, c => X, DamageType.Infernal, numberOfTargets: Game.H - 2);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}