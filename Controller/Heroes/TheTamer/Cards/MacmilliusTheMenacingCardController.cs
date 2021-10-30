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
            //destroy 1 non-hero ongoing card.
            return GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria((Card c) => !c.IsHero && c.IsOngoing, "non-hero ongoing"), false, cardSource: GetCardSource());
        }


    }
}