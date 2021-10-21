using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.TheTamer
{
    public class KittyGlovesCardController : TheTamerCardController
    {

        public KittyGlovesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override void AddTriggers()
		{
			//Whenever a Lion enters play, reduce damage dealt to that Lion by 1 until the start of your next turn.
			AddTargetEntersPlayTrigger((Card c) => IsLion(c), ReduceDamageResponse, TriggerType.CreateStatusEffect, TriggerTiming.After);
		}

		private IEnumerator ReduceDamageResponse(Card target)
		{
			//Reduce damage dealt to that Lion by 1 until the start of your next turn.
			ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
			reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = target;
			reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
			reduceDamageStatusEffect.UntilCardLeavesPlay(target);
			IEnumerator coroutine = AddStatusEffect(reduceDamageStatusEffect);
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