using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.TheTamer
{
    public class BeoninTheBraggertCardController : LionCardController
    {

        public BeoninTheBraggertCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DealDamage)
        {

        }

        protected override IEnumerator DealtExactlyOneDamageResponse(DealDamageAction dd)
        {
            //it deals each non-hero target 2 melee damage and 1 other hero target 2 melee damage.
            IEnumerator coroutine = DealDamage(Card, c => !c.IsHero && c.IsTarget, 2, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, Card), 2, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => c.IsHero && c.IsTarget && c != Card, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }


    }
}