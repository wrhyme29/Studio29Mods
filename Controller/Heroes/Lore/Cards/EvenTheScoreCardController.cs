using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Studio29.Lore
{
    public class EvenTheScoreCardController : LoreCardController
    {

        public EvenTheScoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //If there is a myth card in play, any hero with an odd number of HP regains 1 hp.
            bool mythCardsInPlay = FindCardsWhere(c => c.IsInPlayAndHasGameText && IsMyth(c)).Any();
            IEnumerator coroutine;
            if(mythCardsInPlay)
            {
                coroutine = GameController.GainHP(DecisionMaker, (Card c) => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.HitPoints.Value.IsEven(), 1, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            } else
            {
                //If there is not a myth card in play, draw a card.
                coroutine = DrawCard();
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