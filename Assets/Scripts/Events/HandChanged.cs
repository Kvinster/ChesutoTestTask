using Chesuto.Cards;

namespace Chesuto.Events {
    public struct HandChanged {
        public readonly Hand Hand;

        public HandChanged(Hand hand) {
            Hand = hand;
        }
    }
}
