using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheTamer
{
    public class WhippingWhiskersCardController : TheTamerCardController
    {

        public WhippingWhiskersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //One Lion deals one other Lion 2 melee damage. A Lion dealt damage this way deals all non-hero targets 1 energy damage.
            IEnumerable<Card> choices = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsLion(c));
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsLion(c), "lion"), storedResults, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidSelectCard(storedResults))
            {
                Card source = GetSelectedCard(storedResults);
                List<DealDamageAction> storedDamage = new List<DealDamageAction>() ;
                coroutine = GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(GameController, source), 2, DamageType.Melee, new int?(1), false, new int?(1), additionalCriteria: (Card c) => IsLion(c) && c.IsInPlayAndHasGameText && c != source, storedResultsDamage: storedDamage, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if(DidDealDamage(storedDamage) && storedDamage.FirstOrDefault().DidDestroyTarget == false)
                {
                    Card target = storedDamage.FirstOrDefault().Target;
                    if(IsLion(target))
                    {
                        coroutine = DealDamage(target, (Card c) => !c.IsHero && c.IsTarget, 1, DamageType.Energy);
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

            yield break;
        }
    }
}