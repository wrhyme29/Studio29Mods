using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.DummyPlayer
{
	public class DummyPlayerCharacterCardController : DummyPlayerUtilityCharacterCardController
	{
		public DummyPlayerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}


		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						IEnumerator coroutine3 = SelectHeroToPlayCard(DecisionMaker);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}
						break;
					}
				case 1:
					{
						IEnumerator coroutine2 = base.GameController.SelectHeroToUsePower(DecisionMaker, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine2);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine2);
						}
						break;
					}
				case 2:
					{
						IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(DecisionMaker, optionalSelectHero: false, optionalDrawCard: true, allowAutoDecideHero: false, null, null, null, GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
			}
		}


	}
}
