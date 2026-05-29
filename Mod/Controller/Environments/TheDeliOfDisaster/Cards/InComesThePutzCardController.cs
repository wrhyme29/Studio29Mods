using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class InComesThePutzCardController : DinerCardController
    {

        public InComesThePutzCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the villain target with the highest HP 4 sonic damage. If damage is taken this way, play the top card of the villain deck and restore this card to its max hp.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.PlayCard });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //this card deals the villain target with the highest HP 4 sonic damage.
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamageToHighestHP(Card, 1, c => IsVillainTarget(c), c => 4, DamageType.Sonic, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(!DidDealDamage(storedResults))
            {
                yield break;
            }

            //If damage is taken this way, play the top card of the villain deck
            coroutine = PlayTheTopCardOfTheVillainDeckWithMessageResponse(storedResults.First());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //and restore this card to its max hp.
            coroutine = GameController.SetHP(Card, Card.MaximumHitPoints.Value, cardSource: GetCardSource());
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