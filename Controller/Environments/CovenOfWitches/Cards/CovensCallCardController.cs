using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Studio29;

namespace Studio29.CovenOfWitches
{
    public class CovensCallCardController : CovenOfWitchesCardController
    {

        public CovensCallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }



        public override void AddTriggers()
        {
            // At the end of the Environment turn, destroy this card.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);

           
        }

        
        public override IEnumerator Play()
        {
            // When this card enters play, discover 2 witch cards
            IEnumerator coroutine = this.Discover(TurnTakerController, TurnTaker.Deck, new LinqCardCriteria((Card c) => IsWitch(c), "witch"), Game.H - 2, shuffleTrashIntoDeckFirst: true);
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