
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

        //Find Presents in play

        protected IEnumerable<Card> GetPresentsInPlay()
        {
            return FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsPresent());
        }

        protected IEnumerable<Card> GetAllPresents()
        {
            return FindCardsWhere(c => !c.IsOffToTheSide && !c.IsOutOfGame && c.IsPresent());
        }

        protected int NumberOfCardsBirthdayBoyOwns => TurnTaker.GetAllCards().Where(c => !c.IsOffToTheSide && !c.IsOutOfGame && !c.IsCharacter).Count();




    }
}