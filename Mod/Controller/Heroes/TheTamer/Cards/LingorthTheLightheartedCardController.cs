using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.TheTamer
{
    public class LingorthTheLightheartedCardController : LionCardController
    {

        public LingorthTheLightheartedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DealDamage)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //it deals one non-hero target 4 melee damage.
            return GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, Card), 4, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !c.IsHero && c.IsTarget, cardSource: GetCardSource());
        }
    }
}