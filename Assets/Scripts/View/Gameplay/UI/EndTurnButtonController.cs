using UnityEngine;
using UnityEngine.UI;

using Chesuto.Chess;
using Chesuto.Events;
using Chesuto.Manager;
using Chesuto.Starter;

namespace Chesuto.Gameplay.View.UI {
    public sealed class EndTurnButtonController : GameComponent {
        public GameObject ButtonRoot;
        public Button     Button;

        GameManager _gameManager;

        bool _useAction;
        bool _overrideShow;
        
        void OnDestroy() {
            if ( _gameManager != null ) {
                _gameManager.StateChangedEvent -= OnStateChanged;
            }
            
            EventManager.Unsubscribe<GameStarted>(OnGameStarted);
            EventManager.Unsubscribe<GameEnded>(OnGameEnded);
            EventManager.Unsubscribe<TurnStarted>(OnTurnStarted);
            EventManager.Unsubscribe<TurnEnded>(OnTurnEnded);
            EventManager.Unsubscribe<ClaymoreOfRush_SecondMoveStarted>(OnClaymoreOfRush_SecondMoveStarted);
            EventManager.Unsubscribe<CruelCrusade_SecondMoveStarted>(OnCruelCrusade_SecondMoveStarted);
        }

        public override void Init(GameStarter gameStarter) {
            _gameManager = gameStarter.GameManager;
            _gameManager.StateChangedEvent += OnStateChanged;
            
            ButtonRoot.SetActive(false);
            Button.onClick.AddListener(OnClick);

            EventManager.Subscribe<GameStarted>(OnGameStarted);
            EventManager.Subscribe<GameEnded>(OnGameEnded);
            EventManager.Subscribe<TurnStarted>(OnTurnStarted);
            EventManager.Subscribe<TurnEnded>(OnTurnEnded);
            EventManager.Subscribe<ClaymoreOfRush_SecondMoveStarted>(OnClaymoreOfRush_SecondMoveStarted);
            EventManager.Subscribe<CruelCrusade_SecondMoveStarted>(OnCruelCrusade_SecondMoveStarted);
        }

        void OnStateChanged(GameManager.State newState) {
            switch ( newState ) {
                case GameManager.State.Idle:
                case GameManager.State.CellSelected:
                case GameManager.State.SelectingCell_Recruitment:
                case GameManager.State.SelectingPawn_ClaymoreOfRush: 
                case GameManager.State.PromotingPawn:{
                    UpdateButton();
                    break;
                }
                default: {
                    Debug.LogErrorFormat("Unsupported state '{0}'", newState.ToString());
                    break;
                }
            }
        }

        void OnGameStarted(GameStarted ev) {
            UpdateButton();
        }

        void OnGameEnded(GameEnded ev) {
            ButtonRoot.SetActive(false);
        }

        void OnTurnStarted(TurnStarted ev) {
            UpdateButton();
        }

        void OnTurnEnded(TurnEnded ev) {
            _useAction    = false;
            _overrideShow = false;
            
            UpdateButton();
        }

        void OnClaymoreOfRush_SecondMoveStarted(ClaymoreOfRush_SecondMoveStarted ev) {
            if ( _gameManager.Game.CurPlayer is HumanPlayer ) {
                _useAction    = true;
                _overrideShow = true;
            }
            UpdateButton();
        }

        void OnCruelCrusade_SecondMoveStarted(CruelCrusade_SecondMoveStarted ev) {
            if ( _gameManager.Game.CurPlayer is HumanPlayer ) {
                _useAction    = true;
                _overrideShow = true;
            }
            UpdateButton();
        }

        void UpdateButton() {
            if ( _gameManager.Game.CurPlayer is HumanPlayer ) {
                ButtonRoot.SetActive(true);
            }
            Button.interactable = _gameManager.Game.CanEndTurn() || _overrideShow;
        }

        void OnClick() {
            _gameManager.TryEndTurn(_useAction);
        }
    }
}
