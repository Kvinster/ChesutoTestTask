using UnityEngine;
using UnityEngine.UI;

using Chesuto.Cards;
using Chesuto.Events;
using Chesuto.Manager;

using TMPro;

namespace Chesuto.Gameplay.View.UI {
    public sealed class CardView : MonoBehaviour {
        public Button   Button;
        public TMP_Text CardNameText;

        GameManager _gameManager;

        bool _isCommonInit;
        
        CardType _cardType = CardType.Unknown;

        void OnEnable() {
            EventManager.Subscribe<CardActivated>(OnCardActivated);
            EventManager.Subscribe<PlayerEffectEnded>(OnPlayerEffectEnded);
            EventManager.Subscribe<ChessFigureMoved>(OnChessFigureMoved);
            EventManager.Subscribe<GameActionsLeftChanged>(OnGameActionsLeftChanged);
        }

        void OnDisable() {
            EventManager.Unsubscribe<CardActivated>(OnCardActivated);
            EventManager.Unsubscribe<PlayerEffectEnded>(OnPlayerEffectEnded);
            EventManager.Unsubscribe<ChessFigureMoved>(OnChessFigureMoved);
            EventManager.Unsubscribe<GameActionsLeftChanged>(OnGameActionsLeftChanged);
        }

        public void CommonInit(GameManager gameManager) {
            _gameManager = gameManager;

            Button.onClick.AddListener(OnClick);

            _isCommonInit = true;
        }

        public void Init(CardType cardType) {
            if ( !_isCommonInit ) {
                Debug.LogError("CardView must be CommonInit before Init!");
                return;
            }
            
            _cardType = cardType;

            CardNameText.text = _cardType.ToString();

            UpdateButton();
        }

        void OnCardActivated(CardActivated ev) {
            UpdateButton();
        }

        void OnPlayerEffectEnded(PlayerEffectEnded ev) {
            UpdateButton();
        }

        void OnChessFigureMoved(ChessFigureMoved ev) {
            UpdateButton();
        }

        void OnGameActionsLeftChanged(GameActionsLeftChanged ev) {
            UpdateButton();
        }

        void UpdateButton() {
            if ( !_isCommonInit ) {
                return;
            }
            Button.interactable = _gameManager.CanActivateCard(_cardType);
        }

        void OnClick() {
            _gameManager.TryActivateCard(_cardType);
        }
    }
}
