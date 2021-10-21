using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.TheTamer
{
	public class TheTamerCharacterCardController : TheTamerSubCharacterCardController
	{
		public TheTamerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Until the start of your next turn, whenever a target would deal exactly 1 damage to { TheTamer}, he may redirect that target to a lion in play.
			int originalDamage = GetPowerNumeral(0, 1);

			RedirectDamageStatusEffect effect = new RedirectDamageStatusEffect();
			effect.UntilStartOfNextTurn(TurnTaker);
			effect.TargetCriteria.IsSpecificCard = Card;
			effect.DamageAmountCriteria.EqualTo = 1;
			effect.RedirectableTargets.HasAnyOfTheseKeywords = new List<string>() { LionKeyword };
			effect.IsOptional = true;

			IEnumerator coroutine = AddStatusEffect(effect);
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
						//One non-character hero target deals 1 target 2 radiant damage.
						IEnumerable<Card> choices = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsHero && !c.IsCharacter);
						List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
						IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsHero && !c.IsCharacter, "non-character hero target"), storedResults, false, cardSource: GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						if (DidSelectCard(storedResults))
						{
							Card source = GetSelectedCard(storedResults);
							coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, source), 2, DamageType.Radiant, new int?(1), false, new int?(1), cardSource: GetCardSource());
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}
						}

						break;
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
