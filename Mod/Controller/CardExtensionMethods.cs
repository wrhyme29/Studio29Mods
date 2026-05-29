using Handelabra.Sentinels.Engine.Model;

namespace Studio29
{
    public static class CardExtensionMethods
    {
        public static bool IsLion(this Card card) => card.DoKeywordsContain("lion");
        public static bool IsStory(this Card card) => card.DoKeywordsContain("story");
        public static bool IsAction(this Card card) => card.DoKeywordsContain("action");
        public static bool IsEpic(this Card card) => card.DoKeywordsContain("epic");
        public static bool IsMystery(this Card card) => card.DoKeywordsContain("mystery");
        public static bool IsMyth(this Card card) => card.DoKeywordsContain("myth");
        public static bool IsRomance(this Card card) => card.DoKeywordsContain("romance");
        public static bool IsPresent(this Card card) => card.DoKeywordsContain("present");
        public static bool IsCurse(this Card card) => card.DoKeywordsContain("curse");
        public static bool IsWitch(this Card card) => card.DoKeywordsContain("witch");
        public static bool IsDish(this Card card) => card.DoKeywordsContain("dish");
    }
}
