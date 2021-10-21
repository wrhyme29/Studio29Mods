using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Studio29.TheTamer
{
    public class TheTamerSubCharacterCardController : HeroCharacterCardController
	{
		public TheTamerSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public static readonly string LionKeyword = "lion";
		protected bool IsLion(Card card)
		{
			return card.DoKeywordsContain(LionKeyword);
		}
	}
}
