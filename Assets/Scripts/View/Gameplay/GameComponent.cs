using UnityEngine;

using System.Collections.Generic;

using Chesuto.Starter;

namespace Chesuto.Gameplay.View {
    public abstract class GameComponent : MonoBehaviour {
        public static readonly HashSet<GameComponent> Instances = new HashSet<GameComponent>();
        
        protected void OnEnable() {
            Instances.Add(this);
        }

        protected void OnDisable() {
            Instances.Remove(this);
        }

        public abstract void Init(GameStarter gameStarter);
    }
}
