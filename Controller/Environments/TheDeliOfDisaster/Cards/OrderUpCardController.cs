using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

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
            //At the end of the environment turn, any player with fewer than 4 cards in their hand may draw a card.
            AddEndOfTurnTrigger(tt => TurnTaker == tt,
                pca => EachPlayerDrawsACard(htt => htt.NumberOfCardsInHand < 4),
                TriggerType.DrawCard);
        }
    }
}