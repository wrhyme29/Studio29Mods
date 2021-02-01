using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class YoureInvitedCardController : BirthdayBoyCardController
    {

        public YoureInvitedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Any other hero with no presents in play may play an ongoing card, equipment card, or target with no more than a max hp of 5.
            IEnumerable<TurnTaker> heroesWithNoPresents = GameController.TurnTakerControllers.Where(ttc => ttc.TurnTaker.IsHero && ttc.TurnTaker != TurnTaker && !ttc.TurnTaker.IsIncapacitatedOrOutOfGame && !GetPresentsInPlay().Any(c => GetOriginalOwner(c) == ttc.TurnTaker && GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()))).Select(ttc => ttc.TurnTaker);
            HeroTurnTakerController httc;
            IEnumerator coroutine;
            foreach(TurnTaker tt in heroesWithNoPresents)
            {
                httc = FindHeroTurnTakerController(tt.ToHero());
                coroutine = GameController.SelectAndPlayCardFromHand(httc, true, cardCriteria: new LinqCardCriteria(c => c.IsOngoing || IsEquipment(c) || (c.IsTarget && c.MaximumHitPoints <= 5), "ongoing card, equipment card, or target with no more than a max hp of 5", useCardsSuffix: false), cardSource: GetCardSource());
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