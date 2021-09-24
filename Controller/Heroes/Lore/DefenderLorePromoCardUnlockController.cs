using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Studio29
{
    public class DefenderLorePromoCardUnlockController : PromoCardUnlockController
    {

        public DefenderLorePromoCardUnlockController(GameController gameController) : base(gameController, "Studio29.Lore", "DefenderLoreCharacter")
        {

        }

        public const string DefenderLoreUnlockCondition1 = "DefenderLoreUnlockCondition1";
        public const string PostSaveDefenderLoreUnlockCondition1 = "DefenderLoreUnlockCondition1_h2656047196";

        public override bool IsUnlockPossibleThisGame()
        {
            return IsInGame("Lore", "LoreCharacter") && GameController.GetPersistentValueFromView<bool>(PostSaveDefenderLoreUnlockCondition1);
        }

        public string PrintFlag()
        {
            return GameController.GetPersistentValueFromView<bool>(DefenderLoreUnlockCondition1).ToString();
        }

        public override bool IsFlagPossibleThisGame()
        {
            return AreInGame(new string[] { "Lore", "TheTamer" },new Dictionary<string, string> { {"Lore", "LoreCharacter" }, { "TheTamer", "TheTamerCharacter" } }) && !GameController.GetPersistentValueFromView<bool>(PostSaveDefenderLoreUnlockCondition1);
        }

        public override void CheckForFlags(GameAction action)
        {
            //Lose a game with Lore and The Tamer
            if (IsGameOverDefeat(action))
            {
                Log.Debug(LogName.PromoCards, "Defender Lore flag has been set!");
                GameController.SetPersistentValueInView(DefenderLoreUnlockCondition1, value: true);
                ContinueCheckingForFlags = false;
            }
        }

        public override bool CheckForUnlock(GameAction action)
        {
            //Lore has plays a story card.
            // && IsGameOverVictory(action)
            if (GameController.GetPersistentValueFromView<bool>(PostSaveDefenderLoreUnlockCondition1))
            {
                IsUnlocked = FindCardsPlayedThisGame((Card c) => IsStory(c) && c.Owner.Identifier == "Lore").Distinct().Count() >= 1;
                IsUnlocked = true;
                Log.Debug(LogName.PromoCards, "The heroes have won a game where Lore played every Story card!");
            }

            return IsUnlocked;
        }

        public static readonly string StoryKeyword = "story";
        protected bool IsStory(Card card)
        {
            return card.DoKeywordsContain(StoryKeyword);
        }

    }
}