using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Studio29;

namespace Studio29.CovenOfWitches
{
    public class TrippedWardsCardController : CovenOfWitchesCardController
    {

        public TrippedWardsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCards(cursesInPlayCriteria).Condition = () => Card.IsInPlayAndHasGameText;
        }

        private LinqCardCriteria cursesInPlayCriteria => new LinqCardCriteria(c => IsCurse(c) && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
        private int numCursesInPlay => FindCardsWhere(cursesInPlayCriteria).Count();


        public override void AddTriggers()
        {
            // At the start of the Environment turn, destroy this card.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);

            // When this card is destroyed, this card deals all non-environment targets X infernal damage, where X is the number of curses in play.
            AddWhenDestroyedTrigger((DestroyCardAction dca) => DealDamage(Card, c => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), numCursesInPlay, DamageType.Infernal), TriggerType.DealDamage);
        }

        
        public override IEnumerator Play()
        {
            // When this card enters play, Discover {H - 2} curse cards
            IEnumerator coroutine = this.Discover(TurnTakerController, TurnTaker.Deck, new LinqCardCriteria((Card c) => IsCurse(c), "curse"), Game.H - 2);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

        }


    }
}