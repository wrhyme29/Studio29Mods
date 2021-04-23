using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class AStoryOfSpeedCardController : LoreCardController
    {

        public AStoryOfSpeedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{Lore} deals one target 2 sonic damage. 
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Sonic, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

			//You may shuffle one trash into its deck.
			List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
			coroutine = GameController.SelectTurnTaker(base.HeroTurnTakerController, SelectionType.ShuffleTrashIntoDeck, storedResults, additionalCriteria: (TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (!DidSelectTurnTaker(storedResults))
			{
				yield break;
			}
			TurnTaker turnTaker = storedResults.First().SelectedTurnTaker;
			coroutine = GameController.ShuffleTrashIntoDeck(FindTurnTakerController(turnTaker),cardSource: GetCardSource());
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