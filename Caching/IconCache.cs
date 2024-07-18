using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Interface.Internal;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Logging;
using Dalamud.Utility;
using ImGuiScene;

namespace KamiLib.Caching;

public class IconCache : IDisposable
{
    private readonly Dictionary<uint, IDalamudTextureWrap?> iconTextures = new();

    private const string IconFilePath = "ui/icon/{0:D3}000/{1:D6}_hr1.tex";
    
    private static IconCache? _instance;
    public static IconCache Instance => _instance ??= new IconCache();
    
    public static void Cleanup()
    {
        _instance?.Dispose();
    }
    
    public void Dispose() 
    {
        foreach (var texture in iconTextures.Values) 
        {
            texture?.Dispose();
        }

        iconTextures.Clear();
    }
        
    private void LoadIconTexture(uint iconId) 
    {
        Task.Run(() => 
        {
            try
            {
                var path = IconFilePath.Format(iconId / 1000, iconId);
                //var tex = Service.TextureProvider.GetTextureFromGame(path);
                var imtex = Service.TextureProvider.GetFromGame(path);
                var tex = imtex.GetWrapOrDefault();

                if (tex is not null && tex.ImGuiHandle != nint.Zero) 
                {
                    iconTextures[iconId] = tex;
                } 
                else 
                {
                    tex?.Dispose();
                }
            } 
            catch (Exception ex) 
            {
                Service.PluginLog.Error($"Failed loading texture for icon {iconId} - {ex.Message}");
            }
        });
    }
    
    public IDalamudTextureWrap? GetIcon(uint iconId) 
    {
        if (iconTextures.TryGetValue(iconId, out var value)) return value;

        iconTextures.Add(iconId, null);
        LoadIconTexture(iconId);

        return iconTextures[iconId];
    }
}
