using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Party;

namespace KamiLib.Extensions;

public static class PartyListExtensions
{
    public static IEnumerable<IPartyMember> Alive(this IEnumerable<IPartyMember> list)
    {
        return list.Where(member => member.GameObject != null && !member.GameObject.IsDead);
    }

    public static IEnumerable<IPartyMember> WithRole(this IEnumerable<IPartyMember> list, uint roleID)
    {
        return list.Where(member => member.ClassJob.Value.Role == roleID);
    }

    public static IEnumerable<IPartyMember> WithJob(this IEnumerable<IPartyMember> list, uint jobID)
    {
        return list.Where(member => member.ClassJob.RowId == jobID);
    }

    public static IEnumerable<IPartyMember> WithJob(this IEnumerable<IPartyMember> list, List<uint> jobList)
    {
        return list.Where(member => jobList.Contains(member.ClassJob.RowId));
    }

    public static IEnumerable<IPartyMember> WithStatus(this IEnumerable<IPartyMember> list, uint statusID)
    {
        return list.Where(member => member.HasStatus(statusID));
    }

    public static IEnumerable<IPartyMember> WithStatus(this IEnumerable<IPartyMember> list, List<uint> statusList)
    {
        return list.Where(member => member.HasStatus(statusList));
    }
}
