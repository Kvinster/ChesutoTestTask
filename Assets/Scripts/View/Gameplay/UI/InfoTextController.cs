using System.Collections.Generic;
using System.Text;

using Chesuto.Cards;
using Chesuto.Chess;
using Chesuto.Events;
using Chesuto.Manager;
using Chesuto.Starter;

using TMPro;

namespace Chesuto.Gameplay.View.UI {
    public sealed class InfoTextController : GameComponent {
        const string CurTurnTextTemplate   = "Current Turn: {0}";
        const string TurnCountTextTemplate = "Turn count: {0}";
        const string EffectsTitle          = "Effects:";
        
        public TMP_Text CurTurnText;
        public TMP_Text TurnCountText;
        public TMP_Text EffectsText;

        GameManager _gameManager;
        
        readonly StringBuilder _stringBuilder = new StringBuilder();

        void OnDestroy() {
            EventManager.Unsubscribe<TurnStarted>(OnTurnStarted);
            EventManager.Unsubscribe<PlayerEffectStarted>(OnPlayerEffectStarted);
            EventManager.Unsubscribe<PlayerEffectEnded>(OnPlayerEffectEnded);
        }

        public override void Init(GameStarter gameStarter) {
            _gameManager = gameStarter.GameManager;

            EventManager.Subscribe<TurnStarted>(OnTurnStarted);
            EventManager.Subscribe<PlayerEffectStarted>(OnPlayerEffectStarted);
            EventManager.Subscribe<PlayerEffectEnded>(OnPlayerEffectEnded);
        }

        void UpdateText() {
            CurTurnText.text   = string.Format(CurTurnTextTemplate, _gameManager.Game.CurPlayer.Color.ToString());
            TurnCountText.text = string.Format(TurnCountTextTemplate, _gameManager.Game.CurPlayer.TurnCount);
            UpdateEffectsText();
        }

        void UpdateEffectsText() {
            _stringBuilder.Clear();
            var list = _gameManager.Game.CurPlayer.Effects;
            if ( list.Count == 0 ) {
                EffectsText.text = string.Empty;
            } else {
                _stringBuilder.AppendLine(EffectsTitle);
                foreach ( var effect in list ) {
                    _stringBuilder.AppendLine(effect.Type.ToString());
                }
                EffectsText.text = _stringBuilder.ToString();
                _stringBuilder.Clear();
            }
        }

        void OnTurnStarted(TurnStarted ev) {
            UpdateText();
        }

        void OnPlayerEffectStarted(PlayerEffectStarted ev) {
            if ( _gameManager.Game.CurPlayer == ev.Player ) {
                UpdateEffectsText();
            }
        }

        void OnPlayerEffectEnded(PlayerEffectEnded ev) {
            if ( _gameManager.Game.CurPlayer == ev.Player ) {
                UpdateEffectsText();
            }
        }
    }
}
