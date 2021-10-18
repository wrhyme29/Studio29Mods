using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class ItsTheThoughtThatCountsCardController : BirthdayBoyCardController
    {

        public ItsTheThoughtThatCountsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Return any number of presents to the play area of its original owner. 
            IEnumerable<Card> presentsList = GetPresentsInPlay();

            List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
            IEnumerator coroutine = GameController.SelectCardsAndStoreResults(HeroTurnTakerController, SelectionType.MoveCard, c=> presentsList.Contains(c),numberOfCards: presentsList.Count(), storedResults: storedResults, optional: false, requiredDecisions: 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidSelectCards(storedResults))
            {
                TurnTaker tt;
                foreach(Card selectedCard in GetSelectedCards(storedResults))
                {
                    tt = GetOriginalOwner(selectedCard);
                    GameController.ChangeCardOwnership(selectedCard, tt);
                    coroutine = GameController.ModifyKeywords("present", addingOrRemoving: false, affectedCards: selectedCard.ToEnumerable().ToList(), cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    Location destination = tt.PlayArea;
                    if (tt.IsIncapacitatedOrOutOfGame)
                    {
                        destination = tt.OutOfGame;
                    }

                    coroutine = GameController.MoveCard(TurnTakerController, selectedCard, destination, playCardIfMovingToPlayArea: !selectedCard.Location.IsPlayArea, showMessage: true, cardSource: GetCardSource()); ;
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
            //You may move one card from your trash to your hand for each present moved this way.
            int X = GetNumberOfCardsSelected(storedResults);
            List<MoveCardDestination> destinations = new List<MoveCardDestination>()
            {
                new MoveCardDestination(HeroTurnTaker.Hand)
            };
            coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(HeroTurnTakerController, TurnTaker.Trash, 0, X, new LinqCardCriteria((Card c) => true), destinations, cardSource: GetCardSource());
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