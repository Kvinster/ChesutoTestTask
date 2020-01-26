using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;

using Chesuto.Cards;
using Chesuto.Controller;

namespace Chesuto.View.MainMenu.DeckBuilder {
    public sealed class DeckBuilder : MonoBehaviour {
        public DeckPresetSerializable DefaultDeck;
        public List<CardPackView>     CardPackViews;

        public void Init() {
            var values = Enum.GetValues(typeof(CardType)).Cast<CardType>().ToList();
            values.Remove(CardType.Unknown);
            Debug.Assert(CardPackViews.Count >= values.Count);
            var cardPacks = (GameController.Instance.Deck == null)
                ? DefaultDeck.DeckPreset.CardPacks
                : GameController.Instance.Deck.GetCardPacks();
            int i;
            for ( i = 0; i < values.Count; ++i ) {
                var cardType = values[i];
                var cardPackView = CardPackViews[i];
                var amount = 0;
                foreach ( var cardPack in cardPacks ) {
                    if ( cardPack.CardType == cardType ) {
                        amount = cardPack.CardsAmount;
                        break;
                    } 
                }
                cardPackView.Init(cardType, amount);
                cardPackView.gameObject.SetActive(true);
            }
            for ( ; i < CardPackViews.Count; ++i ) {
                CardPackViews[i].gameObject.SetActive(false);
            }
        }

        public Deck BuildDeck() {
            var cardPacks = new List<CardPack>();
            foreach ( var cardPackView in CardPackViews ) {
                var cardPack = cardPackView.BuildCardPack();
                if ( cardPack != null ) {
                    cardPacks.Add(cardPack);
                }
            }
            return Deck.FromCardPacks(cardPacks);
        }
    }
}
