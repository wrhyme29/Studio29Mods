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
            //At the start of the environment turn, discard the top card of each deck.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnResponse, TriggerType.DiscardCard);

            //At the end of the environment turn, shuffle each trash into its deck.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, TriggerType.ShuffleTrashIntoDeck);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //need to adjust criteria to pull from only visible decks for OA correctly and aeon men scion deck when appropriate
            return GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame, ""), SelectionType.ShuffleTrashIntoDeck, (TurnTaker tt) => GameController.ShuffleTrashIntoDeck(GameController.FindTurnTakerController(tt), cardSource: GetCardSource()), allowAutoDecide: true, cardSource: GetCardSource());
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            //need to adjust criteria to pull from only visible decks for OA correctly and aeon men scion deck when appropriate
            return GameController.DiscardTopCardsOfDecks(DecisionMaker, (Location l) => !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, 1, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
        }
    }
}