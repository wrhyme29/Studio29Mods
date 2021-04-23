using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class RallyingCryCardController : LoreCardController
    {

        public RallyingCryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override IEnumerator Play()
        {
            //If there is an action card in play, increase damage dealt by hero targets by 1 until the start of your next turn.            
            bool actionCardsInPlay = FindCardsWhere(c => c.IsInPlayAndHasGameText && IsAction(c)).Any();
            IEnumerator coroutine;
            if (actionCardsInPlay)
            {
                IncreaseDamageStatusEffect effect = new IncreaseDamageStatusEffect(1);
                effect.SourceCriteria.IsHero = true;
                effect.SourceCriteria.IsTarget = true;
                effect.UntilStartOfNextTurn(TurnTaker);
                coroutine = AddStatusEffect(effect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                //If there is not an action card in play, draw a card.
                coroutine = DrawCard();
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
}