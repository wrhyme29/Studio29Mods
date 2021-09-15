using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class ClosingTimeCardController : TheDeliOfDisasterCardController
    {

        public ClosingTimeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, each player may discard a card. Deal any character that does not discard a card 2 energy damage.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DealDamage });
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            //each character may discard a card.
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = this.GameController.EachPlayerDiscardsCards(0, 1, storedResults, cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            IEnumerable<TurnTaker> discardingHeroes = storedResults.Select(dca => dca.HeroTurnTakerController.TurnTaker);

            //This card deals any hero character that does not discard a card 2 energy damage.
            coroutine = DealDamage(Card, c => c.IsHeroCharacterCard && !discardingHeroes.Contains(c.Owner), 2, DamageType.Energy);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}