using BepInEx;
using HarmonyLib;
using InputFramework;
using Rewired;
using UnityEngine;
namespace Airspeed
{
    [BepInPlugin("neutral.airspeed", "AirspeedMod", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private const string SwapAction = "Airspeed::SwapReadout";
        private const string BothAction = "Airspeed::ShowBoth";
        private float _logAccum;
        private void Awake()
        {
            Log.Src = Logger;
            AirspeedConfig.Bind(Config);
            ExtraInputManager.LoadPendingActions();
            ExtraInputManager.RegisterAction(SwapAction, InputActionType.Button, "Debug");
            ExtraInputManager.RegisterAction(BothAction, InputActionType.Button, "Debug");
            Harmony h = new Harmony("neutral.airspeed");
            h.CreateClassProcessor(typeof(Patches.SpeedGauge_Refresh_Patch)).Patch();
            h.CreateClassProcessor(typeof(Patches.BasicFlightInstruments_Refresh_Patch)).Patch();
            h.CreateClassProcessor(typeof(Patches.VirtualMFD_Update_Patch)).Patch();
            h.CreateClassProcessor(typeof(Patches.LandingScreenUI_LateUpdate_Patch)).Patch();
            h.CreateClassProcessor(typeof(RewiredActionInjector)).Patch();
            string stamp = "unknown";
            try
            {
                string dll = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(dll) && System.IO.File.Exists(dll))
                    stamp = System.IO.File.GetLastWriteTime(dll).ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch { }
            Logger.LogInfo("[Airspeed] AirspeedMod v1.0.0 build " + stamp +
                           " loaded. 4 readout patches applied. Bind " + SwapAction + " and " +
                           BothAction + " under Controls > Debug.");
        }
        private void Update()
        {
            if (!AirspeedConfig.Enabled.Value) return;
            PollKeys();
            if (Log.V) LogReading(Time.unscaledDeltaTime);
        }
        private void PollKeys()
        {
            if (!ExtraInputManager.RewiredInitialized) return;
            bool inChat = false;
            try { inChat = CursorManager.GetFlag(CursorFlags.Chat); } catch { }
            if (inChat) return;
            Player p = ReInput.players.GetPlayer(0);
            if (p == null) return;
            if (AirspeedConfig.SwapHoldMode.Value)
            {
                Patches.Swapped = p.GetButton(SwapAction);
            }
            else if (p.GetButtonDown(SwapAction))
            {
                Patches.Swapped = !Patches.Swapped;
                Log.Info("[Airspeed] Readout swapped to " + (Patches.Swapped ? "true" : "calibrated") + " airspeed.");
            }
            if (AirspeedConfig.BracketHoldMode.Value)
            {
                Patches.Brackets = p.GetButton(BothAction);
            }
            else if (p.GetButtonDown(BothAction))
            {
                Patches.Brackets = !Patches.Brackets;
                Log.Info("[Airspeed] Bracketed second value " + (Patches.Brackets ? "shown." : "hidden."));
            }
        }
        private void LogReading(float dt)
        {
            _logAccum += dt;
            if (_logAccum < 1f) return;
            _logAccum = 0f;
            Aircraft ac = Patches.PlayerAircraft();
            if (ac == null) return;
            float rho = ac.airDensity;
            float tas = ac.speed;
            float cas = Patches.CasOf(ac);
            Log.Dbg($"[Airspeed] alt={ac.GlobalPosition().y:F0}m rho={rho:F3} " +
                    $"TAS={tas:F1}m/s CAS={cas:F1}m/s ratio={(tas > 0f ? cas / tas : 1f):F3}");
        }
    }
}