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
            //One Lion deals one other Lion 1 melee damage. A Lion dealt damage this way deals all non-hero targets 1 energy damage.
            IEnumerable<Card> choices = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsLion());
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsLion(), "lion"), storedResults, false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if(DidSelectCard(storedResults))
            {
                Card source = GetSelectedCard(storedResults);
                List<DealDamageAction> storedDamage = new List<DealDamageAction>() ;
                coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, source), 1, DamageType.Melee, new int?(1), false, new int?(1), additionalCriteria: (Card c) => c.IsLion() && c.IsInPlayAndHasGameText && c != source, storedResultsDamage: storedDamage, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if(DidDealDamage(storedDamage) && storedDamage.FirstOrDefault().DidDestroyTarget == false)
                {
                    Card target = storedDamage.FirstOrDefault().Target;
                    if(target.IsLion())
                    {
                        coroutine = DealDamage(target, (Card c) => !c.IsHero && c.IsTarget, 1, DamageType.Energy);
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

            yield break;
        }
    }
}