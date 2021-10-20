using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.BirthdayBoy
{
    public class GiftCertificateCardController : BirthdayBoyCardController
    {

        public GiftCertificateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


        public override void AddTriggers()
        {
            AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);
        }


		public override IEnumerator UsePower(int index = 0)
		{
			//{BirthdayBoy} may move any hero ongoing, hero equipment, or hero target with max 5 HP or fewer in another player's hand to your hand. Any card moved this way now belongs to {BirthdayBoy} (when it is destroyed, shuffle into the deck of {BirthdayBoy}). Any card moved this way gains the keyword “Present”
			IEnumerator coroutine;
			if (NumberOfCardsBirthdayBoyOwns < 40)
			{
				coroutine = MoveCardsToOwnHand();
			}
			else
			{
				coroutine = GameController.SendMessageAction($"{TurnTaker.Name} already owns 40 cards! Let's not go overboard with the presents!", Priority.High, GetCardSource(), showCardSource: true);
			}
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

		private IEnumerator MoveCardsToOwnHand()
		{
			//{BirthdayBoy} may move any hero ongoing, hero equipment, or hero target with max 5 HP or fewer in another player's hand to your hand. Any card moved this way now belongs to {BirthdayBoy} (when it is destroyed, shuffle into the deck of {BirthdayBoy}). Any card moved this way gains the keyword “Present”
			LinqCardCriteria criteria = new LinqCardCriteria((Card c) => c.Owner != base.TurnTaker && c.IsInHand && c.IsHero && (c.IsOngoing || IsEquipment(c) || (c.IsTarget && c.MaximumHitPoints <= 5)), "hero ongoing, hero equipment, or hero target with max 5hp");
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			IEnumerator coroutine = GameController.MoveCards(base.HeroTurnTakerController, criteria, (Card c) => base.HeroTurnTaker.Hand, numberOfCards: new int?(1), requiredDecisions: new int?(0), playIfMovingToPlayArea: false, storedResults: storedResults, cardSource: GetCardSource());
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