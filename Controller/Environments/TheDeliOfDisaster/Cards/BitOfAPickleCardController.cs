using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class BitOfAPickleCardController : TheDeliOfDisasterCardController
    {

        public BitOfAPickleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == base.Card || card.Location == base.Card.UnderLocation;
        }

        public override void AddTriggers()
        {
            //Listed win or loss conditions on cards are not in effect and may not be used to win or lose the game.
            AddTrigger((GameOverAction action) => action.EndingResult != EndingResult.HeroesDestroyedDefeat && action.EndingResult != EndingResult.VillainDestroyedVictory && action.EndingResult != EndingResult.EnvironmentDefeat, action => CancelActionProxy(action), TriggerType.CancelAction, TriggerTiming.Before);

            //At the start of the environment turn, one player may discard their hand to shuffle this card back into the environment deck, ignoring its indestructability.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, ShuffleBackIntoDeckResponse, TriggerType.ShuffleCardIntoDeck);
        }

        private IEnumerator CancelActionProxy(GameOverAction action)
        {
            IEnumerator coroutine = GameController.SendMessageAction($"But wait! {TurnTaker.Name} is still open for business... Play on!", Priority.Critical, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = CancelAction(action);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator ShuffleBackIntoDeckResponse(PhaseChangeAction pca)
        {
            List<SelectTurnTakerDecision> storedResultsHero = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = GameController.SelectHeroToDiscardTheirHand(DecisionMaker, optionalSelectHero: true, optionalDiscardCards: false, storedResultsTurnTaker: storedResultsHero, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (!DidDiscardHand(storedResultsHero))
            {
                yield break;
            }
            coroutine = GameController.MoveCard(TurnTakerController, Card, TurnTaker.Deck, evenIfIndestructible: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
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