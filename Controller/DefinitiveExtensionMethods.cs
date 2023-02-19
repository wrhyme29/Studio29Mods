using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Studio29
{
    public static class DefinitiveExtensionMethods
    {


        public static IEnumerator Discover(this CardController card, TurnTakerController ttc, Location deck, LinqCardCriteria criteria, int numToDiscover, List<Card> storedResults = null, bool shuffleTrashIntoDeckFirst = false)
        {
            if(shuffleTrashIntoDeckFirst)
            {
                IEnumerator shuffle = card.GameController.ShuffleTrashIntoDeck(ttc, cardSource: card.GetCardSource());
                if (card.UseUnityCoroutines)
                {
                    yield return card.GameController.StartCoroutine(shuffle);
                }
                else
                {
                    card.GameController.ExhaustCoroutine(shuffle);
                }
            }
            IEnumerator coroutine = card.RevealCards_MoveMatching_ReturnNonMatchingCards(ttc, deck, true, false, false, criteria, numToDiscover, storedPlayResults: storedResults);
            if (card.UseUnityCoroutines)
            {
                yield return card.GameController.StartCoroutine(coroutine);
            }
            else
            {
                card.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public static IEnumerator Summon(this CardController card, string cardIdentifierToSummon)
        {
            Card cardToSummon = card.GameController.FindCardController(cardIdentifierToSummon).Card;
            Location location = cardToSummon.Location;
            if (location.Name == LocationName.Trash || location.Name == LocationName.Deck)
            {
                IEnumerator coroutine = card.GameController.PlayCard(card.TurnTakerController, cardToSummon, isPutIntoPlay: true, cardSource: card.GetCardSource());
                if (card.UseUnityCoroutines)
                {
                    yield return card.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    card.GameController.ExhaustCoroutine(coroutine);
                }
                if (location.Name == LocationName.Deck)
                {
                    coroutine = card.GameController.ShuffleLocation(card.TurnTaker.Deck, cardSource: card.GetCardSource());
                    if (card.UseUnityCoroutines)
                    {
                        yield return card.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        card.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                yield break;
            }

            string message = "The " + cardToSummon.Title + " was not found in " + card.TurnTaker.Name + "'s deck nor trash.";
            if (cardToSummon.IsInPlayAndHasGameText)
            {
                message = "The " + cardToSummon.Title + " is already in play.";
            }
            IEnumerator coroutine2 = card.GameController.SendMessageAction(message, Priority.Medium, card.GetCardSource());
            if (card.UseUnityCoroutines)
            {
                yield return card.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                card.GameController.ExhaustCoroutine(coroutine2);
            }
        }

    }
}
