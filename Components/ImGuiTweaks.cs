﻿using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;
using KamiLib.Extensions;

namespace KamiLib.Components;

public static class ImGuiTweaks {
    public static bool ColorEditWithDefault(string label, ref Vector4 color, Vector4 defaultColor) {
        var valueChanged = ImGui.ColorEdit4($"##{label}", ref color, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);

        ImGui.SameLine();
        
        if (ImGui.Button($"Default##{label}")) {
            color = defaultColor;
        }

        ImGui.SameLine();

        ImGui.TextUnformatted(label);
        
        return valueChanged;
    }

    public static bool IconButtonWithSize(IFontHandle font, FontAwesomeIcon icon, string id, Vector2 size, string? tooltip = null) {
        using var imRaiiId = ImRaii.PushId(id);
        bool result;

        using (font.Push()) {
            result = ImGui.Button($"{icon.ToIconString()}", size);
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && tooltip is not null) {
            ImGui.SetTooltip(tooltip);
        }

        return result;
    }

    public static void TextColoredUnformatted(Vector4 color, string text) {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, color);
        ImGui.TextUnformatted(text);
    }

    public static bool EnumCombo<T>(string label, ref T refValue) where T : Enum {
        using var combo = ImRaii.Combo(label, refValue.GetDescription());
        if (!combo) return false;

        foreach (Enum enumValue in Enum.GetValues(refValue.GetType())) {
            if (!ImGui.Selectable(enumValue.GetDescription(), enumValue.Equals(refValue))) continue;
            
            refValue = (T)enumValue;
            return true;
        }

        return false;
    }

    public static bool Checkbox(string label, ref bool value, string? hintText) {
        using var group = ImRaii.Group();
        
        var result = ImGui.Checkbox(label, ref value);

        if (hintText is not null) {
            ImGuiComponents.HelpMarker(hintText);
        }

        return result;
    }

    public static bool PriorityInt(DalamudPluginInterface pluginInterface, string label, ref int value) {
        ImGui.SetNextItemWidth(22.0f * ImGuiHelpers.GlobalScale);
        var valueChanged = ImGui.InputInt($"##{label}_input_int", ref value, 0, 0);
        
        using (pluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {

            ImGui.SameLine();
            if (ImGui.Button(FontAwesomeIcon.ChevronUp.ToIconString())) {
                value++;
                valueChanged = true;
            }
            
            ImGui.SameLine();
            if (ImGui.Button(FontAwesomeIcon.ChevronDown.ToIconString())) {
                value--;
                valueChanged = true;
            }

            value = Math.Clamp(value, -9, 9);
        }
        
        ImGui.SameLine();
        ImGui.Text(label);
        
        return valueChanged;
    }
}