using Chesuto.Cards;

namespace Chesuto.Events {
    public struct CardActivated {
        public readonly GenericCard Card;

        public CardActivated(GenericCard card) {
            Card = card;
        }
    }
}
