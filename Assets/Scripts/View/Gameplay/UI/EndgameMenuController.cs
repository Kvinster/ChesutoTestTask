using UnityEngine;
using UnityEngine.UI;

using Chesuto.Chess;
using Chesuto.Events;
using Chesuto.Manager;
using Chesuto.Starter;

using TMPro;

namespace Chesuto.Gameplay.View.UI {
    public sealed class EndgameMenuController : GameComponent {
        const string CheckText     = "Check!";
        const string CheckmateText = "Checkmate!";
        const string EndgameText   = "Game over!\nPlayer {0} wins!";
        
        public GameObject MenuRoot;
        public Button     SkipButton;
        public TMP_Text   MessageText;
        public GameObject ButtonRoot;
        public Button     Button;

        GameManager _gameManager;

        void OnDestroy() {
            EventManager.Unsubscribe<ChessCheck>(OnCheck);
            EventManager.Unsubscribe<ChessCheckmate>(OnCheckmate);
            EventManager.Unsubscribe<GameEnded>(OnGameEnded);
        }

        public override void Init(GameStarter gameStarter) {
            _gameManager = gameStarter.GameManager;
            
            MenuRoot.SetActive(false);
            ButtonRoot.SetActive(false);
            Button.onClick.RemoveAllListeners();
            
            SkipButton.onClick.AddListener(Hide);
            Button.onClick.AddListener(() => {
                _gameManager.StartGame(gameStarter.DeckPreset);
                Hide();
            });

            EventManager.Subscribe<ChessCheck>(OnCheck);
            EventManager.Subscribe<ChessCheckmate>(OnCheckmate);
            EventManager.Subscribe<GameEnded>(OnGameEnded);
        }

        void OnCheck(ChessCheck ev) {
            if ( IsHumanPlayer(ev.PlayerColor) ) {
                MessageText.text = CheckText;
                SkipButton.enabled = true;
                ButtonRoot.SetActive(false);
                Show();
            }
        }

        void OnCheckmate(ChessCheckmate ev) {
            if ( IsHumanPlayer(ev.PlayerColor) ) {
                MessageText.text = CheckmateText;
                SkipButton.enabled = true;
                ButtonRoot.SetActive(false);
                Show();
            }
        }

        void OnGameEnded(GameEnded ev) {
            MessageText.text = string.Format(EndgameText, ev.Winner.ToString());
            SkipButton.enabled = false;
            ButtonRoot.SetActive(true);
            Show();
        }

        bool IsHumanPlayer(ChessColor playerColor) {
            return (_gameManager.Game.GetPlayer(playerColor) is HumanPlayer);
        }
        
        void Show() {
            MenuRoot.SetActive(true);
        }

        void Hide() {
            MenuRoot.SetActive(false);
            ButtonRoot.SetActive(false);
        }
    }
}
