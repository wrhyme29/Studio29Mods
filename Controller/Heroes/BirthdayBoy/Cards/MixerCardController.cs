using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.BirthdayBoy
{
    public class MixerCardController : BirthdayBoyCardController
    {

        public MixerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //You may destroy any number of presents.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(HeroTurnTakerController, new LinqCardCriteria((Card c) => IsPresent(c), "present"), null,  requiredDecisions: 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //You may shuffle up to 2 trashes into their deck.
            List<Location> selectedTrashes = new List<Location>();
            for(int i = 0; i < 2; i++)
            {
                IEnumerable<LocationChoice> choices = FindLocationsWhere(loc => loc.IsTrash && !loc.OwnerTurnTaker.IsIncapacitatedOrOutOfGame && GameController.IsLocationVisibleToSource(loc, GetCardSource()) && !selectedTrashes.Contains(loc)).Select(loc => new LocationChoice(loc));

                List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                coroutine = GameController.SelectLocation(HeroTurnTakerController, choices, SelectionType.ShuffleTrashIntoDeck, storedResults, optional: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidSelectLocation(storedResults))
                {
                    Location selectedLocation = GetSelectedLocation(storedResults);
                    selectedTrashes.Add(selectedLocation);
                    coroutine = GameController.ShuffleTrashIntoDeck(FindTurnTakerController(selectedLocation.OwnerTurnTaker), cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    yield break;
                }
            }
           
        }


    }
}