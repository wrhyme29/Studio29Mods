using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.CovenOfWitches
{
    public class EvanoraTheBargainerCardController : WitchCardController
    {

        public EvanoraTheBargainerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "CurseOfEvanora")
        {
        }


        public override void AddTriggers()
        {
            // At the start of the Environment turn, each player may discard 2 cards. Any player that does deals 1 non-environment 2 infernal damage.
            AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnResponse, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DealDamage
            });

            // At the end of the Environment turn, 1 hero may draw X cards, where X is the number of hero cards discarded this turn.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, TriggerType.DrawCard);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction arg)
        {
            //1 hero may draw X cards, where X is the number of hero cards discarded this turn.
            int X = Game.Journal.DiscardCardEntriesThisTurn().Count(entry => entry.ResponsibleTurnTaker.IsHero);
            IEnumerator coroutine = GameController.SelectHeroToDrawCards(DecisionMaker, numberOfCards: X, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            // each player may discard 2 cards. 
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && (tt as HeroTurnTaker).HasCardsInHand && (tt as HeroTurnTaker).Hand.Cards.Count() > 1, $"heroes with at least 2 cards in hand"), SelectionType.DiscardCard, (TurnTaker tt) =>
                SelectAndDiscardCards(FindHeroTurnTakerController((HeroTurnTaker)tt), 2, optional: true, 2, storedResults: storedResults), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (storedResults.Count() <= 0)
            {
                yield break;
            }

            foreach(HeroTurnTakerController httc in storedResults.Select(d => d.HeroTurnTakerController).Distinct())
            {
                if (httc != null)
                {
                    // Any player that does deals 1 non - environment 2 infernal damage.
                    List<Card> storedCharacter = new List<Card>();
                    coroutine = FindCharacterCard(httc.HeroTurnTaker, SelectionType.HeroToDealDamage, storedCharacter);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    Card card = storedCharacter.FirstOrDefault();
                    if (card != null)
                    {
                        coroutine = base.GameController.SelectTargetsAndDealDamage(httc, new DamageSource(GameController, card), 2, DamageType.Infernal, 1,  optional: false, 1, additionalCriteria: c => !c.IsEnvironment, cardSource: GetCardSource());
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
           
        }


       


    }
}