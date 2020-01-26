using System.Collections.Generic;

using Chesuto.Chess;
using Chesuto.Events;

namespace Chesuto.Cards {
    public sealed class CruelCrusadeEffect : GenericPlayerEffect {
        const int EffectDuration = 7;
        const int PlayerHandSize = 3;

        int _initialTurnCount;

        bool _usedEffect;
        
        public override PlayerEffectType Type => PlayerEffectType.CruelCrusade;

        protected override void EndEffect() {
            Player.SetHandSize(Hand.MaxHandSize);
            base.EndEffect();

            EventManager.Unsubscribe<TurnStarted>(OnTurnStarted);
            EventManager.Unsubscribe<TurnEnded>(OnTurnEnded);
            EventManager.Unsubscribe<CardActivated>(OnCardActivated);
        }

        protected override void Init() {
            _initialTurnCount = Player.TurnCount;
            Player.SetHandSize(PlayerHandSize);

            EventManager.Subscribe<TurnStarted>(OnTurnStarted);
            EventManager.Subscribe<TurnEnded>(OnTurnEnded);
            EventManager.Subscribe<CardActivated>(OnCardActivated);
        }

        void OnTurnStarted(TurnStarted ev) {
            if ( ev.PlayerColor != Player.Color ) {
                return;
            }
            _usedEffect = false;
            if ( Player.TurnCount > _initialTurnCount + EffectDuration ) {
                EndEffect();
            }
        }

        void OnTurnEnded(TurnEnded ev) {
            if ( ev.PlayerColor != Player.Color ) {
                return;
            }
            Player.DrawCard();
            _usedEffect = false;
        }

        void OnCardActivated(CardActivated ev) {
            if ( (Game.CurPlayer != Player) || _usedEffect ) {
                return;
            }
            Game.AddActions(1);
            _usedEffect = true;
            EventManager.Fire(new CruelCrusade_SecondMoveStarted());
        }
    }
}
