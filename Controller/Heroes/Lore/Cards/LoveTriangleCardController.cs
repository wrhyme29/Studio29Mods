using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Lore
{
    public class LoveTriangleCardController : StoryCardController
    {

        public LoveTriangleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, RomanceKeyword)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //{Lore} deals himself 2 psychic damage. Two character cards other than {Lore} regain 2 hp.
            int selfDamage = GetPowerNumeral(0, 2);
            int hpGain = GetPowerNumeral(1, 2);

            IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, selfDamage, DamageType.Psychic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SelectAndGainHP(DecisionMaker, hpGain, additionalCriteria: (Card c) => c.IsInPlayAndHasGameText && c.IsCharacter && c != CharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()), numberOfTargets: 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

    }
}