using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Debugger
{
    public class SetHitpointsCardController : CardController
    {

        public SetHitpointsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private Card CustomDecision_SelectedTarget = null;
        private enum CustomDecisionMode
        {
            SELECTCARD,
            SELECTHP
        }

        private CustomDecisionMode mode = CustomDecisionMode.SELECTCARD;
        public override IEnumerator Play()
        {
            //Select a target in play. Select a value between 0 and that target's maximum HP. Set that target's HP to the selected value.
            List<SelectCardDecision> storedTarget = new List<SelectCardDecision>();
            mode = CustomDecisionMode.SELECTCARD;
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SetHP, new LinqCardCriteria(c => c.IsTarget & c.IsInPlayAndHasGameText, "target", useCardsSuffix: false), storedTarget, optional: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidSelectCard(storedTarget))
            {
                coroutine = DestroyThisCardResponse(null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                yield break;
            }

            Card selectedTarget = GetSelectedCard(storedTarget);
            CustomDecision_SelectedTarget = selectedTarget;
            int maxHP = selectedTarget.MaximumHitPoints.Value;
            string[] hpChoices = Enumerable.Range(0, maxHP).Select(i => i.ToString()).ToArray();
            List<SelectWordDecision> hpDecisionResult = new List<SelectWordDecision>();
            mode = CustomDecisionMode.SELECTHP;
            coroutine = GameController.SelectWord(DecisionMaker, hpChoices, SelectionType.Custom, storedResults: hpDecisionResult, optional: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectWord(hpDecisionResult))
            {
                coroutine = DestroyThisCardResponse(null);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                yield break;
            }

            int selectedHP = Convert.ToInt32(GetSelectedWord(hpDecisionResult));
            coroutine = GameController.SetHP(selectedTarget, selectedHP, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DestroyThisCardResponse(null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            CustomDecisionText cdt = null;
            if (mode == CustomDecisionMode.SELECTCARD)
            {
                cdt = new CustomDecisionText($"Select a target to set the hp of",
                                            $"They are selecting a target to set the hp of",
                                            $"Vote for a target to set the hp of",
                                            $"selecting a target to set the hp of");
            }
            if (mode == CustomDecisionMode.SELECTHP)
            {
                cdt = new CustomDecisionText($"Select the hp that {CustomDecision_SelectedTarget.AlternateTitleOrTitle} should be set to",
                                            $"They are selecting the hp that {CustomDecision_SelectedTarget.AlternateTitleOrTitle} should be set to",
                                            $"Vote for the hp that {CustomDecision_SelectedTarget.AlternateTitleOrTitle} should be set to",
                                            $"selecting the hp that {CustomDecision_SelectedTarget.AlternateTitleOrTitle} should be set to");
            }
            return cdt;
        }
    }
}