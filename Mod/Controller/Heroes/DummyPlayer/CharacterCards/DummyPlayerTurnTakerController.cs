using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.DummyPlayer
{
    public class DummyPlayerTurnTakerController : HeroTurnTakerController
    {
        public DummyPlayerTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

      
        public override IEnumerator StartGame()
        {
          
            IEnumerator coroutine = GameController.FlipCard(CharacterCardController, cardSource: CharacterCardController.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}