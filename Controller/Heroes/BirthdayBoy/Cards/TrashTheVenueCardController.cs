using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.BirthdayBoy
{
    public class TrashTheVenueCardController : BirthdayBoyCardController
    {

        public TrashTheVenueCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);
        }

        public override IEnumerator Play()
        {
            //Destroy up to 2 environment cards. 

            List<DestroyCardAction> storedResults = new List<DestroyCardAction>()
                ;
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment"), 2, optional: false, requiredDecisions: 0, storedResultsAction: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //For each card destroyed this way, move 1 hero ongoing, hero equipment, or hero target with max 5 HP or fewer belonging to another hero in play to your hand.
            if(DidDestroyCards(storedResults))
            {
                foreach(DestroyCardAction dca in storedResults)
                {
                    coroutine = GrabPresentsFromOtherPlayers();
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

        private IEnumerator GrabPresentsFromOtherPlayers()
        {
            IEnumerator coroutine;
            if (TurnTaker.GetAllCards().Where(c => !c.IsOffToTheSide && !c.IsOutOfGame).Count() < 41)
            {
                coroutine = MoveCardsToOwnHand();
            }
            else
            {
                coroutine = GameController.SendMessageAction($"{Card.Title} already owns 40 cards! Let's not go overboard with the presents!", Priority.High, GetCardSource(), showCardSource: true);
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

		private IEnumerator MoveCardsToOwnHand()
		{
			//{BirthdayBoy} may move any hero ongoing, hero equipment, or hero target with max 5 HP or fewer in play to your hand. Any card moved this way now belongs to {BirthdayBoy} (when it is destroyed, shuffle into the deck of {BirthdayBoy}). Any card moved this way gains the keyword “Present”
			LinqCardCriteria criteria = new LinqCardCriteria((Card c) => c.Owner != base.TurnTaker && c.IsInPlayAndHasGameText && c.IsHero && (c.IsOngoing || IsEquipment(c) || (c.IsTarget && c.MaximumHitPoints <= 5)), "hero ongoing, hero equipment, or hero target with max 5hp");
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = GameController.MoveCards(base.HeroTurnTakerController, criteria, (Card c) => base.HeroTurnTaker.Hand, numberOfCards: new int?(1),  storedResults: storedResults, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if (DidSelectCard(storedResults))
			{
				//Any card moved this way now belongs to { BirthdayBoy} (when it is destroyed, shuffle into the deck of { BirthdayBoy}).
				Card movedCard = GetSelectedCard(storedResults);
				Log.Debug("Old owner: " + movedCard.Owner.Identifier);
				GameController.AddCardPropertyJournalEntry(movedCard, "OverrideTurnTaker", new string[] { movedCard.Owner.QualifiedIdentifier, movedCard.Identifier });
				GameController.ChangeCardOwnership(movedCard, TurnTaker);

				Log.Debug("New owner: " + movedCard.Owner.Identifier);
				Log.Debug("Original owner: " + GetOriginalOwner(movedCard).Identifier);

				coroutine = GameController.ModifyKeywords("present", addingOrRemoving: true, affectedCards: movedCard.ToEnumerable().ToList(), cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				coroutine = GameController.SendMessageAction($"{movedCard.Title} is now a Present belonging to { base.CharacterCard.AlternateTitleOrTitle}", Priority.High, GetCardSource());
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