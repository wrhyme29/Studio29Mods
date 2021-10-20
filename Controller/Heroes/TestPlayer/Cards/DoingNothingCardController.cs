using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TestPlayer
{
    public class DoingNothingCardController : CardController
    {

        public DoingNothingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            IEnumerable<ITrigger> trigs = GameController.FindTriggersWhere((ITrigger trig) => trig is PhaseChangeTrigger pca);
           foreach(ITrigger trig in trigs)
            {
                IEnumerator coroutine = trig.ActionPerformed(null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }


    }
}