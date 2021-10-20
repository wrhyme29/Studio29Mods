using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Studio29.Lore
{
    public class LoreCardController : CardController
    {

        public LoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string StoryKeyword = "story";
        public static readonly string ActionKeyword = "action";
        public static readonly string EpicKeyword = "epic";
        public static readonly string MysteryKeyword = "mystery";
        public static readonly string MythKeyword = "myth";
        public static readonly string RomanceKeyword = "romance";

        protected bool IsStory(Card card)
        {
            return card.DoKeywordsContain(StoryKeyword);
        }

        protected bool IsAction(Card card)
        {
            return card.DoKeywordsContain(ActionKeyword);
        }

        protected bool IsEpic(Card card)
        {
            return card.DoKeywordsContain(ActionKeyword);
        }

        protected bool IsMystery(Card card)
        {
            return card.DoKeywordsContain(MysteryKeyword);
        }

        protected bool IsMyth(Card card)
        {
            return card.DoKeywordsContain(MythKeyword);
        }

        protected bool IsRomance(Card card)
        {
            return card.DoKeywordsContain(RomanceKeyword);
        }
    }
}