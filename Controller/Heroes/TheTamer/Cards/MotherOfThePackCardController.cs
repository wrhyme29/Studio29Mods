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

        private static readonly string MarkForRetaliationKey = "MarkForRetaliation";

        public override void AddTriggers()
        {
            //You may redirect any damage dealt to other lion cards to this card. If this card was dealt damage this way, this card deals the source of that damage 3 melee damage.
            AddTrigger((DealDamageAction dd) => IsLion(dd.Target) && dd.Target != Card && (dd.BattleZone == null || dd.BattleZone == Card.BattleZone), RedirectDamageResponse, new TriggerType[] { TriggerType.RedirectDamage, TriggerType.DealDamage }, TriggerTiming.Before);
            AddTrigger((DealDamageAction dd) => dd.DidDealDamage && dd.Target == Card && dd.DamageSource != null && dd.DamageSource.IsCard && GetCardPropertyJournalEntryBoolean(MarkForRetaliationKey) != null && GetCardPropertyJournalEntryBoolean(MarkForRetaliationKey).Value == true, RetaliationResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator RetaliationResponse(DealDamageAction dd)
        {
            //If this card was dealt damage this way, this card deals the source of that damage 3 melee damage.
            Card originalSourceOfDamage = dd.DamageSource.Card;
            SetCardProperty(MarkForRetaliationKey, false);
            IEnumerator coroutine = DealDamage(Card, originalSourceOfDamage, 3, DamageType.Melee, isCounterDamage: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dd)
        {
            //You may redirect any damage dealt to other lion cards to this card.
            SetCardPropertyToTrueIfRealAction(MarkForRetaliationKey);

            IEnumerator coroutine = GameController.RedirectDamage(dd, Card, isOptional: true, cardSource: GetCardSource());
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