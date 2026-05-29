using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Studio29;

namespace Studio29.CovenOfWitches
{
    public class WitchCardController : CovenOfWitchesCardController
    {

        public string RelatedCurseIdentifier { get; set; }


        public WitchCardController(Card card, TurnTakerController turnTakerController, string relatedCurseIdentifier) : base(card, turnTakerController)
        {
            RelatedCurseIdentifier = relatedCurseIdentifier;
        }

        public override IEnumerator Play()
        {
            // When this card enters play, summon the related curse. "
            IEnumerator coroutine = this.Summon(RelatedCurseIdentifier);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);

            }
        }


    }
}