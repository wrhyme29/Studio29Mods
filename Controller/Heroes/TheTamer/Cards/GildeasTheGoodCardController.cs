using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class GildeasTheGoodCardController : TheTamerCardController
    {

        public GildeasTheGoodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override void AddTriggers()
        {
            //When this card is dealt damage, it deals TheTamer 1 melee damage. Then, draw a card.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == base.Card && dd.DidDealDamage, DealDamageAndDrawCardResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DrawCard
            }, TriggerTiming.After);
        }

        private IEnumerator DealDamageAndDrawCardResponse(DealDamageAction dd)
        {
            //it deals TheTamer 1 melee damage
            IEnumerator coroutine = DealDamage(base.Card, base.CharacterCard, 1, DamageType.Melee, isCounterDamage: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //Then, draw a card
            coroutine = DrawCard();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}