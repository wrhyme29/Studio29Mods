using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class GrandMasterTamerCharacterCardController : HeroCharacterCardController
    {
		public GrandMasterTamerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
            // {TheTamer} deals 1 sonic damage to a target in the {TheTamer}'s play area.
            int sonicDamageAmount = GetPowerNumeral(0, 1);
			int numCardsToDraw = GetPowerNumeral(1, 2);
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(
				HeroTurnTakerController, 
				new DamageSource(GameController, Card),
				sonicDamageAmount, 
				DamageType.Sonic, 
				numberOfTargets: 1,
				optional: false,
				requiredTargets: 1,
				additionalCriteria: (Card c) => c.Location.IsPlayAreaOf(TurnTaker), 
				storedResultsDamage: storedResults, 
				cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

            // If {TheTamer} is dealt damage this way, draw 2 cards.
            if (DidDealDamage(storedResults, toSpecificTarget: Card))
            {
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

			yield break;
		}

		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			yield return this.StandardPowerPlayDrawIncapacitatedAbility(index);
        }
	}
}
