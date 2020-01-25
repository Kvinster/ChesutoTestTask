using System.Collections.Generic;

using Random = UnityEngine.Random;

namespace Chesuto.Cards {
    public class Deck {
        readonly List<GenericCard> _cards = new List<GenericCard>();

        public bool IsEmpty => (_cards.Count == 0);

        public GenericCard DrawCard() {
            if ( IsEmpty ) {
                return null;
            }
            var card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }
        
        public static Deck FromPreset(DeckPreset deckPreset) {
            var deck = new Deck();
            foreach ( var cardsPack in deckPreset.CardPacks ) {
                for ( var i = 0; i < cardsPack.CardsAmount; ++i ) {
                    deck._cards.Insert(Random.Range(0, deck._cards.Count), CardFactory.FromType(cardsPack.CardType));
                }
            }
            return deck;
        }
    }
}
