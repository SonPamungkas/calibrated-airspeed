# Requested by Firelight Shine
True airspeed keeps climbing as the air thins, so at altitude the vanilla gauge reads far higher than the number that matters for stall margin, corner speed and structural limits. Calibrated airspeed is the one real pilots fly by:

image here

At sea level the two are identical. At 6 km the gauge reads about three quarters of the vanilla number.

## Features

- **Calibrated airspeed on four readouts** — the cockpit HUD speed gauge, the basic flight instruments, the speed line on the maximised map MFD, and the landing screen.
- **`C-` prefix** so you always know what you are reading. `C-800km/h` is calibrated; a bare `800km/h` is true airspeed. Honours your metric/imperial unit setting exactly as vanilla does.
- **Swap key** — flip the readouts between calibrated and true airspeed. Hold to peek, or set it to latch on a press.
- **Show-both key** — stacks the other value in brackets above the number on the HUD gauge, so you can read both at once without clipping the fuel gauge beside it:

<img width="311" height="219" alt="image" src="https://github.com/user-attachments/assets/28734bf8-fe67-4cf2-b9f2-68e27288c368" />

- **Display only.** Overspeed warnings, the gauge colour gradient, flight physics, AI and every other unit's speed readout are untouched and still use true airspeed.
- **No mod dependencies.** The input framework is compiled into the mod itself.

## Keybinds

Both keys ship **unbound**. Bind them under **Controls > Debug**:

| Action | What it does |
| --- | --- |
| `Airspeed::SwapReadout` | Swaps the primary number between calibrated and true airspeed, on all four readouts. |
| `Airspeed::ShowBoth` | Shows the other value in brackets above the primary. HUD speed gauge only. |

If the actions do not appear in the controls list, restart the game once so they are registered before the input system initialises.
