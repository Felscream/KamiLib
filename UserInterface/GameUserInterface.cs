#region

using Dalamud.Plugin.Services;
using System;

#endregion

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace KamiLib.UserInterface
{
    public class GameUserInterface : IDisposable
    {

        private static GameUserInterface? _instance;
        private bool lastState;

        private GameUserInterface()
        {
            Service.Framework.Update += FrameworkUpdate;
        }
        public static GameUserInterface Instance => _instance ??= new GameUserInterface();

        public bool IsVisible => !lastState;

        public void Dispose()
        {
            Service.Framework.Update -= FrameworkUpdate;
        }
        public event EventHandler? UiHidden;
        public event EventHandler? UiShown;

        public static void Cleanup()
        {
            _instance?.Dispose();
        }

        private void FrameworkUpdate(IFramework framework)
        {
            var partyList = Service.GameGui.GetAddonByName("_PartyList");
            var todoList = Service.GameGui.GetAddonByName("_ToDoList");
            var enemyList = Service.GameGui.GetAddonByName("_EnemyList");

            var partyListVisible = partyList != null && partyList.IsVisible;
            var todoListVisible = todoList != null && todoList.IsVisible;
            var enemyListVisible = enemyList != null && enemyList.IsVisible;

            var shouldHideUi = !partyListVisible && !todoListVisible && !enemyListVisible;

            if (lastState != shouldHideUi)
            {
                if (shouldHideUi)
                {
                    UiHidden?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    UiShown?.Invoke(this, EventArgs.Empty);
                }
            }

            lastState = shouldHideUi;
        }
    }
}
