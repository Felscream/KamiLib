using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;

namespace KamiLib.Extensions;

public static class PlayerCharacterExtensions
{
    public static bool HasStatus(this IPlayerCharacter character, uint statusId)
    {
        return character.StatusList.Any(status => status.StatusId == statusId);
    }

    public static bool HasStatus(this IPlayerCharacter character, List<uint> statusList)
    {
        return character.StatusList.Any(status => statusList.Contains(status.StatusId));
    }

    public static bool HasOnlineStatus(this IPlayerCharacter character, uint statusId)
    {
        return character.OnlineStatus.RowId == statusId;
    }

    public static int StatusCount(this IPlayerCharacter character, List<uint> statusList)
    {
        return character.StatusList.Count(status => statusList.Contains(status.StatusId));
    }

    public static bool IsValidObject(this IPlayerCharacter? character)
    {
        return character?.EntityId is not null and not 0 and not 0xE000_0000;
    }

    public static bool HasPet(this IPlayerCharacter character)
    {
        var ownedObjects = Service.ObjectTable.Where(obj => obj.OwnerId == character.EntityId);

        return ownedObjects.Any(obj => obj.ObjectKind == ObjectKind.BattleNpc &&
                                       (obj as IBattleNpc)?.SubKind == (byte)BattleNpcSubKind.Pet);
    }

    public static IEnumerable<IPlayerCharacter> Alive(this IEnumerable<IPlayerCharacter> list)
    {
        return list.Where(member => member.CurrentHp > 0);
    }

    public static IEnumerable<IPlayerCharacter> WithJob(this IEnumerable<IPlayerCharacter> list, uint jobID)
    {
        return list.Where(member => member.ClassJob.RowId == jobID);
    }

    public static IEnumerable<IPlayerCharacter> WithJob(this IEnumerable<IPlayerCharacter> list, List<uint> jobList)
    {
        return list.Where(member => jobList.Contains(member.ClassJob.RowId));
    }

    public static IEnumerable<IPlayerCharacter> WithStatus(this IEnumerable<IPlayerCharacter> list, uint statusID)
    {
        return list.Where(member => member.HasStatus(statusID));
    }

    public static IEnumerable<IPlayerCharacter> WithStatus(
        this IEnumerable<IPlayerCharacter> list, List<uint> statusList)
    {
        return list.Where(member => member.HasStatus(statusList));
    }
}
