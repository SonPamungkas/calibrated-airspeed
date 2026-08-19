using BepInEx.Logging;
namespace Airspeed
{
    internal static class Log
    {
        internal static ManualLogSource Src;
        internal static bool V => AirspeedConfig.Verbose != null && AirspeedConfig.Verbose.Value;
        internal static void Info(string m)
        {
            if (Src != null) Src.LogInfo(m);
        }
        internal static void Warn(string m)
        {
            if (Src != null) Src.LogWarning(m);
        }
        internal static void Error(string m)
        {
            if (Src != null) Src.LogError(m);
        }
        internal static void Dbg(string m)
        {
            if (Src != null) Src.LogDebug(m);
        }
    }
}