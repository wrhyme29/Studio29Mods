using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;

namespace Studio29.TheDeliOfDisaster
{
    public class KugelConundrumCardController : TheDeliOfDisasterCardController
    {

        public KugelConundrumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, each hero character card with an odd number of hp regains 1 hp.
            //Each hero character card with an even number of hp may take 1 toxic damage from this card to deal 2 toxic damage to another target. 
            //Each character dealt damage by this card must discard 1 card.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, new TriggerType[] { TriggerType.GainHP, TriggerType.DealDamage, TriggerType.DiscardCard });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //Each hero character card with an odd number of hp regains 1 hp.
            IEnumerator coroutine = GameController.GainHP(DecisionMaker, c => c.IsHeroCharacterCard && c.HitPoints % 2 == 1, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Each hero character card with an even number of hp may take 1 toxic damage from this card to deal 2 toxic damage to another target. 
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 1, DamageType.Toxic, FindCardsWhere(c => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && c.HitPoints % 2 == 0).Count(), false, 0, additionalCriteria: c => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && c.HitPoints % 2 == 0, addStatusEffect: DealFollowupDamageResponse, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealFollowupDamageResponse(DealDamageAction dd)
        {
            if(!dd.DidDealDamage || dd.DidDestroyTarget)
            {
                yield break;
            }

            //deal 2 toxic damage to another target.
            Card source = dd.Target;
            HeroTurnTakerController hero = FindHeroTurnTakerController(source.Owner.ToHero());
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(hero, new DamageSource(GameController, source), 2, DamageType.Toxic, 1, false, 1, additionalCriteria: c => c != source, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Each character dealt damage by this card must discard 1 card.
            coroutine = GameController.SelectAndDiscardCard(hero, cardSource: GetCardSource());
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