using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Lore
{
    public class RhetoricalBoostCardController : LoreCardController
    {

        public RhetoricalBoostCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt by {Lore} by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(CharacterCard), 1);

			//Whenever a story card enters play, draw a card
			AddTrigger((CardEntersPlayAction cep) => IsStory(cep.CardEnteringPlay) && cep.IsSuccessful, DrawCardResponse, TriggerType.DrawCard, TriggerTiming.After);

			//If this card has negative HP, reduce damage dealt to {Lore} by 1 for each HP below zero.
			AddReduceDamageTrigger((DealDamageAction dd) => Card.HitPoints.Value < 0 && dd.Target == CharacterCard, dd => -1 * Card.HitPoints.Value);
		}


		private IEnumerator DrawCardResponse(CardEntersPlayAction cep)
		{
			IEnumerator drawCard = DrawCard();
			string message = $"{Card.Title} allows {TurnTaker.Name} to draw a card.";
			IEnumerator coroutine = GameController.SendMessageAction(message, Priority.Medium, GetCardSource(), showCardSource: true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(drawCard);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(drawCard);
			}
		}

	}
}