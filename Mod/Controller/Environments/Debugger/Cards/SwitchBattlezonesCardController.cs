using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Debugger
{
    public class SwitchBattlezonesCardController : OptionsCardController
	{

        public SwitchBattlezonesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override IEnumerator Play()
		{
			//Select a Hero. Move them to the other battlezone

			List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
			IEnumerator coroutine = GameController.SelectTurnTaker(DecisionMaker, SelectionType.SwitchBattleZone, storedResults, additionalCriteria: tt => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame, ignoreBattleZone: true, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (DidSelectTurnTaker(storedResults))
			{
				HeroTurnTaker htt = GetSelectedTurnTaker(storedResults).ToHero();
				HeroTurnTakerController httc = FindHeroTurnTakerController(htt);
				coroutine = GameController.SwitchBattleZone(httc);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			coroutine = DestroyThisCardResponse(null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public override CustomDecisionText GetCustomDecisionText(IDecision decision)
		{
			return new CustomDecisionText($"Select a deck to play cards from",
											"They are selecting a deck to play cards from",
											"Vote for a deck to play cards from",
											"selecting a deck to play cards from");

		}


	}
}