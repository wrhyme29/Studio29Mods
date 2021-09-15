using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class PartyTilDawnCardController : BirthdayBoyCardController
    {

        public PartyTilDawnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsPresent(c), "present"));
        }

        public override IEnumerator Play()
        {
            // You may use an additional power this turn.
            IncreasePhaseActionCountStatusEffect effect = new IncreasePhaseActionCountStatusEffect(1);
            effect.ToTurnPhaseCriteria.Phase = Phase.UsePower;
            effect.UntilThisTurnIsOver(Game);
            effect.ToTurnPhaseCriteria.TurnTaker = TurnTaker;
            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{BirthdayBoy} deals up to X targets 1 psychic damage each, where X is the number of presents in his play area."
            coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 1, DamageType.Psychic, GetPresentsInPlay().Count() + 1, false, 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            IEnumerable<IncreasePhaseActionCountStatusEffect> source = from sec in GameController.StatusEffectControllers
                                                                       where sec.StatusEffect.CardSource == base.Card && sec.StatusEffect is IncreasePhaseActionCountStatusEffect
                                                                       select sec.StatusEffect as IncreasePhaseActionCountStatusEffect;
            if (source.Count() > 0)
            {
                return source.Where((IncreasePhaseActionCountStatusEffect se) => se.ActualToTurnPhaseCriteria.GetTurnTakerExpression()(tt)).Count() > 0;
            }
            return false;
        }

        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            if (base.GameController.ActiveTurnPhase.IsUsePower )
            {
                return ShouldIncreasePhaseActionCount(base.GameController.ActiveTurnTaker);
            }
            return false;
        }


    }
}