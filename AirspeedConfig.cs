using BepInEx.Configuration;
namespace Airspeed
{
    internal static class AirspeedConfig
    {
        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<bool> Verbose;
        internal static ConfigEntry<bool> SwapHoldMode;
        internal static ConfigEntry<bool> BracketHoldMode;
        internal static void Bind(ConfigFile cfg)
        {
            Enabled = cfg.Bind("General", "Enabled", true,
                new ConfigDescription(
                    "Show calibrated airspeed instead of true airspeed on your own aircraft's readouts "
                    + "(HUD speed gauge, basic instruments, map MFD, landing screen). "
                    + "Overspeed warnings and target speeds are unaffected."));
            Verbose = cfg.Bind("General", "Verbose", false,
                new ConfigDescription(
                    "Write a diagnostic line to the log about once a second while airborne, showing "
                    + "altitude, air density, true airspeed, calibrated airspeed and their ratio."));
            SwapHoldMode = cfg.Bind("Keybinds", "Swap Readout Hold Mode", true,
                new ConfigDescription(
                    "If true, the swap key peeks at the other value only while held. "
                    + "If false, it toggles the readouts between calibrated and true airspeed."));
            BracketHoldMode = cfg.Bind("Keybinds", "Show Both Hold Mode", true,
                new ConfigDescription(
                    "If true, the bracketed second value appears on the HUD only while the key is held. "
                    + "If false, the key toggles it on and off."));
        }
    }
}