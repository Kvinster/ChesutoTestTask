using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Chesuto.Chess;
using Chesuto.Events;

namespace Chesuto.Cards {
    public sealed class Hand : IEnumerable<GenericCard> {
        const int MaxHandSize = 10;

        readonly List<GenericCard> _cards = new List<GenericCard>();

        public int HandSize => _cards.Count;

        public bool IsEmpty => (HandSize == 0);
        public bool IsFull  => (HandSize >= MaxHandSize);

        public GenericCard this[int i] {
            get {
                if ( (i < 0) || (i >= _cards.Count) ) {
                    Debug.LogErrorFormat("Invalid card index '{0}'", i);
                    return null;
                }
                return _cards[i];
            }
        }

        public bool TryAddCard(GenericCard card) {
            if ( _cards.Count == MaxHandSize ) {
                return false;
            }
            _cards.Add(card);
            EventManager.Fire(new HandChanged(this));
            return true;
        }

        public bool ContainsCard(CardType cardType) {
            foreach ( var card in _cards ) {
                if ( card.Type == cardType ) {
                    return true;
                }
            }
            return false;
        }

        public bool TryRemoveCard(CardType cardType) {
            for ( var i = 0; i < _cards.Count; ++i ) {
                var card = _cards[i];
                if ( card.Type == cardType ) {
                    _cards.RemoveAt(i);
                    EventManager.Fire(new HandChanged(this));
                    return true;
                }
            }
            return false;
        }

        public bool CanActivateCard(CardType cardType, Game game, GenericPlayer player) {
            foreach ( var card in _cards ) {
                if ( (card.Type == cardType) && card.CanActivate(game, player) ) {
                    return true;
                }
            }
            return false;
        }

        public bool TryActivateCard(CardType cardType, Game game, GenericPlayer player,
                                    out GenericPlayerEffect playerEffect) {
            for ( var i = 0; i < _cards.Count; ++i ) {
                var card = _cards[i];
                if ( (card.Type == cardType) && card.TryActivate(game, player, out playerEffect) &&
                     TryRemoveCard(cardType) ) {
                    EventManager.Fire(new CardActivated(card));
                    return true;
                }
            }
            playerEffect = null;
            return false;
        }

        IEnumerator<GenericCard> IEnumerable<GenericCard>.GetEnumerator() {
            return _cards.GetEnumerator();
        }

        public IEnumerator GetEnumerator() {
            return _cards.GetEnumerator();
        }
    }
}
