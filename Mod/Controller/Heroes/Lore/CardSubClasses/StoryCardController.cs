using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.Lore
{
    public class StoryCardController : LoreCardController
    {

        public StoryCardController(Card card, TurnTakerController turnTakerController, string genreKeyword) : base(card, turnTakerController)
        {
            GenreKeyword = genreKeyword;
        }

        private string GenreKeyword { get; set; }

        public override IEnumerator Play()
        {
            //Return all other [matching genre] cards in play to your hand.
            IEnumerator coroutine = GameController.MoveCards(DecisionMaker, new LinqCardCriteria((Card c) => IsMatchingKeyword(c)  && c.IsInPlayAndHasGameText && c != base.Card, GenreKeyword), (Card c) => HeroTurnTaker.Hand, selectionType: SelectionType.MoveCardToHand, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool IsMatchingKeyword(Card card)
        {
            return card.DoKeywordsContain(GenreKeyword);
        }
    }
}