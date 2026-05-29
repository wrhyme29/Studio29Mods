using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections;

namespace Studio29
{
    public static class HeroCharacterCardControllerExtensionMethods
    {
        public static IEnumerator StandardPowerPlayDrawIncapacitatedAbility(this HeroCharacterCardController hccc, int index)
        {
            switch (index)
            {
                case 0:
                {
                    IEnumerator coroutine3 = hccc.GameController.SelectHeroToPlayCard(hccc.DecisionMaker, cardSource: hccc.GetCardSource());
                    if (hccc.UseUnityCoroutines)
                    {
                        yield return hccc.GameController.StartCoroutine(coroutine3);
                    }
                    else
                    {
                        hccc.GameController.ExhaustCoroutine(coroutine3);
                    }
                    break;
                }
                case 1:
                {
                    IEnumerator coroutine2 = hccc.GameController.SelectHeroToUsePower(hccc.DecisionMaker, cardSource: hccc.GetCardSource());
                    if (hccc.UseUnityCoroutines)
                    {
                        yield return hccc.GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        hccc.GameController.ExhaustCoroutine(coroutine2);
                    }
                    break;
                }
                case 2:
                {
                    IEnumerator coroutine = hccc.GameController.SelectHeroToDrawCard(hccc.DecisionMaker, cardSource: hccc.GetCardSource());
                    if (hccc.UseUnityCoroutines)
                    {
                        yield return hccc.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        hccc.GameController.ExhaustCoroutine(coroutine);
                    }
                    break;
                }
                default:
                    Log.Warning("Unknown incap index passed in: " + index);
                    break;
            }
        }
    }
}
