using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(base.CharacterCard, (Card c) => IsLion(c) && c.IsInPlayAndHasGameText, 1, DamageType.Energy, storedResults: storedResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //If no lions were dealt damage this way, {Tamer} deals himself 2 energy damage and draws 2 cards.
            if(DidDealDamage(storedResults) && storedResults.Any(dd => IsLion(dd.Target) && dd.DidDealDamage))
            {
                yield break;
            }

            coroutine = DealDamage(CharacterCard, CharacterCard, 2, DamageType.Energy, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DrawCards(HeroTurnTakerController, 2);
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