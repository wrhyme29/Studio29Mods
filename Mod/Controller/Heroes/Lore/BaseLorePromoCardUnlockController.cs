using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System.Reflection;
using System;

namespace Studio29
{
    public class BaseLorePromoCardUnlockController : PromoCardUnlockController
    {

        public BaseLorePromoCardUnlockController(GameController gameController) : base(gameController, "Studio29.Lore", "LoreCharacter")
        {

        }

        public override bool IsUnlockPossibleThisGame()
        {
            return IsInGame("Lore", "LoreCharacter");
        }

     
       
        public override bool CheckForUnlock(GameAction action)
        {

            if (IsGameOver(action))
            {
                Log.Debug("Unlock for Lore Character achieved!");
                return true;
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