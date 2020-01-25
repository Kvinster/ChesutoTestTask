using System;
using UnityEngine;

using System.Collections.Generic;

namespace Chesuto.Utils {
    public class UnityContext : MonoBehaviour {
        static UnityContext _instance = null;

        static UnityContext Instance {
            get {
                if ( _instance == null ) {
                    var instanceGO = new GameObject("[UnityContext]");
                    _instance = instanceGO.AddComponent<UnityContext>();
                }
                return _instance;
            }
        }

        readonly List<Action> _actions  = new List<Action>();
        readonly List<Action> _toAdd    = new List<Action>(1);
        readonly List<Action> _toRemove = new List<Action>(1);

        bool _isUpdating;

        void Update() {
            _isUpdating = true;
            foreach ( var action in _actions ) {
                action?.Invoke();
            }
            foreach ( var action in _toAdd ) {
                _actions.Add(action);
            }
            _toAdd.Clear();
            foreach ( var action in _toRemove ) {
                _actions.Remove(action);
            }
            _toRemove.Clear();
            _isUpdating = false;
        }

        void AddUpdateCallbackInternal(Action action) {
            if ( _isUpdating ) {
                _toAdd.Add(action);
            } else {
                _actions.Add(action);
            }
        }

        void RemoveUpdateCallbackInternal(Action action) {
            if ( _isUpdating ) {
                _toRemove.Add(action);
            } else {
                _actions.Remove(action);
            }
        }

        public static void AddUpdateCallback(Action action) {
            if ( action == null ) {
                return;
            }
            Instance.AddUpdateCallbackInternal(action);
        }

        public static void RemoveUpdateCallback(Action action) {
            if ( action == null ) {
                return;
            }
            Instance.RemoveUpdateCallbackInternal(action);
        }
    }
}
