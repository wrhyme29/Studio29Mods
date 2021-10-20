using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Lore
{
    public class SaveYourselfCardController : StoryCardController
    {

        public SaveYourselfCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ActionKeyword)
        {

        }

        public readonly string FirstTimeHasBeenDealtDamageToSelfThisTurnKey = "FirstTimeHasBeenDealtDamageToSelfThisTurn";
        private bool _destroyedByOwnEffectFlag = false;

        public override void AddTriggers()
        {
            //The first time each turn a hero target would deal itself damage, you may redirect that damage to any target. If you do, discard a card or destroy this card. If this card is destroyed this way, it first deals Lore 2 toxic damage.
            AddTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(dd.Target) && dd.Target.IsHero && !HasBeenSetToTrueThisTurn(FirstTimeHasBeenDealtDamageToSelfThisTurnKey), RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before);

            AddWhenDestroyedTrigger(DestroyCardResponse, new TriggerType[] { TriggerType.DealDamage }, additionalCriteria: DestroyCardAction => _destroyedByOwnEffectFlag );
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dd)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeHasBeenDealtDamageToSelfThisTurnKey);
            Card originalTarget = dd.Target;
            //you may redirect that damage to any target. 
            IEnumerator coroutine = GameController.SelectTargetAndRedirectDamage(DecisionMaker, (Card c) => GameController.IsCardVisibleToCardSource(c, GetCardSource()), dd, optional: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(dd.Target != originalTarget)
            {
                //If you do, discard a card or destroy this card. If this card is destroyed this way, Lore deals himself 2 toxic damage.
                List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
                coroutine = base.GameController.SelectAndDiscardCard(HeroTurnTakerController, optional: true, storedResults: storedResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (!DidDiscardCards(storedResults))
                {
                    _destroyedByOwnEffectFlag = true;
                    coroutine = base.GameController.DestroyCard(DecisionMaker, Card, cardSource: GetCardSource());
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

        private IEnumerator DestroyCardResponse(DestroyCardAction dca)
        {
            // Lore deals himself 2 toxic damage.
            _destroyedByOwnEffectFlag = false;
            IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, 2, DamageType.Toxic, cardSource: GetCardSource());
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