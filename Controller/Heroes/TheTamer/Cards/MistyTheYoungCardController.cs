using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.TheTamer
{
    public class MistyTheYoungCardController : LionCardController
    {

        public MistyTheYoungCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DealDamage)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //it deals 2 other targets 1 melee damage each.
            return GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, Card), 1, DamageType.Melee, 2, false, 2, additionalCriteria: (Card c) => c != Card, cardSource: GetCardSource());
        }


    }
}