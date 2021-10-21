using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.Lore
{
    public class RevengementCardController : StoryCardController
    {

        public RevengementCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ActionKeyword)
        {

        }

        public override void AddTriggers()
        {
            //Whenever a hero target with fewer than 10 hp is dealt damage, Lore may deal one target 1 cold damage.
            AddTrigger((DealDamageAction dd) => dd.Target.IsHero && dd.TargetHitPointsBeforeBeingDealtDamage < 10 && dd.DidDealDamage && !CharacterCard.IsIncapacitatedOrOutOfGame, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(DealDamageAction dd)
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Cold, 1, false, 0, cardSource: GetCardSource());
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