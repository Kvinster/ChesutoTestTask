using Chesuto.Chess;

namespace Chesuto.Events {
    public struct PlayerEffectStarted {
        public readonly GenericPlayer       Player;
        public readonly GenericPlayerEffect PlayerEffect;

        public PlayerEffectStarted(GenericPlayer player, GenericPlayerEffect playerEffect) {
            Player       = player;
            PlayerEffect = playerEffect;
        }
    }
}
