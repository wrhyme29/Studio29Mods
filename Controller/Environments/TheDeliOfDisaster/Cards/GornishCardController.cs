using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class GornishCardController : TheDeliOfDisasterCardController
    {

        public GornishCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of a hero's turn, that hero discards 2 cards from their hand.
            AddStartOfTurnTrigger(tt => tt.IsHero && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()), StartOfTurnResponse, TriggerType.DiscardCard);

            //At the end of a hero's turn, that hero draws 2 cards.
            AddEndOfTurnTrigger(tt => tt.IsHero && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()), EndOfTurnResponse, TriggerType.ShuffleTrashIntoDeck);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            TurnTaker tt = pca.ToPhase.TurnTaker;
            HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
            return GameController.SelectAndDiscardCards(httc, 2, false, 2, cardSource: GetCardSource());
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            TurnTaker tt = pca.ToPhase.TurnTaker;
            HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
            return DrawCards(httc, 2);
        }
    }
}