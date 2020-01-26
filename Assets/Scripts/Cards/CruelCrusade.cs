using Chesuto.Chess;

namespace Chesuto.Cards {
    public sealed class CruelCrusade : GenericCard {
        public override CardType Type => CardType.CruelCrusade;
        
        public override bool TryActivate(Game game, GenericPlayer player, out GenericPlayerEffect playerEffect) {
            playerEffect = new CruelCrusadeEffect();
            return true;
        }

        public override bool CanActivate(Game game, GenericPlayer player) {
            foreach ( var effect in player.Effects ) {
                if ( effect.Type == PlayerEffectType.CruelCrusade ) {
                    return false;
                }
            }
            return true;
        }
    }
}
