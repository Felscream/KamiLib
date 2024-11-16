using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Drawing;
using Lumina.Excel.Sheets;

namespace KamiLib.Extensions;

public static class TerritoryTypeExtensions
{
    public static void DrawLabel(this TerritoryType data)
    {
        var placeString = data.GetPlaceNameString();

        var startPosition = ImGui.GetCursorPos();
        ImGui.TextColored(Colors.Grey, data.RowId.ToString());
        ImGui.SameLine(startPosition.X + 50.0f * ImGuiHelpers.GlobalScale);
        ImGui.Text(placeString);
    }

    public static string GetPlaceNameString(this TerritoryType data)
    {
        var placeNameRow = data.PlaceName.RowId;
        var placeName = LuminaCache<PlaceName>.Instance.GetRow(placeNameRow);
        var placeString = placeName?.Name.ToDalamudString().TextValue ?? "Unknown PlaceName";

        return placeString;
    }
}
