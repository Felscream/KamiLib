#region

using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using KamiLib.Caching;
using KamiLib.ChatCommands;
using KamiLib.Localization;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace KamiLib.Teleporter
{
    public class TeleportInfo
    {

        public TeleportInfo(uint commandID, Enum target, uint aetheriteID)
        {
            CommandID = commandID;
            Target = target;
            Aetherite = GetAetheryte(aetheriteID);
        }
        public uint CommandID { get; }
        public Enum Target { get; }
        public AetheryteTransient? Aetherite { get; }

        private static AetheryteTransient? GetAetheryte(uint id)
        {
            return LuminaCache<AetheryteTransient>.Instance.GetRow(id)!;
        }
    }

    public record TeleportLinkPayloads(Enum Location, DalamudLinkPayload Payload);

    public class TeleportManager : IDisposable
    {
        private static TeleportManager? _instance;
        private readonly ICallGateSubscriber<bool> showChatMessageIpc;

        private readonly List<TeleportInfo> teleportInfoList = new();

        private readonly ICallGateSubscriber<uint, byte, bool> teleportIpc;

        private TeleportManager()
        {
            teleportIpc = Service.PluginInterface.GetIpcSubscriber<uint, byte, bool>(Strings.Teleport_Label);
            showChatMessageIpc = Service.PluginInterface.GetIpcSubscriber<bool>("Teleport.ChatMessage");
        }
        public static TeleportManager Instance => _instance ??= new TeleportManager();

        private List<TeleportLinkPayloads> ChatLinkPayloads { get; } = new();

        public void Dispose()
        {
            foreach (var payload in teleportInfoList)
            {
                Service.Chat.RemoveChatLinkHandler(payload.CommandID);
            }
        }

        public static void Cleanup()
        {
            _instance?.Dispose();
        }

        public void AddTeleports(IEnumerable<TeleportInfo> teleports)
        {
            teleportInfoList.AddRange(teleports);

            foreach (var teleport in teleportInfoList)
            {
                Service.Chat.RemoveChatLinkHandler(teleport.CommandID);

                var linkPayload = Service.Chat.AddChatLinkHandler(teleport.CommandID, TeleportAction);

                ChatLinkPayloads.Add(new TeleportLinkPayloads(teleport.Target, linkPayload));
            }
        }

        private void TeleportAction(uint command, SeString message)
        {
            var teleportInfo = teleportInfoList.First(teleport => teleport.CommandID == command);

            if (AetheryteUnlocked(teleportInfo.Aetherite, out var targetAetheriteEntry))
            {
                Teleport(targetAetheriteEntry!);
            }
            else
            {
                Service.PluginLog.Error("User attempted to teleport to an aetheryte that is not unlocked");
                UserError(Strings.Teleport_NotUnlocked);
            }
        }

        public DalamudLinkPayload GetPayload(Enum targetLocation)
        {
            return ChatLinkPayloads.First(payload => Equals(payload.Location, targetLocation)).Payload;
        }

        private void Teleport(IAetheryteEntry aetheryte)
        {
            try
            {
                var didTeleport = teleportIpc.InvokeFunc(aetheryte.AetheryteId, aetheryte.SubIndex);
                var showMessage = showChatMessageIpc.InvokeFunc();

                if (!didTeleport)
                {
                    UserError(Strings.Teleport_BadSituation);
                }
                else if (showMessage)
                {
                    Chat.Print(Strings.Teleport_Label,
                               string.Format(Strings.Teleport_TeleportingTo, GetAetheryteName(aetheryte)));
                }
            }
            catch (IpcNotReadyError)
            {
                Service.PluginLog.Error("Teleport IPC not found");
                UserError(Strings.Teleport_InstallTeleporter);
            }
        }

        private void UserError(string error)
        {
            Service.Chat.PrintError(error);
            Service.Toast.ShowError(error);
        }

        private string GetAetheryteName(IAetheryteEntry aetheryte)
        {
            var gameData = aetheryte.AetheryteData.Value;
            var placeName = gameData.PlaceName.Value;

            return string.IsNullOrEmpty(placeName.Name.ToString()) ? "[Name Lookup Failed]" : placeName.Name.ToString();
        }

        private bool AetheryteUnlocked(IExcelRow<AetheryteTransient> aetheryte, out IAetheryteEntry? entry)
        {
            if (Service.AetheryteList.Any(entry => entry.AetheryteId == aetheryte.RowId))
            {
                entry = Service.AetheryteList.First(entry => entry.AetheryteId == aetheryte.RowId);
                return true;
            }
            entry = null;
            return false;
        }
    }
}
