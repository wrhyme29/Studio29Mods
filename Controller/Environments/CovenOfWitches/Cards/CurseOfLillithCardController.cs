using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Studio29.CovenOfWitches
{
    public class CurseOfLillithCardController : TiedCursesCardController
    {

        public CurseOfLillithCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "CurseOfArwen")
        {
            SpecialStringMaker.ShowSpecialString(() => $"{nonActiveHeroTurnTakers} cannot deal damage this turn.", showInEffectsList: () => true).Condition = () => Card.IsInPlayAndHasGameText;
        }

        
        private string nonActiveHeroTurnTakers => Game.HeroTurnTakers.Where(htt => !htt.IsIncapacitatedOrOutOfGame && !htt.IsActiveTurnTaker).Select(htt => htt.NameRespectingVariant).ToCommaList(useWordAnd: true);

        public override void AddTriggers()
        {
            // At the start of the Environment turn, each Player may discard 1 card each to destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => EachPlayerMayDiscardOneCardToPerformAction(p, DestroyThisCardResponse(p), "destroy " + Card.Title), new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DestroySelf
            });

            // Hero characters may only deal damage on their own turn.
            AddCannotDealDamageTrigger((Card c) => c.IsHeroCharacterCard && c.Owner != Game.ActiveTurnTaker);

            // Add TiedCurses triggers
            base.AddTriggers();
        }


    }
}