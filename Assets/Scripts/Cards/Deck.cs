using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;

namespace Chesuto.Cards {
    public class Deck {
        readonly List<GenericCard> _cards = new List<GenericCard>();

        bool IsEmpty => (_cards.Count == 0);

        public GenericCard DrawCard() {
            if ( IsEmpty ) {
                return null;
            }
            var card = _cards[0];
            _cards.RemoveAt(0);
            return card;
        }

        public List<CardPack> GetCardPacks() {
            var packs = new Dictionary<CardType, CardPack>();
            foreach ( var card in _cards ) {
                if ( !packs.TryGetValue(card.Type, out var cardPack) ) {
                    cardPack = new CardPack(card.Type, 0);
                    packs.Add(card.Type, cardPack);
                }
                ++cardPack.CardsAmount;
            }
            return packs.Values.ToList();
        }

        public static Deck Clone(Deck original) {
            if ( original == null ) {
                return null;
            }
            var deck = new Deck();
            foreach ( var card in original._cards ) {
                deck._cards.Add(CardFactory.FromType(card.Type));
            }
            return deck;
        }
        
        public static Deck FromPreset(DeckPreset deckPreset) {
            return FromCardPacks(deckPreset.CardPacks);
        }

        public static Deck FromCardPacks(List<CardPack> cardPacks) {
            var deck = new Deck();
            foreach ( var cardsPack in cardPacks ) {
                for ( var i = 0; i < cardsPack.CardsAmount; ++i ) {
                    deck._cards.Insert(Random.Range(0, deck._cards.Count), CardFactory.FromType(cardsPack.CardType));
                }
            }
            return deck;
        }
    }
}
