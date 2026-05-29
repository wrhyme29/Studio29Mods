using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.Lore
{
    public class ReadingGlassesCardController : LoreCardController
    {
		public override bool DoesHaveActivePlayMethod => false;

		public ReadingGlassesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			GameController.AddCardControllerToList(CardControllerListType.IncreasePhaseActionCount, this);
		}

		public override void AddTriggers()
		{
			AddAdditionalPhaseActionTrigger((TurnTaker tt) => ShouldIncreasePhaseActionCount(tt), Phase.UsePower, 1);
		}

		public override IEnumerator Play()
		{
			IEnumerator coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == TurnTaker, Phase.UsePower, 1);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
		{
			return tt == TurnTaker;
		}

		public override bool AskIfIncreasingCurrentPhaseActionCount()
		{
			if (GameController.ActiveTurnPhase.IsUsePower)
			{
				return ShouldIncreasePhaseActionCount(GameController.ActiveTurnTaker);
			}
			return false;
		}

	}
}