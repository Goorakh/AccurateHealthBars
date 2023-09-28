using BepInEx.Configuration;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Globalization;
using UnityEngine;

namespace AccurateHealthBars
{
    static class HealthBarPatches
    {
        public static void Init()
        {
            IL.RoR2.UI.HealthBar.UpdateHealthbar += HealthBar_UpdateHealthbar;
        }

        static string formatHealthBarValue(float value, ConfigEntry<int> numDigitsConfig)
        {
            NumberFormatInfo numberFormat = NumberFormatInfo.CurrentInfo;
            string result = value.ToString($"F{numDigitsConfig.Value}", numberFormat);

            // If the number has a decimal part, check for unnecessary zeroes
            if (numDigitsConfig.Value > 0)
            {
                string zeroString = numberFormat.NativeDigits[0];

                int numTrailingZeroMatches = 0;
                for (int i = result.Length - zeroString.Length; i >= 0; i -= zeroString.Length)
                {
                    if (result.Substring(i, zeroString.Length) == zeroString)
                    {
                        numTrailingZeroMatches++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (numTrailingZeroMatches > 0)
                {
                    result = result.Remove(result.Length - (numTrailingZeroMatches * zeroString.Length) - numberFormat.NumberDecimalSeparator.Length);
                }
            }

            return result;
        }

        static void HealthBar_UpdateHealthbar(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(MoveType.Before,
                              x => x.MatchCallOrCallvirt(AccessTools.DeclaredPropertyGetter(typeof(HealthComponent), nameof(HealthComponent.combinedHealth))),
                              x => x.MatchCall(SymbolExtensions.GetMethodInfo(() => Mathf.Ceil(default)))))
            {
                c.Index++;
                c.Next.OpCode = OpCodes.Nop;

                int displayCurrentHealthLocalIndex = -1;
                if (c.TryGotoNext(x => x.MatchStloc(out displayCurrentHealthLocalIndex)))
                {
                    if (c.TryGotoNext(MoveType.Before,
                                      x => x.MatchLdloca(displayCurrentHealthLocalIndex),
                                      x => x.MatchCallOrCallvirt<float>(nameof(float.ToString))))
                    {
                        c.Index++;
                        c.Remove();
                        c.EmitDelegate((ref float displayValue) => formatHealthBarValue(displayValue, Main.CurrentHealthDecimals));
                    }
                    else
                    {
                        Log.Error("Failed to find currentHealth patch location");
                    }
                }
                else
                {
                    Log.Error("Failed to find currentHealth patch location");
                }
            }
            else
            {
                Log.Error("Failed to find currentHealth patch location");
            }

            c.Index = 0;
            if (c.TryGotoNext(MoveType.Before,
                              x => x.MatchLdloc(out _),
                              x => x.MatchCall(SymbolExtensions.GetMethodInfo(() => Mathf.Ceil(default)))))
            {
                c.Index++;
                c.Next.OpCode = OpCodes.Nop;

                int displayMaxHealthLocalIndex = -1;
                if (c.TryGotoNext(x => x.MatchStloc(out displayMaxHealthLocalIndex)))
                {
                    if (c.TryGotoNext(MoveType.Before,
                                      x => x.MatchLdloca(displayMaxHealthLocalIndex),
                                      x => x.MatchCallOrCallvirt<float>(nameof(float.ToString))))
                    {
                        c.Index++;
                        c.Remove();
                        c.EmitDelegate((ref float displayValue) => formatHealthBarValue(displayValue, Main.MaxHealthDecimals));
                    }
                    else
                    {
                        Log.Error("Failed to find maxHealth patch location");
                    }
                }
                else
                {
                    Log.Error("Failed to find maxHealth patch location");
                }
            }
            else
            {
                Log.Error("Failed to find maxHealth patch location");
            }
        }
    }
}
