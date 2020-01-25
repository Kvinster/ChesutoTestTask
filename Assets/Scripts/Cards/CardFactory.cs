using UnityEngine;

namespace Chesuto.Cards {
    public static class CardFactory {
        public static GenericCard FromType(CardType cardType) {
            switch ( cardType ) {
                case CardType.StrengthenedRise: return new StrengthenedRise();
                case CardType.Recruitment:      return new Recruitment();
                case CardType.ClaymoreOfRush:   return new ClaymoreOfRush();
                default: {
                    Debug.LogErrorFormat("Unsupported card type '{0}'", cardType.ToString());
                    return null;
                }
            }
        }
    }
}
