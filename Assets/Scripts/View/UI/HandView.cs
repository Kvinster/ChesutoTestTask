using UnityEngine;

using System.Collections.Generic;

using Chesuto.Cards;
using Chesuto.Chess;
using Chesuto.Events;
using Chesuto.Manager;
using Chesuto.Starter;

namespace Chesuto.View.UI {
    public sealed class HandView : GameComponent {
        public GameObject     CardsRoot;
        public CanvasGroup    CanvasGroup;
        public List<CardView> CardViews;

        GameManager   _gameManager;
        GenericPlayer _player;

        bool _isCommonInit;
        bool _isActive;

        Hand Hand => _player?.Hand;

        void Reset() {
            CardViews = new List<CardView>();
            GetComponentsInChildren(CardViews);
        }

        void OnDestroy() {
            EventManager.Unsubscribe<HandChanged>(OnHandChanged);
            EventManager.Unsubscribe<TurnStarted>(OnTurnStarted);
            EventManager.Unsubscribe<GameStarted>(OnGameStarted);
            EventManager.Unsubscribe<GameEnded>(OnGameEnded);
            EventManager.Unsubscribe<GameActionsLeftChanged>(OnGameActionsLeftChanged);
        }

        public override void Init(GameStarter gameStarter) {
            _gameManager = gameStarter.GameManager;
            
            foreach ( var cardView in CardViews ) {
                cardView.CommonInit(_gameManager);
            }

            _isCommonInit = true;

            EventManager.Subscribe<HandChanged>(OnHandChanged);
            EventManager.Subscribe<TurnStarted>(OnTurnStarted);
            EventManager.Subscribe<GameStarted>(OnGameStarted);
            EventManager.Subscribe<GameEnded>(OnGameEnded);
            EventManager.Subscribe<GameActionsLeftChanged>(OnGameActionsLeftChanged);
        }

        void OnHandChanged(HandChanged ev) {
            if ( ev.Hand != Hand ) {
                return;
            }
            UpdateCardViews();
        }

        void OnTurnStarted(TurnStarted ev) {
            if ( !_isActive ) {
                return;
            }

            var isHuman = _gameManager.Game.CurPlayer is HumanPlayer;
            if ( isHuman ) {
                _player = _gameManager.Game.CurPlayer;
                UpdateCardViews();
                CardsRoot.SetActive(true);
                CanvasGroup.interactable = (_player.TurnCount > 3) && _gameManager.Game.HasActions;
            } else {
                CardsRoot.SetActive(false);
            }
        }

        void OnGameStarted(GameStarted ev) {
            if ( !_isCommonInit ) {
                Debug.LogError("HandView must be CommonInit before game start");
            }
            
            _player   = _gameManager.Game.GetPlayer(ChessColor.White);
            _isActive = true;
            
            UpdateCardViews();
            
            CanvasGroup.interactable = (_gameManager.Game.CurPlayer == _player);
        }

        void OnGameEnded(GameEnded ev) {
            if ( !_isActive ) {
                return;
            }
            
            _player   = null;
            _isActive = false;

            CanvasGroup.interactable = false;
        }

        void OnGameActionsLeftChanged(GameActionsLeftChanged ev) {
            if ( !_isActive ) {
                return;
            }

            UpdateCardViews();
        }

        void UpdateCardViews() {
            CanvasGroup.interactable = (_player.TurnCount > 3) && _gameManager.Game.HasActions;
            int i;
            for ( i = 0; i < Hand.HandSize; ++i ) {
                var cardView = CardViews[i];
                cardView.Init(Hand[i].Type);
                cardView.gameObject.SetActive(true);
            }
            for ( ; i < CardViews.Count; ++i ) {
                CardViews[i].gameObject.SetActive(false);
            }
        }
    }
}
