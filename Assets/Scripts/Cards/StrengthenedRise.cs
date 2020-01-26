using Chesuto.Chess;

namespace Chesuto.Cards {
    public sealed class StrengthenedRise : GenericCard {
        public override CardType Type => CardType.StrengthenedRise;

        public override bool TryActivate(Game game, GenericPlayer player, out GenericPlayerEffect playerEffect) {
            playerEffect = new StrengthenedRiseEffect();
            return true;
        }

        public override bool CanActivate(Game game, GenericPlayer player) {
            foreach ( var effect in player.Effects ) {
                if ( effect.Type == PlayerEffectType.StrengthenedRise ) {
                    return false;
                }
            }
            return true;
        }
    }
}
