using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Debugger
{
    public class DebuggerInstructionsCardController : VillainCharacterCardController
    {

        public DebuggerInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "Environment cards cannot be played.", showInEffectsList: () => true);

            AddInhibitorException((GameAction ga) => ga is PlayCardAction && Card.Location.IsDeck);
            AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            IEnumerator coroutine;
            if (TurnTakerController is DebuggerTurnTakerController)
            {
                List<bool> didEnterPlay = new List<bool>();
                DebuggerTurnTakerController debuggerTTC = TurnTakerController as DebuggerTurnTakerController;
                coroutine = GameController.PlayCard(TurnTakerController, Card, isPutIntoPlay: true, wasCardPlayed: didEnterPlay, canBeCancelled: false, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(didEnterPlay.First() == false)
                    Log.Debug($"Debugger Instructions card did not enter play.");

                coroutine = OpenDebuggingMenuInterface();
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

        public override void AddTriggers()
        {
            CannotPlayCards(ttc => ttc == TurnTakerController);
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => OpenDebuggingMenuInterface(), TriggerType.FirstTrigger);
        }

            private IEnumerator OpenDebuggingMenuInterface()
        {
            IEnumerator coroutine = GameController.SendMessageAction("Launching the debugging menu...", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = ShowDebuggingMenu();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SendMessageAction("Closing the debugging menu...", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator ShowDebuggingMenu()
        {
            IEnumerable<Card> options = FindCardsWhere(new LinqCardCriteria(c => IsOption(c) && c.Owner == TurnTaker)).OrderBy(c => c.Title);
            string[] optionChoices = options.Select(c => c.Title).ToArray();
            List<SelectWordDecision> optionsDecisionResult = new List<SelectWordDecision>();
            IEnumerator coroutine;
            string selectedTitle;
            Card cardToPlay;

            while (true)
            {
                coroutine = GameController.SelectWord(DecisionMaker, optionChoices, SelectionType.Custom, storedResults: optionsDecisionResult, optional: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(!DidSelectWord(optionsDecisionResult))
                {
                    yield break;
                }

                selectedTitle = GetSelectedWord(optionsDecisionResult);
                optionsDecisionResult.Clear();
                cardToPlay = options.Where(c => c.Title == selectedTitle).First();

                coroutine = GameController.PlayCard(DecisionMaker, cardToPlay, isPutIntoPlay: true, canBeCancelled: false, cardSource: GetCardSource());
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


        public static readonly string OptionKeyword = "option";
        private bool IsOption(Card card)
        {
            return card != null && card.DoKeywordsContain(OptionKeyword);
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText(  "Select an option:", 
                                            "They are selecting an option.", 
                                            "Vote for an option to select.", 
                                            "option to select");

        }


    }
}