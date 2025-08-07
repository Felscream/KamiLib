#region

using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using KamiLib.Caching;
using Lumina.Excel.Sheets;

#endregion

namespace KamiLib.GameState
{
    public static class Condition
    {
        public static bool IsBoundByDuty()
        {
            if (IsInIslandSanctuary()) return false;

            return Service.Condition[ConditionFlag.BoundByDuty] ||
                   Service.Condition[ConditionFlag.BoundByDuty56] ||
                   Service.Condition[ConditionFlag.BoundByDuty95];
        }

        public static bool IsInCombat()
        {
            return Service.Condition[ConditionFlag.InCombat];
        }
        public static bool IsInCutsceneOrQuestEvent()
        {
            return IsInCutscene() || IsInQuestEvent();
        }
        public static bool IsDutyRecorderPlayback()
        {
            return Service.Condition[ConditionFlag.DutyRecorderPlayback];
        }
        public static bool IsIslandDoingSomethingMode()
        {
            return Service.GameGui.GetAddonByName("MJIPadGuide") != nint.Zero;
        }

        public static bool IsInCutscene()
        {
            return Service.Condition[ConditionFlag.OccupiedInCutSceneEvent] ||
                   Service.Condition[ConditionFlag.WatchingCutscene] ||
                   Service.Condition[ConditionFlag.WatchingCutscene78];
        }

        public static bool IsInQuestEvent()
        {
            if (IsInIslandSanctuary() && IsIslandDoingSomethingMode()) return false;

            return Service.Condition[ConditionFlag.OccupiedInQuestEvent];
        }

        public static bool IsBetweenAreas()
        {
            return Service.Condition[ConditionFlag.BetweenAreas] ||
                   Service.Condition[ConditionFlag.BetweenAreas51];
        }

        public static bool IsInIslandSanctuary()
        {
            var territoryInfo = LuminaCache<TerritoryType>.Instance.GetRow(Service.ClientState.TerritoryType);
            if (territoryInfo is null) return false;

            // Island Sanctuary
            return territoryInfo.Value.TerritoryIntendedUse.RowId == 49;
        }

        public static bool IsCrafting()
        {
            return Service.Condition[ConditionFlag.Crafting] ||
                   Service.Condition[ConditionFlag.ExecutingCraftingAction];
        }

        public static bool IsCrossWorld()
        {
            return Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];
        }

        public static unsafe bool IsInSanctuary()
        {
            return TerritoryInfo.Instance()->InSanctuary;
        }

        public static bool CheckFlag(ConditionFlag flag)
        {
            return Service.Condition[flag];
        }

        public static bool IsGathering()
        {
            return Service.Condition[ConditionFlag.Gathering] ||
                   Service.Condition[ConditionFlag.ExecutingGatheringAction];
        }

        public static bool IsInBardPerformance()
        {
            return Service.Condition[ConditionFlag.Performing];
        }
    }
}
