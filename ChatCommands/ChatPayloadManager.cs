#region

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace KamiLib.ChatCommands
{
    internal record ChatLinkPayload(uint CommandID, uint Type, DalamudLinkPayload Payload);

    public class ChatPayloadManager : IDisposable
    {
        private static ChatPayloadManager? _instance;
        public static ChatPayloadManager Instance => _instance ??= new ChatPayloadManager();

        private List<ChatLinkPayload> ChatLinkPayloads { get; } = new();

        public void Dispose()
        {
            foreach (var payload in ChatLinkPayloads)
            {
                Service.Chat.RemoveChatLinkHandler(payload.Type + 1000);
            }
        }

        public static void Cleanup()
        {
            _instance?.Dispose();
        }

        public DalamudLinkPayload AddChatLink(Enum type, Action<uint, SeString> payloadAction)
        {
            return AddChatLink(Convert.ToUInt32(type), payloadAction);
        }

        private DalamudLinkPayload AddChatLink(uint type, Action<uint, SeString> payloadAction)
        {
            // If the payload is already registered
            var payload = ChatLinkPayloads.FirstOrDefault(linkPayload => linkPayload.CommandID == type + 1000)?.Payload;
            if (payload != null) return payload;

            // else
            Service.Chat.RemoveChatLinkHandler(type + 1000);
            payload = Service.Chat.AddChatLinkHandler(type + 1000, payloadAction);

            ChatLinkPayloads.Add(new ChatLinkPayload(type + 1000, type, payload));

            return payload;
        }
    }
}
