using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Studio29.TheTamer
{
    public class MotherOfThePackCardController : TheTamerCardController
    {

        public MotherOfThePackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //You may redirect any damage dealt to other lion cards to this card. If this card was dealt damage this way, this card deals the source of that damage 3 melee damage.
            AddTrigger((DealDamageAction dd) => IsLion(dd.Target) && dd.Target != Card && (dd.BattleZone == null || dd.BattleZone == Card.BattleZone), RedirectDamageAndRetaliateResponse, new TriggerType[] { TriggerType.RedirectDamage, TriggerType.DealDamage }, TriggerTiming.Before);
        }

        private IEnumerator RedirectDamageAndRetaliateResponse(DealDamageAction dd)
        {
            //You may redirect any damage dealt to other lion cards to this card. 
            IEnumerator coroutine = GameController.RedirectDamage(dd, Card, isOptional: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //If this card was dealt damage this way...
            if (!dd.DidDealDamage || dd.DamageSource is null || !dd.DamageSource.IsCard )
            {
                yield break;
            }

            //this card deals the source of that damage 3 melee damage.
            Card originalDamageSource = dd.DamageSource.Card;
            coroutine = DealDamage(Card, originalDamageSource, 3, DamageType.Melee, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}