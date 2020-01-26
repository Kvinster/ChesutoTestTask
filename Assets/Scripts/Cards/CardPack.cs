using System;

namespace Chesuto.Cards {
    [Serializable]
    public sealed class CardPack {
        public CardType CardType;
        public int      CardsAmount;

        public CardPack(CardType cardType, int cardsAmount) {
            CardType    = cardType;
            CardsAmount = cardsAmount;
        }
    }
}
