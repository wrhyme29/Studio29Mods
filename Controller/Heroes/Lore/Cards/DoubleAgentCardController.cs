using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.Lore
{
    public class DoubleAgentCardController : LoreCardController
    {

        public DoubleAgentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{Lore} may deal one non-character villain target 2 psychic damage. 

            List<DealDamageAction> storedResults = new List<DealDamageAction>() ;
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Psychic, 1, false, 0, additionalCriteria: (Card c) => c.IsVillainTarget && !c.IsCharacter && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedResultsDamage: storedResults, cardSource: GetCardSource()); 
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //A target dealt damage that way now deals a character card in its play area 2 irreducible melee damage.
            if(DidDealDamage(storedResults) && !storedResults.First().DidDestroyTarget)
            {
                Card target = storedResults.First().Target;
                IEnumerable<Card> characterTargetOptions = target.Location.Cards.Where(c => c.IsCharacter && c.IsTarget);
                if(!characterTargetOptions.Any())
                {
                    coroutine = GameController.SendMessageAction($"There are no character card targets in the play area that {target.Title} is in!", Priority.Medium, GetCardSource(), showCardSource: true);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    yield break;
                }
                Card characterTarget = characterTargetOptions.First();
                if(characterTargetOptions.Count() > 1)
                {
                    //multiple options allow player to select

                    List<SelectCardDecision> storedCharacterChoice = new List<SelectCardDecision>();
                    coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DealDamage, characterTargetOptions, storedCharacterChoice, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    if(!DidSelectCard(storedCharacterChoice))
                    {
                        yield break;
                    }

                    characterTarget = GetSelectedCard(storedCharacterChoice);
                }

                coroutine = DealDamage(target, characterTarget, 2, DamageType.Melee, isIrreducible: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

            }
            yield break;
        }

        
    }
}