using System.Collections.Generic;

using Chesuto.Cards;
using Chesuto.Events;

namespace Chesuto.Chess {
    public abstract class GenericPlayer {
        public readonly ChessColor                Color;
        public readonly Deck                      Deck;
        public readonly Hand                      Hand    = new Hand();
        public readonly List<GenericPlayerEffect> Effects = new List<GenericPlayerEffect>();
        
        protected readonly Game Game;
        
        public int TurnCount { get; private set; }

        protected GenericPlayer(ChessColor color, Deck deck, Game game) {
            Color = color;
            Deck  = deck;

            Game = game;
        }
        
        public virtual void StartTurn() {
            ++TurnCount;
        }

        public virtual bool CanEndTurn() {
            foreach ( var effect in Effects ) {
                if ( !effect.CanEndTurn() ) {
                    return false;
                }
            }
            return true;
        }

        public virtual void EndTurn() {
            if ( TurnCount % 2 == 1 ) {
                DrawCard();
            }
        }

        public void DrawCard() {
            var card = Deck.DrawCard();
            if ( card != null ) {
                Hand.TryAddCard(card);
            }
        }

        public bool CanActivateCard(CardType cardType) {
            var res = Hand.CanActivateCard(cardType, Game, this);
            if ( res ) {
                foreach ( var effect in Effects ) {
                    if ( !effect.CanActivateCard(cardType) ) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool TryActivateCard(CardType cardType) {
            var res = Hand.TryActivateCard(cardType, Game, this, out var playerEffect);
            if ( res && (playerEffect != null) ) {
                AddEffect(playerEffect);
            }
            return res;
        }

        public void EndEffect(GenericPlayerEffect effect) {
            Effects.Remove(effect);
            EventManager.Fire(new PlayerEffectEnded(this, effect));
        }

        public void FilterAvailableTurns(Dictionary<Figure, List<ChessCoords>> availableTurns) {
            foreach ( var effect in Effects ) {
                effect.FilterAvailableTurns(availableTurns);
            }
        }

        void AddEffect(GenericPlayerEffect effect) {
            effect.Init(Game, this);
            Effects.Add(effect);
            EventManager.Fire(new PlayerEffectStarted(this, effect));
        }
    }
}
