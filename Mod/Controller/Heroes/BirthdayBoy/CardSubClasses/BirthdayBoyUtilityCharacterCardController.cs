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
		

		protected IEnumerable<Card> FindBirthdayBoysPresentsInPlay()
        {
			return FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsPresent() && c.Owner == TurnTaker);

			
        }
		protected int NumberOfCardsBirthdayBoyOwns => TurnTaker.GetAllCards().Where(c => !c.IsOffToTheSide && !c.IsOutOfGame && !c.IsCharacter).Count();

	}
}
