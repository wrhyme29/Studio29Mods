using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class SpeedReaderCardController : StoryCardController
    {

        public SpeedReaderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, EpicKeyword)
        {

        }

        public override void AddTriggers()
        {
            //At the end of your turn, you may destroy one Story and play one Story. If a Story is destroyed this way, {Lore} deals one target 1 sonic damage. If a Story is played this way, {Lore} deals himself 1 irreducible sonic damage. You may then play another Story. If you do, destroy this card.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.PlayCard, TriggerType.DealDamage });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction arg)
        {
            //you may destroy one Story
            List<DestroyCardAction> storedDestroy = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsStory(c), "story"), optional: true, storedResultsAction: storedDestroy, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...may play one Story. 
            List<PlayCardAction> storedFirstPlay = new List<PlayCardAction>();
            coroutine = GameController.SelectAndPlayCardsFromHand(DecisionMaker, numberOfCards: 1, optional: false, requiredCards: 0, cardCriteria: new LinqCardCriteria(c => IsStory(c), "story"), storedResults: storedFirstPlay, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If a Story is destroyed this way, {Lore} deals one target 1 sonic damage. 
            if(DidDestroyCard(storedDestroy))
            {
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Sonic, 1, false, 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //If a Story is played this way, {Lore} deals himself 1 irreducible sonic damage. 
            if (!DidPlayCards(storedFirstPlay))
            {
                yield break;

            }
            coroutine = DealDamage(CharacterCard, CharacterCard, 1, DamageType.Sonic, isIrreducible: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            

            //You may then play another Story. 
            List<PlayCardAction> storedSecondPlay = new List<PlayCardAction>();
            coroutine = GameController.SelectAndPlayCardsFromHand(DecisionMaker, numberOfCards: 1, optional: false, requiredCards: 0, cardCriteria: new LinqCardCriteria(c => IsStory(c), "story"), storedResults: storedSecondPlay, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If you do, destroy this card.
            if (DidPlayCards(storedSecondPlay))
            {
                coroutine = DestroyThisCardResponse(storedSecondPlay.First());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}