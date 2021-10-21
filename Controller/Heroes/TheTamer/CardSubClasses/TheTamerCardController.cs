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

        public static readonly string LionKeyword = "lion";

        protected bool IsLion(Card card)
        {
            return card.DoKeywordsContain(LionKeyword);
        }

        protected int GetNumberOfLionsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && this.IsLion(c)).Count();
        }

        protected IEnumerable<Card> FindLionsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && this.IsLion(c));
        }

    }
}