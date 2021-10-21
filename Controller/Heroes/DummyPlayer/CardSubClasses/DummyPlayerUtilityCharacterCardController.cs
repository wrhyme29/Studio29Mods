using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Studio29.DummyPlayer
{
    public class TestPlayerUtilityCharacterCardController : HeroCharacterCardController
	{
		public TestPlayerUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

        public override void AddTriggers()
        {
			AddTrigger((GameAction ga) => Game.HasGameStarted && !Card.IsFlipped, FlipThisCharacterCardResponse, TriggerType.FlipCard, TriggerTiming.Before);
        }



	}
}
