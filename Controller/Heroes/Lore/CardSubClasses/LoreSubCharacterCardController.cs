using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
	public class LoreSubCharacterCardController : HeroCharacterCardController
	{
		public LoreSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public static readonly string StoryKeyword = "story";

		protected bool IsStory(Card card)
		{
			return card.DoKeywordsContain(StoryKeyword);
		}

	}
}
