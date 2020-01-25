using UnityEngine;
using UnityEngine.UI;

using Chesuto.Manager;
using Chesuto.Starter;

namespace Chesuto.View.UI {
    public sealed class CancelCardButtonController : GameComponent {
        public GameObject ButtonRoot;
        public Button     Button;
        
        GameManager _gameManager;

        void OnDestroy() {
            if ( _gameManager != null ) {
                _gameManager.StateChangedEvent -= OnStateChanged;
            }
        }

        public override void Init(GameStarter gameStarter) {
            _gameManager = gameStarter.GameManager;
            _gameManager.StateChangedEvent += OnStateChanged;

            Button.onClick.AddListener(_gameManager.ResetState);
            ButtonRoot.SetActive(false);
        }

        void OnStateChanged(GameManager.State newState) {
            switch ( newState ) { 
                case GameManager.State.SelectingCell_Recruitment:
                case GameManager.State.SelectingPawn_ClaymoreOfRush: {
                    ButtonRoot.SetActive(true);
                    break;
                }
                default: {
                    ButtonRoot.SetActive(false);
                    break;
                }
            }
        }
    }
}
