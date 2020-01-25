namespace Chesuto.Events {
    public struct GameActionsLeftChanged {
        public readonly int ActionsLeft;

        public GameActionsLeftChanged(int actionsLeft) {
            ActionsLeft = actionsLeft;
        }
    }
}
