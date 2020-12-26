using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class TapOutCardController : TheTamerCardController
    {

        public TapOutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsLion(c), "lion"));
        }

        public override IEnumerator Play()
        {
            //Destroy all Lions in play. {TheTamer} regains 1 HP for each Lion destroyed this way.
            IEnumerator coroutine = DestroyCardsAndDoActionBasedOnNumberOfCardsDestroyed(DecisionMaker, new LinqCardCriteria((Card c) => IsLion(c), "lion"), GainHPResponse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

			yield break;
        }

		private IEnumerator GainHPResponse(int X)
		{
            //{TheTamer} regains 1 HP for each Lion destroyed this way.
            IEnumerator coroutine = GameController.GainHP(base.CharacterCard, new int?(X), cardSource: GetCardSource());
			
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