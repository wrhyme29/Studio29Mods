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
            //Return 1 present to the play area of its original owner. 
            IEnumerable<Card> presentsList = GetAllPresents();

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.MoveCard, presentsList, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidSelectCard(storedResults))
            {
                Card selectedCard = GetSelectedCard(storedResults);
                TurnTaker tt = GetOriginalOwner(selectedCard);
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

                coroutine = GameController.MoveCard(TurnTakerController, selectedCard, destination,playCardIfMovingToPlayArea: !selectedCard.Location.IsPlayArea, showMessage: true, cardSource: GetCardSource());;
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }
            //You may move one card from your trash to your hand.
            coroutine = base.GameController.SelectCardFromLocationAndMoveIt(HeroTurnTakerController, TurnTaker.Trash, new LinqCardCriteria((Card c) => true), new MoveCardDestination[1]
            {
                new MoveCardDestination(HeroTurnTaker.Hand)
            }, optional: true, cardSource: GetCardSource());
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