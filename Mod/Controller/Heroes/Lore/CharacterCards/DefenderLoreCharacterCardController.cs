using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class DefenderLoreCharacterCardController : HeroCharacterCardController
    {
		public DefenderLoreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//{Lore} deals 1 target X projectile damage, where x = the number of your story cards in play plus 1.

			int powerNumeral = GetPowerNumeral(0, 1);
			int powerNumeral2 = GetPowerNumeral(1, 1);
			IEnumerable<Card> source = FindCardsWhere((Card c) => c.IsStory() && c.IsInPlay && c.Owner == TurnTaker);
			IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), source.Count() + powerNumeral2, DamageType.Projectile, powerNumeral, optional: false, powerNumeral,cardSource: GetCardSource());
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
			
		}


		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{

						break;
					}
				case 1:
					{
						
						break;
					}
				case 2:
					{
						
						break;
					}
			}
			yield break;
		}

	}
}
