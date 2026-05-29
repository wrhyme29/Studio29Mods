using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Boomlagoon.JSON;

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

                if (!Game.IsOblivAeonMode)
                {
                    Card bzOption = TurnTaker.FindCard("SwitchBattlezones");
                    TurnTaker.MoveCard(bzOption, TurnTaker.InTheBox);
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
                
                yield break;
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
            }if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            foreach(CardController cc in TurnTaker.Deck.Cards.Select(c => FindCardController(c)))
            {
                Log.Debug("Running PerformEnteringGameResponse for " + cc.Card.Title);
                coroutine = cc.PerformEnteringGameResponse();
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

        private IEnumerator SelectEnvironment(List<string> storedResults)
        {
            Dictionary<string, string> titleIdentifierDictionary = new Dictionary<string, string>();
            IEnumerable<string> playableBaseIdentifiers = LoadPlayableBaseEnvironmentIdentifiers();
            foreach (string id in playableBaseIdentifiers)
            {
                Log.Debug("Environment in the box: " + id);
                string title = DeckDefinitionCache.GetDeckDefinition(id).Name;
                titleIdentifierDictionary.Add(title, id);
            }
                        
                
            IEnumerable<string> modIdentifiers = LoadAllModIdentifiers();
            foreach (string id in modIdentifiers)
            {
                Log.Debug("Environment in the box: " + id);
                string title = DeckDefinitionCache.GetDeckDefinition(id).Name;
                titleIdentifierDictionary.Add(title, id);
            }

            if (Game.IsOblivAeonMode)
            {
                //remove the other battlezone environment
                string otherEnv = TurnTaker.OtherBattleZone.FindEnvironment().Name;
                titleIdentifierDictionary.Remove(otherEnv);

                //remove any of the other 3 environments in OA mode
                if (Game.ExtraTurnTakers.Where((TurnTaker tt) => tt.IsEnvironment).Any())
                {
                    IEnumerable<string> otherOblivAeonEnvironments = Game.ExtraTurnTakers.Where((TurnTaker tt) => tt.IsEnvironment).Select(tt => tt.Name);
                    foreach (string env in otherOblivAeonEnvironments)
                    {
                        titleIdentifierDictionary.Remove(env);
                    }

                }
            }

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

        private IEnumerable<String> LoadPlayableBaseEnvironmentIdentifiers()
        {
            List<string> envIdentifiers = new List<string>();

            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == "SentinelsEngine").FirstOrDefault();
            if (assembly is null)
                return envIdentifiers;
            IEnumerable<string> unlockedExpansions = GameController.GetHeroCardsInBox(s => true, s => true).Select(kvp => kvp.Key).Where(id => !id.Contains('.')).Select(id => DeckDefinitionCache.GetDeckDefinition(id)).Where(dd => dd.ExpansionIdentifier != null).Select(dd => dd.ExpansionIdentifier).Distinct();
            foreach (var res in assembly.GetManifestResourceNames())
            {

                var stream = assembly.GetManifestResourceStream(res);
                if (stream is null || stream.Length == 0)
                    continue;


                JSONObject jsonObject;
                using (var sr = new System.IO.StreamReader(stream))
                {
                    string text = sr.ReadToEnd();
                    if (string.IsNullOrEmpty(text))
                        continue;
                    jsonObject = JSONObject.Parse(text);
                }
                if (jsonObject is null)
                    continue;
                var kind = jsonObject.GetString("kind");
                if (kind != "Environment")
                    continue;

                if (jsonObject.ContainsKey("expansionIdentifier"))
                {
                    var expansionIdentifier = jsonObject.GetString("expansionIdentifier");
                    if (!unlockedExpansions.Contains(expansionIdentifier))
                        continue;
                }
                string identifier = res.Replace("DeckList.json", string.Empty);
                identifier = identifier.Replace("Handelabra.Sentinels.Engine.DeckLists.", string.Empty);
                envIdentifiers.Add(identifier);
            }

            return envIdentifiers;
        }

        private IEnumerable<string> LoadAllModIdentifiers()
        {
            List<string> envIdentifiers = new List<string>();
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {

                Assembly assembly = ModHelper.GetAssemblyForAssemblyName(a.GetName());
                if (assembly is null)
                {
                    continue;
                }
                foreach (var res in assembly.GetManifestResourceNames())
                {
                    var stream = assembly.GetManifestResourceStream(res);
                    if (stream is null || stream.Length == 0)
                        continue;
                  

                    JSONObject jsonObject;
                    using (var sr = new System.IO.StreamReader(stream))
                    {
                        string text = sr.ReadToEnd();
                        if (string.IsNullOrEmpty(text))
                            continue;
                        jsonObject = JSONObject.Parse(text);
                    }
                    if (jsonObject is null)
                        continue;
                    var kind = jsonObject.GetString("kind");
                    if (kind != "Environment")
                        continue;
                    string identifier = res.Replace("DeckList.json", string.Empty);
                    identifier = identifier.Replace("DeckLists.", string.Empty);
                    if (identifier == TurnTaker.QualifiedIdentifier)
                        continue;
                    envIdentifiers.Add(identifier);
                }
            }

            return envIdentifiers;
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