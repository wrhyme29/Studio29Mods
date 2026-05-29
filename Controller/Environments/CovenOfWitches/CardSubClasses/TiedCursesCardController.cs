using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.CovenOfWitches
{
    public class TiedCursesCardController : CovenOfWitchesCardController
    {
        public string TiedCurseIdentifier { get; set; }

        public TiedCursesCardController(Card card, TurnTakerController turnTakerController, string tiedCurseIdentifier) : base(card, turnTakerController)
        {
            TiedCurseIdentifier = tiedCurseIdentifier;
        }

        public override bool ShouldBeDestroyedNow()
        {
            return Card.IsInPlayAndHasGameText && FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == TiedCurseIdentifier).Any();
        }

        public override void AddTriggers()
        {
            // If tied card is ever in play, destroy it or destroy this card.
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay.Identifier == TiedCurseIdentifier, DestroyTiedCurseResponse, TriggerType.DestroyCard, TriggerTiming.After);

        }

        private IEnumerator DestroyTiedCurseResponse(CardEntersPlayAction cep)
        {
            return GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c == Card || c.Identifier == TiedCurseIdentifier), false, cardSource: GetCardSource());
        }


    }
}