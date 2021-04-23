using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class AStoryOfFireCardController : LoreCardController
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

			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(damage1);
				yield return base.GameController.StartCoroutine(damage2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(damage1);
				base.GameController.ExhaustCoroutine(damage2);
			}
        }


    }
}