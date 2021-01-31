using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using Handelabra;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}