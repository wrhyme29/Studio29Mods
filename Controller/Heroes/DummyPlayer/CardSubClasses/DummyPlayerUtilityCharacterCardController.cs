using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.DummyPlayer
{
	public class DummyPlayerUtilityCharacterCardController : HeroCharacterCardController
	{
		public DummyPlayerUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

        public override void AddTriggers()
        {
			AddTrigger((GameAction ga) => Game.HasGameStarted && !Card.IsFlipped, FlipThisCharacterCardResponse, TriggerType.FlipCard, TriggerTiming.Before);
        }



	}
}
