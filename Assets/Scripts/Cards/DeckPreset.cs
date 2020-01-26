using System;
using System.Collections.Generic;

namespace Chesuto.Cards {
    [Serializable]
    public sealed class DeckPreset {
        public List<CardPack> CardPacks = new List<CardPack>();
    }
}
