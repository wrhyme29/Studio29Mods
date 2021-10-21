using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Studio29.TheTamer
{
    public class NemeanCoatCardController : TheTamerCardController
    {

        public NemeanCoatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
			//Until the start of your next turn, whenever the {TheTamer} is dealt damage, he deals 1 lion X melee damage, where X is the amount of damage dealt to {TheTamer}.

			int powerNumeral = GetPowerNumeral(0, 1);
			int[] powerNumerals = new int[]
			{
				powerNumeral
			};
			OnDealDamageStatusEffect onDealDamageStatusEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(CounterDamageResponse), "Whenever " + Card.Title + "is dealt damage, he may deal that a lion X melee damage, where X is the amount of damage dealt to " + Card.Title + ".", new TriggerType[]
			{
			TriggerType.DealDamage
			}, TurnTaker, Card, powerNumerals);
			onDealDamageStatusEffect.TargetCriteria.IsSpecificCard = CharacterCard;
			onDealDamageStatusEffect.DamageAmountCriteria.GreaterThan = 0;
			onDealDamageStatusEffect.UntilStartOfNextTurn(TurnTaker);
			onDealDamageStatusEffect.UntilTargetLeavesPlay(CharacterCard);
			onDealDamageStatusEffect.BeforeOrAfter = BeforeOrAfter.After;
			onDealDamageStatusEffect.DoesDealDamage = true;
			IEnumerator coroutine = AddStatusEffect(onDealDamageStatusEffect);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
		}

		public IEnumerator CounterDamageResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
		{
			//he deals 1 lion X melee damage, where X is the amount of damage dealt to {TheTamer}
			int? num = null;
			if (powerNumerals != null)
			{
				num = powerNumerals.ElementAtOrDefault(0);
			}
			if (!num.HasValue)
			{
				num = 1;
			}
			int X = dd.Amount;

			IEnumerator coroutine = DealDamage(CharacterCard, c => IsLion(c), c => X, DamageType.Melee, dynamicNumberOfTargets: () => num.Value);
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