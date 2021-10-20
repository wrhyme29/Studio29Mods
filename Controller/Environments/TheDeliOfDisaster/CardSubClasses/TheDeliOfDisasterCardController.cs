using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Studio29.TheDeliOfDisaster
{
    public class TheDeliOfDisasterCardController : CardController
    {

        public TheDeliOfDisasterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string DishKeyword = "dish";

        protected bool IsDish(Card card)
        {
            return card != null && card.DoKeywordsContain(DishKeyword);
        }
    }
}