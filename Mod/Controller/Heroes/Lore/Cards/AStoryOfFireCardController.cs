using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class AStoryOfFireCardController : CardController
    {

        public AStoryOfFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
			//{Lore} deals one target 2 fire damage. Lore deals a second target 2 fire damage
			List<SelectCardDecision> targets = new List<SelectCardDecision>();
			IEnumerator damage1 = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Fire, 1, false, 1, storedResultsDecisions: targets, cardSource: GetCardSource());
			IEnumerator damage2 = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Fire, 1, false, 1, additionalCriteria: (Card card) => !targets.Select((SelectCardDecision d) => d.SelectedCard).Contains(card), cardSource: GetCardSource());

			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(damage1);
				yield return GameController.StartCoroutine(damage2);
			}
			else
			{
				GameController.ExhaustCoroutine(damage1);
				GameController.ExhaustCoroutine(damage2);
			}
        }


    }
}