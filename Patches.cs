using System;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Airspeed
{
    internal static class Patches
    {
        private const float Rho0 = 1.225f;
        internal static bool Swapped;
        internal static bool Brackets;
        internal static float CasOf(Aircraft ac)
        {
            float rho = ac.airDensity;
            float tas = ac.speed;
            return rho > 0f ? tas * Mathf.Sqrt(rho / Rho0) : tas;
        }
        private static string Cas(Aircraft ac)
        {
            return "C-" + UnitConverter.SpeedReading(CasOf(ac));
        }
        private static string Tas(Aircraft ac)
        {
            return UnitConverter.SpeedReading(ac.speed);
        }
        private static string Reading(Aircraft ac, bool allowBrackets)
        {
            string primary = Swapped ? Tas(ac) : Cas(ac);
            if (!Brackets || !allowBrackets) return primary;
            return "(" + (Swapped ? Cas(ac) : Tas(ac)) + ")\n" + primary;
        }
        private static VerticalAlignmentOptions _vAlignOriginal;
        private static bool _vAlignCaptured;
        private static void ApplyStackAlignment(TextMeshProUGUI label, bool stacked)
        {
            if (stacked)
            {
                if (!_vAlignCaptured)
                {
                    _vAlignOriginal = label.verticalAlignment;
                    _vAlignCaptured = true;
                }
                if (label.verticalAlignment != VerticalAlignmentOptions.Bottom)
                    label.verticalAlignment = VerticalAlignmentOptions.Bottom;
            }
            else if (_vAlignCaptured && label.verticalAlignment != _vAlignOriginal)
            {
                label.verticalAlignment = _vAlignOriginal;
            }
        }
        private static void Apply(TextMeshProUGUI label, string text)
        {
            if (label.text != text) label.text = text;
        }
        private static void Apply(Text label, string text)
        {
            if (label.text != text) label.text = text;
        }
        internal static Aircraft PlayerAircraft()
        {
            CombatHUD hud = SceneSingleton<CombatHUD>.i;
            if (hud == null || hud.aircraft == null) return null;
            return hud.aircraft;
        }
        [HarmonyPatch(typeof(SpeedGauge), "Refresh")]
        internal static class SpeedGauge_Refresh_Patch
        {
            private static readonly AccessTools.FieldRef<SpeedGauge, Aircraft> AircraftRef =
                AccessTools.FieldRefAccess<SpeedGauge, Aircraft>("aircraft");
            private static readonly AccessTools.FieldRef<SpeedGauge, TextMeshProUGUI> DisplayRef =
                AccessTools.FieldRefAccess<SpeedGauge, TextMeshProUGUI>("airspeedDisplay");
            [HarmonyPostfix]
            private static void Postfix(SpeedGauge __instance)
            {
                if (!AirspeedConfig.Enabled.Value) return;
                try
                {
                    Aircraft ac = AircraftRef(__instance);
                    TextMeshProUGUI display = DisplayRef(__instance);
                    if (ac == null || display == null) return;
                    Apply(display, Reading(ac, true));
                    ApplyStackAlignment(display, Brackets);
                }
                catch (Exception ex)
                {
                    Log.Error("[Airspeed] SpeedGauge: " + ex);
                }
            }
        }
        [HarmonyPatch(typeof(BasicFlightInstruments), "Refresh")]
        internal static class BasicFlightInstruments_Refresh_Patch
        {
            private static readonly AccessTools.FieldRef<BasicFlightInstruments, Aircraft> AircraftRef =
                AccessTools.FieldRefAccess<BasicFlightInstruments, Aircraft>("aircraft");
            private static readonly AccessTools.FieldRef<BasicFlightInstruments, TextMeshProUGUI> DisplayRef =
                AccessTools.FieldRefAccess<BasicFlightInstruments, TextMeshProUGUI>("airspeedDisplay");
            [HarmonyPostfix]
            private static void Postfix(BasicFlightInstruments __instance)
            {
                if (!AirspeedConfig.Enabled.Value) return;
                try
                {
                    Aircraft ac = AircraftRef(__instance);
                    TextMeshProUGUI display = DisplayRef(__instance);
                    if (ac == null || display == null) return;
                    Apply(display, Reading(ac, false));
                }
                catch (Exception ex)
                {
                    Log.Error("[Airspeed] BasicFlightInstruments: " + ex);
                }
            }
        }
        [HarmonyPatch(typeof(VirtualMFD), "Update")]
        internal static class VirtualMFD_Update_Patch
        {
            private static readonly AccessTools.FieldRef<VirtualMFD, TextMeshProUGUI> SpeedRef =
                AccessTools.FieldRefAccess<VirtualMFD, TextMeshProUGUI>("speed");
            [HarmonyPostfix]
            private static void Postfix(VirtualMFD __instance)
            {
                if (!AirspeedConfig.Enabled.Value || !DynamicMap.mapMaximized) return;
                try
                {
                    Aircraft ac = PlayerAircraft();
                    TextMeshProUGUI display = SpeedRef(__instance);
                    if (ac == null || display == null) return;
                    Apply(display, Reading(ac, false));
                }
                catch (Exception ex)
                {
                    Log.Error("[Airspeed] VirtualMFD: " + ex);
                }
            }
        }
        [HarmonyPatch(typeof(LandingScreenUI), "LateUpdate")]
        internal static class LandingScreenUI_LateUpdate_Patch
        {
            private static readonly AccessTools.FieldRef<LandingScreenUI, Text> SpeedRef =
                AccessTools.FieldRefAccess<LandingScreenUI, Text>("speed");
            [HarmonyPostfix]
            private static void Postfix(LandingScreenUI __instance)
            {
                if (!AirspeedConfig.Enabled.Value) return;
                try
                {
                    Aircraft ac = PlayerAircraft();
                    Text display = SpeedRef(__instance);
                    if (ac == null || display == null) return;
                    Apply(display, "SPD " + Reading(ac, false));
                }
                catch (Exception ex)
                {
                    Log.Error("[Airspeed] LandingScreenUI: " + ex);
                }
            }
        }
    }
}