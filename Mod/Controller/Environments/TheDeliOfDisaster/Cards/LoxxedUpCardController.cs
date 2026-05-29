using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class LoxxedUpCardController : TheDeliOfDisasterCardController
    {

        public LoxxedUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowVillainTargetWithLowestHP();
            SpecialStringMaker.ShowVillainTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, the villain target with the lowest HP deals the hero target with the highest HP 2 toxic damage.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, DealDamageAtStartOfTurnResponse, TriggerType.DealDamage);

            //At the end of the environment turn, the hero target with the lowest HP deals the villain target with the highest HP 2 energy damage.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, DealDamageAtEndOfTurnResponse, TriggerType.DealDamage);

        }

        private IEnumerator DealDamageAtEndOfTurnResponse(PhaseChangeAction pca)
        {
            //the hero target with the lowest HP deals the villain target with the highest HP 2 energy damage.
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsHero, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card card = storedResults.FirstOrDefault();
            if (card is null)
            {
                yield break;
            }

            IEnumerator coroutine2 = DealDamageToHighestHP(card, 1, (Card c) => IsVillainTarget(c), (Card c) => 2, DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }

        private IEnumerator DealDamageAtStartOfTurnResponse(PhaseChangeAction pca)
        {
            //the villain target with the lowest HP deals the hero target with the highest HP 2 toxic damage.
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => IsVillainTarget(c), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card card = storedResults.FirstOrDefault();
            if (card is null)
            {
                yield break;
            }

            IEnumerator coroutine2 = DealDamageToHighestHP(card, 1, (Card c) => c.IsHero, (Card c) => 2, DamageType.Toxic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            
        }
    }
}