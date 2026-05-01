using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using DalamudCommandManager = Dalamud.Plugin.Services.ICommandManager;
using DalamudCondition = Dalamud.Plugin.Services.ICondition;

namespace KamiLib;

internal class Service
{
    [PluginService] public static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static IPluginLog PluginLog { get; private set; } = null!;
    [PluginService] public static DalamudCommandManager Commands { get; private set; } = null!;
    [PluginService] public static IClientState ClientState { get; private set; } = null!;
    [PluginService] public static IPlayerState PlayerState { get; private set; } = null!;
    [PluginService] public static IChatGui Chat { get; private set; } = null!;
    [PluginService] public static IGameGui GameGui { get; private set; } = null!;
    [PluginService] public static DalamudCondition Condition { get; private set; } = null!;
    [PluginService] public static IDataManager DataManager { get; private set; } = null!;
    [PluginService] public static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] public static IFramework Framework { get; private set; } = null!;
    [PluginService] public static IToastGui Toast { get; private set;} = null!;
    [PluginService] public static IAetheryteList AetheryteList { get; private set;} = null!;
    [PluginService] public static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] public static IGameInteropProvider GameInteropProvider { get; private set; } = null!;
}
