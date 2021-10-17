using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class BlowoutCardController : BirthdayBoyCardController
    {

        public BlowoutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Select a target with a max hp of 5 or fewer.

            LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c.HitPoints.HasValue && c.HitPoints.Value <= 5, "target with 5 or fewer HP", useCardsSuffix: false, singular: "target with 5 or fewer HP",  plural: "targets with 5 or fewer HP") ;
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.Custom, cardCriteria, storedResults, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidSelectCard(storedResults))
            {
                Log.Debug("A card was not selected for Blowout");
                yield break;
            }

            Card selectedCard = GetSelectedCard(storedResults);
            int X = selectedCard.HitPoints.Value;
            Location selectedPlayArea = selectedCard.Location;

            //{BirthdayBoy} deals another target in the same play area as the selected target 5 - X fire damage, where X is the HP of the selected target.
            //TODO: Currently this is giving an error during Damage Preview. Figure out why that is the case
            coroutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), 5 - X, DamageType.Fire, 1, false, 1, additionalCriteria: c => c.Location == selectedPlayArea && c != selectedCard, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Destroy the selected target
            coroutine = GameController.DestroyCard(HeroTurnTakerController, selectedCard, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }


        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText($"Select a card to be destroyed",
                                            "They are selecting a card to be destroyed",
                                            "Vote for a card to be destroyed",
                                            "selecting a card to be destroyed");

        }


    }
}