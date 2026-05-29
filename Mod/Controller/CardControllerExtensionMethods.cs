using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;

namespace Studio29
{
    public static class CardControllerExtensionMethods
    {
        public static TurnTaker GetOriginalOwner(this CardController cardController, Card card)
        {
            return (cardController.GameController.FindTurnTakersWhere((TurnTaker tt) => tt.Identifier == card.ParentDeck.Identifier)).FirstOrDefault();
        }
    }
}
