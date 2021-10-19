using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Studio29.TheDeliOfDisaster
{
    public class OrderUpCardController : TheDeliOfDisasterCardController
    {

        public OrderUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, reveal cards from the environment deck until a dish card is revealed. Put the dish card into play. Shuffle the remaining cards back into the environment deck.
            AddStartOfTurnTrigger(tt => TurnTaker == tt,
                pca => RevealCards_MoveMatching_ReturnNonMatchingCards(TurnTakerController, TurnTaker.Deck, playMatchingCards: true, putMatchingCardsIntoPlay: true, moveMatchingCardsToHand: false, cardCriteria: new LinqCardCriteria(c => IsDish(c), "dish"), numberOfMatches: 1, shuffleSourceAfterwards: true, showMessage: true),
                new TriggerType[] { TriggerType.RevealCard, TriggerType.PutIntoPlay, TriggerType.ShuffleDeck });
            //At the end of the environment turn, any player with fewer than 4 cards in their hand may discard a card to draw 2 cards.
            AddEndOfTurnTrigger(tt => TurnTaker == tt, EndOfTurnResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DrawCard });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            List<HeroTurnTaker> turnTakersWithLessThan4CardsInHand = Game.HeroTurnTakers.Where(htt => !htt.IsIncapacitatedOrOutOfGame && htt.NumberOfCardsInHand < 4 && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).ToList();
            return GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria(tt => tt.IsHero && turnTakersWithLessThan4CardsInHand.Contains(tt.ToHero())), SelectionType.DiscardAndDrawCard, DiscardToDraw2CardsResponse, turnTakersWithLessThan4CardsInHand.Count(), false, turnTakersWithLessThan4CardsInHand.Count(), allowAutoDecide: true, cardSource: GetCardSource());
        }

        private IEnumerator DiscardToDraw2CardsResponse(TurnTaker tt)
        {
            HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCards(httc, 1, false, 0, storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidDiscardCards(storedResults))
            {
                yield break;
            }

            coroutine = DrawCards(httc, 2);
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
}
