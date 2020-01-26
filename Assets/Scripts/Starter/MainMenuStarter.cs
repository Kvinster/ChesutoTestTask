using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Chesuto.Controller;
using Chesuto.View.MainMenu.DeckBuilder;

namespace Chesuto.Starter {
    public sealed class MainMenuStarter : MonoBehaviour {
        public DeckBuilder DeckBuilder;
        public Button      StartGameButton;

        void Start() {
            DeckBuilder.Init();

            StartGameButton.onClick.AddListener(OnStartGameButtonClick);
        }

        void OnStartGameButtonClick() {
            var deck = DeckBuilder.BuildDeck();
            if ( deck != null ) {
                GameController.Instance.Deck = deck;
            }
            SceneManager.LoadScene("Gameplay");
        }
    }
}
