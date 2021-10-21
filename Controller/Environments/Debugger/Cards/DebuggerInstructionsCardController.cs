using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Debugger
{
    public class DebuggerInstructionsCardController : VillainCharacterCardController
    {

        public DebuggerInstructionsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddInhibitorException((GameAction ga) => ga is PlayCardAction && Card.Location.IsDeck);
            AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        private enum CustomDecisionMode
        {
            SelectOption,
            SelectEnvironment
        }

        private CustomDecisionMode customDecisionMode = CustomDecisionMode.SelectOption;

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

                coroutine = MoveAllOptionCardsUnderMenu();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = SetEnvironmentForGame();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

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

        private IEnumerator MoveAllOptionCardsUnderMenu()
        {
            IEnumerable<Card> options = FindCardsWhere(new LinqCardCriteria(c => IsOption(c) && c.Owner == TurnTaker));
            IEnumerator coroutine = base.GameController.BulkMoveCards(TurnTakerController, options, Card.UnderLocation, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
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
            IEnumerable<Card> options = FindCardsWhere(new LinqCardCriteria(c => IsOption(c) && Card.UnderLocation.HasCard(c))).OrderBy(c => c.Title);
            string[] optionChoices = options.Select(c => c.Title).ToArray();
            List<SelectWordDecision> optionsDecisionResult = new List<SelectWordDecision>();
            IEnumerator coroutine;
            string selectedTitle;
            Card cardToPlay;
            customDecisionMode = CustomDecisionMode.SelectOption;

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
            return card != null && card.DoKeywordsContain(OptionKeyword, evenIfUnderCard: true);
        }



        private IEnumerator SetEnvironmentForGame()
        {
            /*
             * Here the players should be prompted to select another environment
             * However, that functionality is currently not possible, but has been feature requested
             * In the meantime, we will always pull in InsulaPrimalis
             */

            //TODO: Replace with selected environment identifier once that functionality exists
            List<string> storedResults = new List<string>();
            IEnumerator coroutine = SelectEnvironment(storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(storedResults.Count() == 0)
            {
                yield break;
            }
            string environmentIdentifier = storedResults.First();
            DeckDefinition deckDefinition = DeckDefinitionCache.GetDeckDefinition(environmentIdentifier);
            Card modelCard;
            CardController cardController;
            foreach (CardDefinition cardDefinition in deckDefinition.CardDefinitions)
            {
                for (int i = 0; i < cardDefinition.Count; i++)
                {
                    modelCard = new Card(cardDefinition, TurnTaker, i);
                    TurnTaker.OffToTheSide.AddCard(modelCard);


                    string overrideNamespace = TurnTaker.QualifiedIdentifier;
                    cardController = CardControllerFactory.CreateInstance(modelCard, TurnTakerController, overrideNamespace: environmentIdentifier);
                    TurnTakerController.AddCardController(cardController);
                    List<string> list = new List<string>();
                    list.Add(overrideNamespace);
                    GameController.AddCardPropertyJournalEntry(modelCard, "OverrideTurnTaker", list);
                    TurnTaker.Deck.AddCard(modelCard);
                }
            }

            coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }


            coroutine = GameController.SendMessageAction($"The environment is now {deckDefinition.Name}!", Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator SelectEnvironment(List<string> storedResults)
        {
            string insulaPrimalis = "Insula Primalis";
            string ruinsOfAtlantis = "Ruins of Atlantis";
            string megalopolis = "Megalopolis";
            string wagnerMarsBase = "Wagner Mars Base";

            Dictionary<string, string> titleIdentifierDictionary = new Dictionary<string, string>()
            {
                {insulaPrimalis, "InsulaPrimalis" },
                {megalopolis, "Megalopolis" },
                {ruinsOfAtlantis, "RuinsOfAtlantis" },
                {wagnerMarsBase, "WagnerMarsBase" }
            };

            string[] optionChoices = titleIdentifierDictionary.Keys.ToArray();
            List<SelectWordDecision> optionsDecisionResult = new List<SelectWordDecision>();
            customDecisionMode = CustomDecisionMode.SelectEnvironment;
            IEnumerator coroutine = GameController.SelectWord(DecisionMaker, optionChoices, SelectionType.Custom, storedResults: optionsDecisionResult, optional: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectWord(optionsDecisionResult))
            {
                yield break;
            }

            string selectedEnvironment = GetSelectedWord(optionsDecisionResult);
            string selectedEnvironmentIdentifier = titleIdentifierDictionary[selectedEnvironment];
            storedResults.Add(selectedEnvironmentIdentifier);

            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            CustomDecisionText cdt = null;
            if (customDecisionMode == CustomDecisionMode.SelectOption)
            {
                cdt = new CustomDecisionText("Select an option:",
                                                "They are selecting an option.",
                                                "Vote for an option to select.",
                                                "option to select");
            }

            if(customDecisionMode == CustomDecisionMode.SelectEnvironment)
            {
                cdt = new CustomDecisionText(   "Select an environment to use",
                                                "They are selecting an environment to use",
                                                "Vote for an environment to use",
                                                "environment to use");
            }
            return cdt;
        }


    }
}