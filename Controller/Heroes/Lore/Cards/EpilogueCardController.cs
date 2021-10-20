using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Studio29.Lore
{
    public class EpilogueCardController : StoryCardController
    {

        public EpilogueCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, MythKeyword)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Select a trash. You may search that trash for a target and put that target into play.

            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>() ;
            IEnumerator coroutine = GameController.SelectATrash(DecisionMaker, SelectionType.PlayCard, (Location loc) => GameController.IsLocationVisibleToSource(loc, GetCardSource()), storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidSelectLocation(storedResults))
            {
                yield break;
            }
            Location selectedTrash = GetSelectedLocation(storedResults);
            List<MoveCardDestination> destination = new List<MoveCardDestination>() { new MoveCardDestination(selectedTrash.OwnerTurnTaker.PlayArea) };
            coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, selectedTrash, 0, 1, new LinqCardCriteria(c => c.IsTarget, "target", useCardsSuffix: false, singular: "target", plural: "targets"), destination, isPutIntoPlay: true, cardSource: GetCardSource()); 
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