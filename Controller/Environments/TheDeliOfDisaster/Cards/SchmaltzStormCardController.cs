using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Studio29.TheDeliOfDisaster
{
    public class SchmaltzStormCardController : TheDeliOfDisasterCardController
    {

        public SchmaltzStormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce all damage by 1.
            AddReduceDamageTrigger(dd => true, 1);

            //Reduce all hp recovery by 1.

            AddTrigger((GainHPAction g) => true, (GainHPAction g) => GameController.ReduceHPGain(g, 1, GetCardSource()), new TriggerType[]
            {
                TriggerType.ReduceHPGain,
                TriggerType.ModifyHPGain
            }, TriggerTiming.Before);

            //At the start of the environment turn, each player may discard 1 card to destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction p) => EachPlayerMayDiscardOneCardToPerformAction(p, GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource()), "destroy " + Card.Title), new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DestroySelf
            });
        }


    }
}