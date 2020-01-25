using System;
using System.Collections.Generic;

namespace Chesuto.Events {
    public abstract class GenericHandler {
    }

    public sealed class Handler<T> : GenericHandler where T : struct {
        readonly List<Action<T>> _actions = new List<Action<T>>();
        
        public void Add(Action<T> action) {
            if ( action == null ) {
                return;
            }
            _actions.Add(action);
        }

        public void Remove(Action<T> action) {
            _actions.Remove(action);
        }

        public void Fire(T ev) {
            // TODO: get rid of this cloning, but account for concurrent modification problem
            var actions = new List<Action<T>>(_actions);
            foreach ( var action in actions ) {
                action.Invoke(ev);
            }
        }
    }
}
