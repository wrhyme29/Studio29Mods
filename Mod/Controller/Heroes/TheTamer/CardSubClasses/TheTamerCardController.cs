using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class TheTamerCardController : CardController
    {

        public TheTamerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected int GetNumberOfLionsInPlay()
        {
            return FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsLion()).Count();
        }

        protected IEnumerable<Card> FindLionsInPlay()
        {
            return FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsLion());
        }

    }
}