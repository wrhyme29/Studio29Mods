using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class DefenderLoreCharacterCardController : LoreSubCharacterCardController
	{
		public DefenderLoreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//{Lore} deals 1 target X projectile damage, where x = the number of your story cards in play plus 1.

			int powerNumeral = GetPowerNumeral(0, 1);
			int powerNumeral2 = GetPowerNumeral(1, 1);
			IEnumerable<Card> source = FindCardsWhere((Card c) => IsStory(c) && c.IsInPlay && c.Owner == base.TurnTaker);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.Card), source.Count() + powerNumeral2, DamageType.Projectile, powerNumeral, optional: false, powerNumeral,cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
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
