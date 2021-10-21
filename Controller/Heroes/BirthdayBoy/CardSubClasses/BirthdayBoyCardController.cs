
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class BirthdayBoyCardController : CardController
    {

        public BirthdayBoyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string PresentKeyword = "present";

        //Find Presents in play

        protected bool IsPresent(Card card)
        {
            return GameController.DoesCardContainKeyword(card, PresentKeyword);
        }

        protected IEnumerable<Card> GetPresentsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && IsPresent(c));
        }

        protected IEnumerable<Card> GetAllPresents()
        {
            return base.FindCardsWhere(c => !c.IsOffToTheSide && !c.IsOutOfGame && IsPresent(c));
        }

        protected TurnTaker GetOriginalOwner(Card c)
        {
            return (FindTurnTakersWhere((TurnTaker tt) => tt.Identifier == c.ParentDeck.Identifier)).FirstOrDefault();
        }

        protected int NumberOfCardsBirthdayBoyOwns => TurnTaker.GetAllCards().Where(c => !c.IsOffToTheSide && !c.IsOutOfGame && !c.IsCharacter).Count();




    }
}