using UnityEngine;

using Chesuto.Cards;

using TMPro;

namespace Chesuto.View.MainMenu.DeckBuilder {
    public sealed class CardPackView : MonoBehaviour {
        public TMP_Text       CardTypeText;
        public TMP_InputField InputField;

        CardType _cardType    = CardType.Unknown;
        int      _cardsAmount = 0;

        public void Init(CardType cardType, int defaultAmount) {
            _cardType    = cardType;
            _cardsAmount = defaultAmount;
            
            CardTypeText.text = _cardType.ToReadableString();
            
            var defaultStr  = defaultAmount.ToString();
            InputField.text = defaultStr;

            InputField.onValueChanged.AddListener(OnInputFieldValueChanged);
        }

        public CardPack BuildCardPack() {
            return (_cardType == CardType.Unknown) ? null : new CardPack(_cardType, _cardsAmount);
        }

        void OnInputFieldValueChanged(string newValue) {
            if ( !int.TryParse(newValue, out var res) || (res < 0) ) {
                res = 0;
                InputField.text = "0";
            }
            _cardsAmount = res;
        }
    }
}
