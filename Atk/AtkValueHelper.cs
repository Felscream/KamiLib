using System;
using System.Runtime.InteropServices;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;
using ValueType = FFXIVClientStructs.FFXIV.Component.GUI.AtkValueType;

namespace KamiLib.Atk;

public static class AtkValueHelper
{
    public static unsafe void PrintAtkValue(AtkValue value, int index)
    {
        switch (value.Type)
        {
            case ValueType.Int:
                Service.PluginLog.Debug($"[{index:D3}] [{"int", 7}]: {value.Int}");
                break;
            case ValueType.Bool:
                Service.PluginLog.Debug($"[{index:D3}] [{"bool", 7}]: {(value.Byte != 0 ? "true" : "false")}");
                break;
            case ValueType.UInt:
                Service.PluginLog.Debug($"[{index:D3}] [{"uint", 7}]: {value.UInt}");
                break;
            case ValueType.Float:
                Service.PluginLog.Debug($"[{index:D3}] [{"float", 7}]: {value.Float}");
                break;
            case ValueType.String:
                Service.PluginLog.Debug($"[{index:D3}] [{"string", 7}]: {Marshal.PtrToStringUTF8(new nint(value.String))}");
                break;
            case ValueType.String8:
                Service.PluginLog.Debug($"[{index:D3}] [{"string8", 7}]: {Marshal.PtrToStringUTF8(new nint(value.String))}");
                break;
            case ValueType.Vector:
                Service.PluginLog.Debug($"[{index:D3}] [{"vector", 7}]: No Representation Implemented");
                break;
            default:                        
                Service.PluginLog.Debug($"[{index:D3}] [{"unknown", 7}]: [{value.Type}]: {BitConverter.ToString(BitConverter.GetBytes((long)value.String.Value)).Replace("-", " ")}");
                break;
        }
    }
}

public static class AtkValueExtensions
{
    public static unsafe string GetString(this AtkValue value)
    {
        return Marshal.PtrToStringUTF8(new nint(value.String)) ?? "Unable to Allocate String";
    }
}
