using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Debugger
{
    public class UseIncapCardController : OptionsCardController
	{

        public UseIncapCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override IEnumerator Play()
		{
			//Select a hero. They may use the incapacititated ability on a card in their play area now.

			List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.UseIncapacitatedAbility, false, false, storedResults, heroCriteria: new LinqTurnTakerCriteria(htt => htt.PlayArea.Cards.Any(c => c.Owner == htt && c.IsIncapacitated && c.IncapacitatedAbilities.Count() > 0)), allowIncapacitatedHeroes: true, cardSource: GetCardSource());
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
				coroutine = GameController.SelectIncapacitatedHeroAndUseAbility(httc);
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

	}
}