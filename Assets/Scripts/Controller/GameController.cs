using Chesuto.Cards;

namespace Chesuto.Controller {
    public sealed class GameController {
        static GameController _instance;

        public static GameController Instance => _instance ?? (_instance = new GameController());
        
        public Deck Deck;
    }
}
