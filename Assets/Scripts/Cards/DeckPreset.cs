using UnityEngine;

using System;
using System.Collections.Generic;

namespace Chesuto.Cards {
    [CreateAssetMenu(menuName = "Create DeckPreset", fileName = "Deck")]
    public sealed class DeckPreset : ScriptableObject {
        [Serializable]
        public sealed class CardPack {
            public CardType CardType;
            public int      CardsAmount;

            public CardPack(CardType cardType, int cardsAmount) {
                CardType    = cardType;
                CardsAmount = cardsAmount;
            }
        }

        const int DeckSize = 30;

        public List<CardPack> CardPacks;

        void OnValidate() {
            var cardsCount = 0;
            foreach ( var cardPack in CardPacks ) {
                cardsCount += cardPack.CardsAmount;
            }
            if ( cardsCount != DeckSize ) {
                Debug.LogErrorFormat(this, "Invalid deck size '{0}'", cardsCount);
            }
        }
    }
}
