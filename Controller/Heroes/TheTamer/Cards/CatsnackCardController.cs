using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.TheTamer
{
    public class CatsnackCardController : TheTamerCardController
    {

        public CatsnackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //You may discard a card...
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = SelectAndDiscardCards(base.HeroTurnTakerController, new int?(1), requiredDecisions: new int?(0), storedResults: storedResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDiscardCards(storedResults))
            {
                //If you do, each Lion in play deals 1 target 1 melee damage.
                List<Card> usedSources = new List<Card>();
                IEnumerable<Card> lionsInPlay = FindLionsInPlay();
                while (lionsInPlay.Count() > 0)
                {
                    IEnumerable<Card> source = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && lionsInPlay.Contains(c) && !usedSources.Contains(c));
                    if (source.Count() == 0)
                    {
                        break;
                    }
                    Card lionSource = lionsInPlay.First();
                    if (lionsInPlay.Count() > 1)
                    {
                        List<SelectCardDecision> storedTargetResults = new List<SelectCardDecision>();
                        coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, source, storedTargetResults, optional: false, allowAutoDecide: true);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        SelectCardDecision selectTargetDecision = storedTargetResults.FirstOrDefault();
                        if (selectTargetDecision != null)
                        {
                            lionSource = GetSelectedCard(storedTargetResults);
                        }
                    }


                    usedSources.Add(lionSource);

                    List<SelectCardDecision> selectCards = new List<SelectCardDecision>();
                    List<DealDamageAction> storedDamage = new List<DealDamageAction>();
                    coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, lionSource), 1, DamageType.Melee, 1, false, 1,
                        additionalCriteria: c => c.IsTarget && c.IsInPlayAndHasGameText,
                        storedResultsDecisions: selectCards,
                        storedResultsDamage: storedDamage,
                        cardSource: GetCardSource());
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