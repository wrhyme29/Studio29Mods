using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.TheTamer
{
    public class BalancingActCardController : TheTamerCardController
    {

        public BalancingActCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsLion(c), "lion"));
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsVillainTarget(c),useCardsSuffix: false, singular: "villain target", plural: "villain targets"));
        }

        public override IEnumerator Play()
        {
            //If there are more Lions than villain targets in play, each Lion may deal one target 1 melee damage.
            if (GetNumberOfLionsInPlay() > GetNumberOfVillainTargetsInPlay())
            {
                List<Card> allDamagedTargets = new List<Card>();

                List<Card> usedSources = new List<Card>();
                IEnumerator coroutine;
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
                    coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, lionSource), 1, DamageType.Melee, 1, true, 1,
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
                    if (DidDealDamage(storedDamage))
                    {
                        Card damagedTarget = storedDamage.First().Target;
                        if(!allDamagedTargets.Contains(damagedTarget))
                        {
                            allDamagedTargets.Add(damagedTarget);
                        }
                    }


                }
                //skip all of this is no targets were dealt damage
                if(allDamagedTargets.Count() > 0)
                {
                    //One target dealt damage this way deals one Lion 2 melee damage.
                    List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                    IEnumerator coroutine2 = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, allDamagedTargets, storedResults, optional: false, allowAutoDecide: true);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine2);
                    }
                    SelectCardDecision selectCardDecision = storedResults.FirstOrDefault();
                    if (selectCardDecision != null)
                    {
                        Card damageSource = GetSelectedCard(storedResults);
                        coroutine2 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, damageSource), 2, DamageType.Melee, 1, false, 1,
                            additionalCriteria: c => IsLion(c) && c.IsInPlayAndHasGameText, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }
                    }
                }
               
            } else
            {
                IEnumerator coroutine3 = base.GameController.SendMessageAction("There are not more Lions than Villain targets in play. No damage will be dealt.", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine3);
                }
            }

            yield break;
        }

        private int GetNumberOfVillainTargetsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsVillainTarget).Count();

        }


    }
}