using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.TestPlayer
{
	public class TestPlayerCharacterCardController : TestPlayerUtilityCharacterCardController
	{
		public TestPlayerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public readonly string DamageBeingRedirectedKey = "DamageBeingRedirected";
		public readonly string PowerNumeralForHPGainKey = "PowerNumeralForHPGain";

        public override void AddTriggers()
        {
			AddTrigger((DealDamageAction dd) => dd.Target == Card && dd.DidDealDamage && GetCardPropertyJournalEntryBoolean(DamageBeingRedirectedKey).HasValueWhere((val) => val == true), GainHPResponse, TriggerType.GainHP, TriggerTiming.After);
        }

        private IEnumerator GainHPResponse(DealDamageAction dd)
        {
			int? hpGainAmount = GetCardPropertyJournalEntryInteger(PowerNumeralForHPGainKey);
			IEnumerator coroutine = GameController.GainHP(Card, hpGainAmount, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			Game.Journal.RecordCardProperties(Card, DamageBeingRedirectedKey, (bool?)null);
			Game.Journal.RecordCardProperties(Card, PowerNumeralForHPGainKey, (int?) null);

			yield break;
		}

        public override IEnumerator UsePower(int index = 0)
		{
            int powerNumeral = GetPowerNumeral(0, 2);
			int[] powerNumerals = new int[1]
			{
			powerNumeral
			};
			OnDealDamageStatusEffect redirect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(RedirectDamageToMe), "The next time another hero would be dealt damage, redirect it to {ImpulseCharacter}. If {ImpulseCharacter} takes damage this way, he regains 2 HP.", new TriggerType[] { TriggerType.RedirectDamage, TriggerType.GainHP }, TurnTaker, Card, powerNumerals);
			redirect.CardFlippedExpiryCriteria.Card = base.Card;
			redirect.TargetCriteria.IsHeroCharacterCard = true;
			redirect.TargetCriteria.IsNotSpecificCard = base.Card;
			redirect.NumberOfUses = 1;
			IEnumerator redirectCoroutine = AddStatusEffect(redirect);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(redirectCoroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(redirectCoroutine);
			}
			yield break;
		}

        
		public IEnumerator RedirectDamageToMe(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
		{
			int? num = null;
			if (powerNumerals != null)
			{
				num = powerNumerals.ElementAtOrDefault(0);
			}
			if (!num.HasValue)
			{
				num = 2;
			}

			if(dd.IsRedirectable)
            {
				IEnumerator coroutine = GameController.RedirectDamage(dd, Card, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				SetCardPropertyToTrueIfRealAction(DamageBeingRedirectedKey);
				Game.Journal.RecordCardProperties(Card, PowerNumeralForHPGainKey, num);
			}


			yield break;
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
