using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class GrandMasterTamerCharacterCardController : TheTamerSubCharacterCardController
	{
		public GrandMasterTamerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			// Deal 1 Lion 1 sonic damage. If no damage is dealt this way, draw a card.
			int target = GetPowerNumeral(0, 1);
			int amount = GetPowerNumeral(1, 1);
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(GameController, base.Card), amount, DamageType.Sonic, new int?(target), false, new int?(target), additionalCriteria: (Card c) => IsLion(c), storedResultsDamage: storedResults, cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			if(!DidDealDamage(storedResults))
            {
				coroutine = DrawCard();
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
			switch (index)
			{
				case 0:
					{
						
						yield break;
					}
				case 1:
					{
						
						yield break;
					}
				case 2:
					{
						
						yield break;
					}
			}
			yield break;
		}
	}
}
