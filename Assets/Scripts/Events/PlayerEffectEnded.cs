using Chesuto.Chess;

namespace Chesuto.Events {
    public struct PlayerEffectEnded {
        public readonly GenericPlayer       Player;
        public readonly GenericPlayerEffect PlayerEffect;

        public PlayerEffectEnded(GenericPlayer player, GenericPlayerEffect playerEffect) {
            Player       = player;
            PlayerEffect = playerEffect;
        }
    }
}
