using Chesuto.Chess;

namespace Chesuto.Cards {
    public abstract class GenericCard {
        public abstract CardType Type { get; }

        public abstract bool TryActivate(Game game, GenericPlayer player, out GenericPlayerEffect playerEffect);
        public abstract bool CanActivate(Game game, GenericPlayer player);
    }
}
