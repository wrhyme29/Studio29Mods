using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Studio29.DummyPlayer
{
    public class DummyPlayerCharacterCardController : TestPlayerUtilityCharacterCardController
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
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine3);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine3);
						}
						break;
					}
				case 1:
					{
						IEnumerator coroutine2 = GameController.SelectHeroToUsePower(DecisionMaker, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine2);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine2);
						}
						break;
					}
				case 2:
					{
						IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, optionalSelectHero: false, optionalDrawCard: true, allowAutoDecideHero: false, null, null, null, GetCardSource());
						if (UseUnityCoroutines)
						{
							yield return GameController.StartCoroutine(coroutine);
						}
						else
						{
							GameController.ExhaustCoroutine(coroutine);
						}
						break;
					}
			}
		}


	}
}
