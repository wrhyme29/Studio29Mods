using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class MixerCardController : BirthdayBoyCardController
    {

        public MixerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		private int NumberOfPowerUsesForCustomDecision = 1;

        public override IEnumerator Play()
        {
            //You may destroy any number of presents.
            List<DestroyCardAction> storedDestroyResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCards(HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsPresent(), "present"), null,  requiredDecisions: 0, storedResultsAction: storedDestroyResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidDestroyCards(storedDestroyResults))
            {
                yield break;
            }

            //You may use {BirthdayBoy}'s innate power X + 1 times this turn, where X is the number of presents destroyed this way.
            int X = GetNumberOfCardsDestroyed(storedDestroyResults);
			NumberOfPowerUsesForCustomDecision = X + 1;
            if (GameController.ActiveTurnTaker == TurnTaker)
            {
                AllowSetNumberOfPowerUseStatusEffect allowSetNumberOfPowerUseStatusEffect = new AllowSetNumberOfPowerUseStatusEffect(X + 1);
                allowSetNumberOfPowerUseStatusEffect.UsePowerCriteria.IsSpecificCard = CharacterCard;
                allowSetNumberOfPowerUseStatusEffect.UsePowerCriteria.CardSource = CharacterCard;
                allowSetNumberOfPowerUseStatusEffect.UntilThisTurnIsOver(GameController.Game);
                allowSetNumberOfPowerUseStatusEffect.CardDestroyedExpiryCriteria.Card = CharacterCard;
                allowSetNumberOfPowerUseStatusEffect.NumberOfUses = 1;
                coroutine = AddStatusEffect(allowSetNumberOfPowerUseStatusEffect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            } 

			int timesUsed = (from e in Journal.UsePowerEntriesThisTurn()
							 where e.CardWithPower == CharacterCard
							 select e).Count();
			if (timesUsed < X + 1)
			{
				List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
				SelectionType type = SelectionType.Custom;
				if (timesUsed > 0)
				{
					type = SelectionType.UsePowerAgain;
				}
				IEnumerator coroutine2 = GameController.MakeYesNoCardDecision(HeroTurnTakerController, type, CharacterCard, storedResults: storedResults, cardSource: GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine2);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine2);
				}
				if (!DidPlayerAnswerYes(storedResults))
				{
					yield break;
				}
				for (int i = 0; i < X + 1 - timesUsed; i++)
				{
					coroutine2 = UsePowerOnOtherCard(CharacterCard);
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(coroutine2);
					}
					else
					{
						GameController.ExhaustCoroutine(coroutine2);
					}
				}
			}
			else
			{
				IEnumerator coroutine3 = GameController.SendMessageAction($"{TurnTaker.Name} has already used {CharacterCard.Definition.Body.First()} {X + 1} times this turn.", Priority.High, GetCardSource(), showCardSource: true);
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine3);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine3);
				}
			}

		}

		public override CustomDecisionText GetCustomDecisionText(IDecision decision)
		{

			return new CustomDecisionText($"Do you want to use Birthday Boy's innate power {NumberOfPowerUsesForCustomDecision} times?", $"Should they use Birthday Boy's innate power {NumberOfPowerUsesForCustomDecision} times?", $"Vote for if they should use Birthday Boy's innate  power {NumberOfPowerUsesForCustomDecision} times?", $"use Birthday Boy's innate power {NumberOfPowerUsesForCustomDecision} times");

		}


	}
}