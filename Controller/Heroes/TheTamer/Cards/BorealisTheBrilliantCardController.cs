using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.TheTamer
{
    public class BorealisTheBrilliantCardController : LionCardController
    {

        public BorealisTheBrilliantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController,TriggerType.GainHP)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //each other lion gains 2 HP.
            return GameController.GainHP(HeroTurnTakerController, c => IsLion(c) && c != Card, 2, cardSource: GetCardSource());
        }

    }
}