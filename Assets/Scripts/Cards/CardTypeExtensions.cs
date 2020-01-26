using System.Collections.Generic;

namespace Chesuto.Cards {
    public static class CardTypeExtensions {
        static readonly Dictionary<CardType, string> Readables = new Dictionary<CardType, string> {
            { CardType.Recruitment     , "Recruitment" },
            { CardType.StrengthenedRise, "Strengthened Rise" },
            { CardType.ClaymoreOfRush  , "Claymore of Rush" },
            { CardType.CruelCrusade    , "Cruel Crusade" }
        };
        
        public static string ToReadableString(this CardType cardType) {
            return Readables.TryGetValue(cardType, out var readable) ? readable : cardType.ToString();
        }
    }
}
