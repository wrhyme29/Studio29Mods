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
            //Reduce damage dealt to The Tamer by Lions by 1.
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && IsLion(dd.DamageSource.Card) && dd.Target == base.CharacterCard, (DealDamageAction dd) => 1);
        }


    }
}