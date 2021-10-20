using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.BirthdayBoy
{
    public class BirthdayBoyUtilityCharacterCardController : HeroCharacterCardController
	{
		public BirthdayBoyUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		
		protected TurnTaker GetOriginalOwner(Card c)
        {
			return (FindTurnTakersWhere((TurnTaker tt) => tt.Identifier == c.ParentDeck.Identifier)).FirstOrDefault();
        }

		protected bool IsPresent(Card c)
        {
			bool result = GameController.DoesCardContainKeyword(c, "present");
			return result;
		}

		protected IEnumerable<Card> FindBirthdayBoysPresentsInPlay()
        {
			return FindCardsWhere(c => c.IsInPlayAndHasGameText && IsPresent(c) && c.Owner == base.TurnTaker);

			
        }
		protected int NumberOfCardsBirthdayBoyOwns => TurnTaker.GetAllCards().Where(c => !c.IsOffToTheSide && !c.IsOutOfGame && !c.IsCharacter).Count();

	}
}
