using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.TheDeliOfDisaster
{
    public class RamblingYentaCardController : DinerCardController
    {

        public RamblingYentaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private List<TurnTaker> _alreadyDecisionedHeroes;

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals 2 sonic damage to any hero with ongoing cards in play.Each player dealt damage this way may destroy an ongoing card. If they do not, this card deals them an additional 2 sonic damage.
            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //this card deals 2 sonic damage to any hero with ongoing cards in play.
            _alreadyDecisionedHeroes = new List<TurnTaker>();
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(Card, (Card hero) => hero.IsHeroCharacterCard && hero.Owner.HasCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsOngoing), 2, DamageType.Sonic, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerable<TurnTaker> heroesDealtDamage = storedResults.Where(dd => dd.DidDealDamage && !dd.Target.Owner.IsIncapacitatedOrOutOfGame).Select(dd => dd.Target.Owner).Distinct();
            List<DestroyCardAction> storedDestroyResults = new List<DestroyCardAction>() ;
            coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria(tt => heroesDealtDamage.Contains(tt)), SelectionType.DestroyCard, tt => GameController.SelectAndDestroyCard(ToHeroTurnTakerController(tt), new LinqCardCriteria((Card c) => c.IsOngoing && c.Owner == tt, "ongoing"), true, storedResultsAction: storedDestroyResults, cardSource: GetCardSource()), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerable<TurnTaker> heroesThatDestroyCards = storedDestroyResults.Where(dca => dca.WasCardDestroyed).Select(dca => dca.CardToDestroy.TurnTaker);
            IEnumerable<TurnTaker> heroesThatNeedExtraDamage = heroesDealtDamage.Where(tt => !heroesThatDestroyCards.Contains(tt));

            List<Card> storedCharacter;
            Card card;
            foreach (HeroTurnTaker hero in heroesThatNeedExtraDamage)
            {
                
                //this card deals them an additional 2 sonic damage.
                storedCharacter = new List<Card>();
                coroutine = FindCharacterCardToTakeDamage(hero, storedCharacter, Card, 2, DamageType.Sonic);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                card = storedCharacter.FirstOrDefault();
                if (card != null)
                {
                    IEnumerator coroutine2 = DealDamage(Card, card, 2, DamageType.Sonic);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine2);
                    }
                }
            }
         }
    }
}