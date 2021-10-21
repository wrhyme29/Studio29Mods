using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Studio29.TheTamer
{
    public class ChairOfConfusionCardController : TheTamerCardController
    {

        public ChairOfConfusionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Whenever a lion would be dealt damage, reduce that damage to 1.
            AddReduceDamageToSetAmountTrigger(dd => IsLion(dd.Target), 1);
        }


    }
}