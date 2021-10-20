using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.TheTamer
{
    public class NemeanCoatCardController : TheTamerCardController
    {

        public NemeanCoatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Until the start of your next turn, {TheTamer} is immune to melee damage.
            ImmuneToDamageStatusEffect effect = new ImmuneToDamageStatusEffect();
            effect.DamageTypeCriteria.AddType(DamageType.Melee);
            effect.TargetCriteria.IsSpecificCard = base.CharacterCard;
            effect.UntilStartOfNextTurn(base.TurnTaker);
            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

    }
}