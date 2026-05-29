using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.TheTamer
{
    public class RoadShowTamerCharacterCardController : HeroCharacterCardController
    {
		public RoadShowTamerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Reveal cards from the top of {TheTamer}'s deck until a Lion card is revealed. Put that card into play. Shuffle the other revealed cards into {TheTamer}'s deck."

			IEnumerator coroutine = RevealCards_MoveMatching_ReturnNonMatchingCards(HeroTurnTakerController, TurnTaker.Deck, playMatchingCards: true, putMatchingCardsIntoPlay: true, moveMatchingCardsToHand: false, cardCriteria: new LinqCardCriteria(c => c.IsLion(), "lion"), numberOfMatches: 1, shuffleSourceAfterwards: true, showMessage: true);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
		}

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            yield return this.StandardPowerPlayDrawIncapacitatedAbility(index);
        }
    }
}
