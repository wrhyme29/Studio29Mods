using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.TheTamer
{
    public class MacmilliusTheMenacingCardController : LionCardController
    {

        public MacmilliusTheMenacingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DestroyCard)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //destroy 1 non-hero target with 2 or fewer HP.
            return GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria((Card c) => !c.IsHero && c.IsTarget && c.HitPoints <= 2, "non-hero targets with 2 or fewer HP", useCardsSuffix: false), false, cardSource: GetCardSource());
        }


    }
}