using UnityEngine;

namespace Chesuto.Cards {
    [CreateAssetMenu(menuName = "Create Deck Preset", fileName = "Deck")]
    public class DeckPresetSerializable : ScriptableObject {
        const int DeckSize = 40;

        public DeckPreset DeckPreset = new DeckPreset();

        void OnValidate() {
            var cardsCount = 0;
            foreach ( var cardPack in DeckPreset.CardPacks ) {
                cardsCount += cardPack.CardsAmount;
            }
            if ( cardsCount != DeckSize ) {
                Debug.LogErrorFormat(this, "Invalid deck size '{0}'", cardsCount);
            }
        }
    }
}
