using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class ElementalWhipCardController : TheTamerCardController
    {

        public ElementalWhipCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Deal each Lion card in play 1 energy damage.
            IEnumerator coroutine = DealDamage(base.CharacterCard, (Card c) => IsLion(c) && c.IsInPlayAndHasGameText, 1, DamageType.Energy);
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