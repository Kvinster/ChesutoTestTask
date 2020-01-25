using Chesuto.Chess;

using UnityEngine;
using UnityEngine.UI;

using Chesuto.Events;
using Chesuto.Manager;
using Chesuto.Starter;

namespace Chesuto.View.UI {
    public sealed class EndTurnButtonController : GameComponent {
        public GameObject ButtonRoot;
        public Button     Button;

        GameManager _gameManager;

        bool _useAction;
        
        void OnDestroy() {
            EventManager.Unsubscribe<TurnStarted>(OnTurnStarted);
            EventManager.Unsubscribe<TurnEnded>(OnTurnEnded);
            EventManager.Unsubscribe<ClaymoreOfRush_SecondMoveStarted>(OnClaymoreOfRush_SecondMoveStarted);
            EventManager.Unsubscribe<GameStarted>(OnGameStarted);
            EventManager.Unsubscribe<GameEnded>(OnGameEnded);
        }

        public override void Init(GameStarter gameStarter) {
            _gameManager = gameStarter.GameManager;
            
            ButtonRoot.SetActive(false);
            Button.onClick.AddListener(OnClick);

            EventManager.Subscribe<TurnStarted>(OnTurnStarted);
            EventManager.Subscribe<ClaymoreOfRush_SecondMoveStarted>(OnClaymoreOfRush_SecondMoveStarted);
            EventManager.Subscribe<GameStarted>(OnGameStarted);
            EventManager.Subscribe<GameEnded>(OnGameEnded);
        }

        void OnTurnStarted(TurnStarted ev) {
            if ( _gameManager.Game.CurPlayer is HumanPlayer ) {
                ButtonRoot.SetActive(true);
                Button.interactable = _gameManager.Game.CanEndTurn();
            } else {
                ButtonRoot.SetActive(false);
            }
        }

        void OnTurnEnded(TurnEnded ev) {
            _useAction = false;

            EventManager.Unsubscribe<TurnEnded>(OnTurnEnded);
        }

        void OnClaymoreOfRush_SecondMoveStarted(ClaymoreOfRush_SecondMoveStarted ev) {
            if ( _gameManager.Game.CurPlayer is HumanPlayer ) {
                _useAction = true;
                
                ButtonRoot.SetActive(true);
                Button.interactable = true;

                EventManager.Subscribe<TurnEnded>(OnTurnEnded);
            } else {
                ButtonRoot.SetActive(false);
            }
        }

        void OnGameEnded(GameEnded ev) {
            ButtonRoot.SetActive(false);
        }

        void OnGameStarted(GameStarted ev) {
            ButtonRoot.SetActive(true);
        }

        void OnClick() {
            _gameManager.TryEndTurn(_useAction);
        }
    }
}
