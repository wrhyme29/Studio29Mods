﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class SocialLadderCardController : BirthdayBoyCardController
    {

        public SocialLadderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"{heroesWithPresents.Select(tt => tt.NameRespectingVariant).ToCommaList(useWordAnd: true)} {heroesWithPresents.Count().ToString_SingularOrPlural("has", "have")} presents in play.").Condition = () => heroesWithPresents.Any() && heroesWithNoPresents.Any();
            SpecialStringMaker.ShowSpecialString(() => $"There are no heroes with presents in play.").Condition = () => !heroesWithPresents.Any();

            SpecialStringMaker.ShowSpecialString(() => $"{heroesWithNoPresents.Select(tt => tt.NameRespectingVariant).ToCommaList(useWordAnd: true)} {heroesWithNoPresents.Count().ToString_SingularOrPlural("has", "have")} presents in play.").Condition = () => heroesWithNoPresents.Any() && heroesWithPresents.Any();
            SpecialStringMaker.ShowSpecialString(() => $"All heroes have presents in play.").Condition = () => !heroesWithNoPresents.Any() ;

        }


        IEnumerable<TurnTaker> heroesWithPresents => GameController.TurnTakerControllers.Where(ttc => ttc.TurnTaker.IsHero && ttc.TurnTaker != TurnTaker && !ttc.TurnTaker.IsIncapacitatedOrOutOfGame && GetPresentsInPlay().Any(c => GetOriginalOwner(c) == ttc.TurnTaker && GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()))).Select(ttc => ttc.TurnTaker);
        IEnumerable<TurnTaker> heroesWithNoPresents => GameController.TurnTakerControllers.Where(ttc => ttc.TurnTaker.IsHero && ttc.TurnTaker != TurnTaker && !ttc.TurnTaker.IsIncapacitatedOrOutOfGame && !GetPresentsInPlay().Any(c => GetOriginalOwner(c) == ttc.TurnTaker && GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()))).Select(ttc => ttc.TurnTaker);

        public override IEnumerator Play()
        {
            
            //Any other hero with presents in play from their deck draws X cards, where X is equal to the number of their presents in play 
            IEnumerator coroutine;
            int X;
            foreach(TurnTaker tt in heroesWithPresents)
            {
                X = GetPresentsInPlay().Count(present => GetOriginalOwner(present) == tt);
                coroutine = GameController.DrawCards(FindHeroTurnTakerController(tt.ToHero()), X, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Any other hero with no presents in play deals themselves 2 psychic damage."
            foreach(TurnTaker tt in heroesWithNoPresents)
            {
                IEnumerable<Card> heroCards = tt.CharacterCards.Where(c => c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame);
                foreach(Card hero in heroCards)
                {
                    coroutine = DealDamage(hero, hero, 2, DamageType.Psychic, cardSource: GetCardSource());
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

            yield break;
        }


    }
}