using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Lore
{
    public class BlessingOfTheFifteenthStoneCardController : StoryCardController
    {

        public BlessingOfTheFifteenthStoneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, MythKeyword)
        {

        }

        public override void AddTriggers()
        {
            //At the end of your turn, select one target other than {Lore} with 5 or fewer hp. Until the start of your next turn, that target is indestructible.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, TriggerType.CreateStatusEffect);

        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //select one target other than {Lore} with 5 or fewer hp. 

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.Custom, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget && c.HitPoints <= 5 && c != CharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Until the start of your next turn, that target is indestructible.
            if(!DidSelectCard(storedResults))
            {
                yield break;
            }

            Card selectedTarget = GetSelectedCard(storedResults);

            MakeIndestructibleStatusEffect effect = new MakeIndestructibleStatusEffect();
            effect.CardsToMakeIndestructible.IsSpecificCard = selectedTarget;
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Select a target to make indestructible until the start of your next turn.", "Select a target to make indestructible until the start of their next turn.", "Vote for which target to make indestructible until the start of the their next turn?", "target to become indestructible");

        }
    }
}