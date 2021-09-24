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
			//Until the start of your next turn, the first time a Lion is destroyed, it deals 1 target 3 melee damage. You may return that Lion to your hand.
			int targets = base.GetPowerNumeral(0, 1);
			int amount = base.GetPowerNumeral(1, 3);

			int[] powerNumerals = new int[]
			{
				targets,
				amount
			};

			WhenCardIsDestroyedStatusEffect effect = new WhenCardIsDestroyedStatusEffect(base.Card, "DealDamageAndReturnToHand", "The first time a Lion is destroyed, it deals 1 target 3 melee damage. You may return that Lion to your hand.", new TriggerType[]
			{
				TriggerType.DealDamage,
				TriggerType.ChangePostDestroyDestination
			}, base.HeroTurnTaker, base.Card, powerNumerals);
			effect.UntilStartOfNextTurn(base.TurnTaker);
			effect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string>() { "lion" };
			effect.BeforeOrAfter = BeforeOrAfter.After;
			effect.NumberOfUses = 1;

			IEnumerator coroutine = base.AddStatusEffect(effect);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}

		public IEnumerator DealDamageAndReturnToHand(DestroyCardAction dca, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
		{
			int? targets = null;
			int? amount = null;
			if (powerNumerals != null)
			{
				targets = new int?(powerNumerals.ElementAtOrDefault(0));
				amount = new int?(powerNumerals.ElementAtOrDefault(1));
			}
			if (targets == null)
			{
				targets = new int?(1);
			}
			if (amount == null)
			{
				targets = new int?(3);
			}

			//It deals 1 target 3 melee damage. 
			Card destroyedLion = dca.CardToDestroy.Card;
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, destroyedLion), amount.Value, DamageType.Melee, targets, false, targets, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//You may return that Lion to your hand.

			List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
			coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.MoveCardToHand, dca.CardToDestroy.Card,storedResults: storedResults,cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (base.DidPlayerAnswerYes(storedResults))
			{
				dca.SetPostDestroyDestination(base.HeroTurnTaker.Hand, decisionSources: storedResults.CastEnumerable<YesNoCardDecision, IDecision>());
			}
			yield break;
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
