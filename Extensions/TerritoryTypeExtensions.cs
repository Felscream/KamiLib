﻿using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets2;
using TerritoryType = Lumina.Excel.GeneratedSheets.TerritoryType;

namespace KamiLib.Classes;

public static class TerritoryTypeExtensions {
    private const float Width = 133.5f;
    private const float Height = 75.0f;
    private static readonly Dictionary<uint, ContentFinderCondition?> ContentFinderConditionMap = [];
    
    public static void Draw(this TerritoryType territoryType, IDataManager dataManager, ITextureProvider textureProvider) {
        using var id = ImRaii.PushId(territoryType.RowId.ToString());
        
        DrawTerritoryImage(territoryType, dataManager, textureProvider);
        ImGui.SameLine();
        DrawTerritoryInfo(territoryType, dataManager);
    }
    
    private static void DrawTerritoryImage(TerritoryType option, IDataManager dataManager, ITextureProvider textureProvider) {
        using var imageFrame = ImRaii.Child($"image_frame{option}", ImGuiHelpers.ScaledVector2(Width * ImGuiHelpers.GlobalScale, Height), false, ImGuiWindowFlags.NoInputs);
        if (!imageFrame) return;
        
        if (dataManager.GetExcelSheet<LoadingImage>()!.GetRow(option.LoadingImage) is { } loadingImageInfo) {
            if (textureProvider.GetTextureFromGame($"ui/loadingimage/{loadingImageInfo.Unknown0}_hr1.tex") is {  } texture) {
                ImGui.Image(texture.ImGuiHandle, ImGuiHelpers.ScaledVector2(Width, Height), new Vector2(0.15f, 0.15f), new Vector2(0.85f, 0.85f));
            }
            else {
                ImGuiHelpers.ScaledDummy(Width, Height);
            }
        }
    }
    
    private static void DrawTerritoryInfo(TerritoryType option, IDataManager dataManager) {
        using var contentsFrame = ImRaii.Child("contents_frame", new Vector2(ImGui.GetContentRegionAvail().X, Height * ImGuiHelpers.GlobalScale), false, ImGuiWindowFlags.NoInputs);
        if (!contentsFrame) return;

        ImGuiHelpers.ScaledDummy(1.0f);
        
        using var table = ImRaii.Table("data_table", 2, ImGuiTableFlags.SizingStretchProp);
        if (!table) return;
        
        ImGui.TableSetupColumn("##column1", ImGuiTableColumnFlags.None, 2.0f);
        ImGui.TableSetupColumn("##column2", ImGuiTableColumnFlags.None, 1.0f);

        var placeName = option.PlaceName.Value?.Name ?? "Unknown PlaceName";
        var zoneName = option.PlaceNameZone.Value?.Name;
        var regionName = option.PlaceNameRegion.Value?.Name;
        
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(placeName);
        
        ImGui.TableNextColumn();
        ImGui.TextUnformatted(option.RowId.ToString());
        
        ImGui.TableNextRow();
        ImGui.TableNextColumn();

        using var grayColor = ImRaii.PushColor(ImGuiCol.Text, KnownColor.DarkGray.Vector());
        if (zoneName is not null && !zoneName.ToString().IsNullOrEmpty() && regionName is not null && !regionName.ToString().IsNullOrEmpty()) {
            ImGui.TextUnformatted($"{regionName}, {zoneName}");
        }
        else if (zoneName is not null && !zoneName.ToString().IsNullOrEmpty()) {
            ImGui.TextUnformatted($"{zoneName}");
        }
        else if (regionName is not null && !regionName.ToString().IsNullOrEmpty()) {
            ImGui.TextUnformatted($"{regionName}");
        }

        ImGui.TableNextColumn();
        ImGui.TextUnformatted($"{((TerritoryIntendedUseEnum)option.TerritoryIntendedUse).ToString()}");
        
        ImGui.TableNextColumn();
        if  (ContentFinderConditionMap.ContainsKey(option.RowId) && ContentFinderConditionMap.TryGetValue(option.RowId, out var cfc) && cfc is not null) {
            ImGui.TextUnformatted($"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cfc.Name)}");
        }
        else if (!ContentFinderConditionMap.ContainsKey(option.RowId)) {
            ContentFinderConditionMap.TryAdd(option.RowId, dataManager.GetExcelSheet<ContentFinderCondition>()!.FirstOrDefault(contentFinderCondition => contentFinderCondition.TerritoryType.Row == option.RowId));
        }
    }
}