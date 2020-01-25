using System.Collections.Generic;

using Chesuto.Cards;

namespace Chesuto.Chess {
    public abstract class GenericPlayerEffect {
        public abstract PlayerEffectType Type { get; }

        protected Game          Game;
        protected GenericPlayer Player;

        protected GenericPlayerEffect() {
        }

        public void Init(Game game, GenericPlayer player) {
            Game   = game;
            Player = player;

            Init();
        }

        public virtual bool CanEndTurn() {
            return true;
        }

        public virtual bool CanActivateCard(CardType cardType) {
            return true;
        }

        protected abstract void Init();

        protected virtual void EndEffect() {
            Player.EndEffect(this);
        }

        public virtual void FilterAvailableTurns(Dictionary<Figure, List<ChessCoords>> availableTurns) { }
    }
}
