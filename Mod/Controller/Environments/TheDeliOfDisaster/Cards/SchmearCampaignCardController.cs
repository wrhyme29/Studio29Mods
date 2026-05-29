using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.TheDeliOfDisaster
{
    public class SchmearCampaignCardController : TheDeliOfDisasterCardController
    {

        public SchmearCampaignCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, this card deals one hero character card 3 energy damage. If no damage is taken this way, destroy this card.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, DealDamageAtStartOfTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroySelf });

            //At the start of the villain turn, one player may discard a card to reveal the top card of the villain deck. They may discard or replace the revealed card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt.IsVillain, RevealOrDiscardVillainResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.RevealCard });
        }

        private IEnumerator RevealOrDiscardVillainResponse(PhaseChangeAction pca)
        {
            //At the start of the villain turn, one player may discard a card to reveal the top card of the villain deck.
            List<SelectTurnTakerDecision> heroes = new List<SelectTurnTakerDecision>();
            LinqTurnTakerCriteria heroCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => tt != null && tt.IsHero, "heroes");
            
            IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.DiscardCard, optional: true, allowAutoDecide: false, heroes, heroCriteria, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SelectTurnTakerDecision selectTurnTakerDecision = heroes.FirstOrDefault();
            if (selectTurnTakerDecision == null || selectTurnTakerDecision.SelectedTurnTaker == null || !selectTurnTakerDecision.SelectedTurnTaker.IsHero)
            {
                yield break;
            }

            HeroTurnTakerController heroTurnTakerController = FindHeroTurnTakerController(selectTurnTakerDecision.SelectedTurnTaker.ToHero());
            List<DiscardCardAction> discards = new List<DiscardCardAction>();
            if (heroTurnTakerController == null)
            {
                yield break;
            }
            coroutine = GameController.SelectAndDiscardCard(heroTurnTakerController, optional: true, storedResults: discards, responsibleTurnTaker: heroTurnTakerController.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(discards.Count == 0)
            {
                yield break;    
            }

            //they may discard or replace the revealed card.
            coroutine = TakeAPeekResponse(pca);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealDamageAtStartOfTurnResponse(PhaseChangeAction pca)
        {
            //this card deals one hero character card 3 energy damage.
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 3, DamageType.Energy, 1, false, 1, additionalCriteria: c => c.IsHeroCharacterCard && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedResultsDamage: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If no damage is taken this way, destroy this card.
            if(DidDealDamage(storedResults))
            {
                yield break;
            }

            coroutine = DestroyThisCardResponse(pca);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator TakeAPeekResponse(PhaseChangeAction pca)
        {
            TurnTakerController revealingTurnTakerController = FindTurnTakerController(pca.ToPhase.TurnTaker);
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            IEnumerator coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location l) => l.OwnerTurnTaker == pca.ToPhase.TurnTaker, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Location deck = GetSelectedLocation(storedResults);
            if (deck == null)
            {
                yield break;
            }
            List<Card> storedResultsCard = new List<Card>();
            coroutine = GameController.RevealCards(revealingTurnTakerController, deck, 1, storedResultsCard, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedResultsCard.FirstOrDefault();
            if (card != null)
            {
                List<MoveCardDestination> list = new List<MoveCardDestination>();
                list.Add(new MoveCardDestination(FindTrashFromDeck(deck)));
                list.Add(new MoveCardDestination(deck));
                coroutine = GameController.SelectLocationAndMoveCard(DecisionMaker, card, list, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            List<Location> list2 = new List<Location>();
            list2.Add(deck.OwnerTurnTaker.Revealed);
            coroutine = CleanupCardsAtLocations(list2, deck, shuffleAfterwards: false, cardsInList: storedResultsCard);
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