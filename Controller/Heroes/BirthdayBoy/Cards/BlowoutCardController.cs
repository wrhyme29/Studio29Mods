using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class BlowoutCardController : BirthdayBoyCardController
    {

        public BlowoutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override bool DoNotMoveOneShotToTrash
        {
            get
            {
                return true;
            }
        }

        public override IEnumerator Play()
        {
            //Select a target with a max hp of 5 or fewer that was not in play at the start of the game. Remove that target from the game.

            IEnumerable<Card> choices = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c.HitPoints.HasValue && c.HitPoints.Value <= 5 && WasNotInPlayAtTheStartOfTheGame(c), "target with 5 or fewer HP that were not in play at the start of the game", useCardsSuffix: false, useCardsPrefix: false, null, "targets with 5 or fewer HP that were not in play at the start of the game")) ;
            SelectCardDecision selectCardDecision = new SelectCardDecision(GameController, HeroTurnTakerController, SelectionType.RemoveCardFromGame, choices, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardAndDoAction(selectCardDecision, (SelectCardDecision scd) => GameController.MoveCard(TurnTakerController, scd.SelectedCard, scd.SelectedCard.Owner.OutOfGame, showMessage: true, cardSource: GetCardSource()));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Remove this card and one Present in play from the game.
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.RemoveCardFromGame, GetPresentsInPlay(), storedResults, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            List<Card> cardsToRemove = new List<Card>();
            cardsToRemove.Add(Card);
            if(DidSelectCard(storedResults))
            {
                cardsToRemove.Insert(0, GetSelectedCard(storedResults));
            }
            coroutine = GameController.MoveCards(TurnTakerController, cardsToRemove, (Card c) => new MoveCardDestination(TurnTaker.OutOfGame), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;

        }

        private bool WasNotInPlayAtTheStartOfTheGame(Card c)
        {
            return (from e in Game.Journal.CardEntersPlayEntries()
                    where e.Card == c && e.Round > 0
                    select e).Any();
        }


    }
}