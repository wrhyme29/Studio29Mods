using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class GiftReceiptCardController : BirthdayBoyCardController
    {

        public GiftReceiptCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Return 1 Present to the trash of its original owner.
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
            if(DidSelectCard(storedResults))
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

                Location destination = tt.Trash;
                if(tt.IsIncapacitatedOrOutOfGame)
                {
                    destination = tt.OutOfGame;
                }

                coroutine = GameController.MoveCard(TurnTakerController, selectedCard, destination, showMessage: true, cardSource: GetCardSource());
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
}