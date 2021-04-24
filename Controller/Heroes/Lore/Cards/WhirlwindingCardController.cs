using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class WhirlwindingCardController : StoryCardController
    {

        public WhirlwindingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, RomanceKeyword)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {

			int hpGain = GetPowerNumeral(0, 1);
			int damage = GetPowerNumeral(1, 2);
			int[] powerNumerals = new int[]
			{
				hpGain,
				damage
			};

			//Select one hero character card. Until the start of your next turn, whenever that hero is dealt damage, Lore regains 1 hp and deals one target 2 psychic damage.
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, "active hero character cards in play", useCardsSuffix: false), storedResults, optional: false, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (!DidSelectCard(storedResults))
			{
				yield break;
			}
			Card selectedCard = GetSelectedCard(storedResults);
			OnDealDamageStatusEffect onDealDamageStatusEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(DealDamageAndGainHPResponse), "Whenever " + selectedCard.AlternateTitleOrTitle + " is dealt damage, " + CharacterCard.AlternateTitleOrTitle + " regains 1 hp and deals one target 2 psychic damage.", new TriggerType[]
					{
						TriggerType.GainHP,
						TriggerType.DealDamage
					},TurnTaker, Card, powerNumerals);
			onDealDamageStatusEffect.BeforeOrAfter = BeforeOrAfter.After;
			onDealDamageStatusEffect.TargetCriteria.IsSpecificCard = selectedCard;
			onDealDamageStatusEffect.UntilStartOfNextTurn(TurnTaker);
			onDealDamageStatusEffect.UntilTargetLeavesPlay(selectedCard);
			onDealDamageStatusEffect.DoesDealDamage = true;
			coroutine = AddStatusEffect(onDealDamageStatusEffect);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
        }

        public IEnumerator DealDamageAndGainHPResponse(DealDamageAction dd, HeroTurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
			if(!dd.DidDealDamage)
            {
				yield break;
            }

			int? hpGain = null;
			int? damage = null;
			if (powerNumerals != null)
			{
				hpGain = powerNumerals.ElementAtOrDefault(0);
				damage = powerNumerals.ElementAtOrDefault(1);
			}
			if (!hpGain.HasValue)
			{
				hpGain = 1;
			}

			if(!damage.HasValue)
            {
				damage = 2;
            }

			//Lore regains 1 hp and deals one target 2 psychic damage
			IEnumerator coroutine = GameController.GainHP(CharacterCard, hpGain, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), damage.Value, DamageType.Psychic, 1, false, 1, cardSource: GetCardSource());
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

    }
}