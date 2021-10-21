using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using System.Linq;

namespace Studio29Tests
{
    [TestFixture()]
    public class DebuggerTests : CustomBaseTest
    {

        #region TheDeliOfDisasterHelperFunctions

        protected TurnTakerController debugger { get { return FindEnvironment(); } }
        protected bool IsOption(Card card)
        {
            return card != null && card.DoKeywordsContain("option", evenIfUnderCard: true);
        }

        private Card debuggerMenu => FindCardsWhere(c => c.Identifier == "DebuggerInstructions", false).First();

        #endregion

        [Test()]
        public void TestDebuggerWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.Debugger");
            DecisionSelectWords = new string[] {"Insula Primalis", null };
            StartGame(resetDecisions: false);
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        public void TestDebuggerMovesOptionsUnder()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.Debugger");
            DecisionSelectWords = new string[] { "Insula Primalis", null };
            StartGame(resetDecisions: false);
            AssertNumberOfCardsUnderCard(debuggerMenu, 6);
            AssertNumberOfCardsInDeck(debugger, 15);

        }

        [Test()]
        public void TestDebugger_SendEnvironmentCardToTrash()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.Debugger");
            DecisionSelectWords = new string[] { "Insula Primalis", null, "4. Send Cards to Trash", null };
            StartGame(resetDecisions: false);

            GoToPlayCardPhase(debugger);
            Card topEnvironmentCard = debugger.TurnTaker.Deck.TopCard;
            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(debugger.TurnTaker.Deck) };
            DecisionSelectCards = new Card[] { topEnvironmentCard, null };

            GoToEndOfTurn(debugger);

            AssertInTrash(topEnvironmentCard);


        }

        [Test()]
        public void TestDebugger_PlayEnvironmentCard()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Studio29.Debugger");
            DecisionSelectWords = new string[] { "Insula Primalis", null, "3. Play Cards", null };
            StartGame(resetDecisions: false);

            GoToPlayCardPhase(debugger);
            Card topEnvironmentCard = GetCard("EnragedTRex");
            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(debugger.TurnTaker.Deck) };
            DecisionSelectCards = new Card[] { topEnvironmentCard, null };

            GoToEndOfTurn(debugger);

            AssertInPlayArea(debugger, topEnvironmentCard);


        }


    }
}
