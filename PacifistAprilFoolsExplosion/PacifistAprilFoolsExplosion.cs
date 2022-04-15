using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BaseX;
using CodeX;
using FrooxEngine;
using FrooxEngine.LogiX;
using FrooxEngine.LogiX.Data;
using FrooxEngine.LogiX.ProgramFlow;
using FrooxEngine.UIX;
using HarmonyLib;
using NeosModLoader;

namespace PacifistAprilFoolsExplosion
{
    public class PacifistAprilFoolsExplosion : NeosMod
    {
        public static ModConfiguration Config;

        private const string explosionSlotName = "Explosion";

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> DisableExplosion = new ModConfigurationKey<bool>("DisableExplosion", "Disable the Violent Explosion component locally when it's loaded.", () => true);

        /*
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> HideFlames = new ModConfigurationKey<bool>("HideFlames", "Hide the Violent Explosion's flame particles.", () => true);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> MuteBloating = new ModConfigurationKey<bool>("MuteBloating", "Mute the Violent Explosion's bloating sound effect.", () => false);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> MuteExplosion = new ModConfigurationKey<bool>("MuteExplosion", "Mute the Violent Explosion's explosion sound effect.", () => true);*/

        public override string Author => "Banane9";
        public override string Link => "https://github.com/Banane9/NeosPacifistAprilFoolsExplosion";
        public override string Name => "PacifistAprilFoolsExplosion";
        public override string Version => "1.0.0";

        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony($"{Author}.{Name}");
            Config = GetConfiguration();
            Config.Save(true);
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(ViolentAprilFoolsExplosion))]
        private static class ViolentAprilFoolsExplosionPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnAwake")]
            private static void OnAwakePostfix(ViolentAprilFoolsExplosion __instance)
            {
                __instance.RunInUpdates(2, () => ValueUserOverride.OverrideForUser(__instance.EnabledField, __instance.World.LocalUser, !Config.GetValue(DisableExplosion)));
            }
        }

        /*
        [HarmonyPatch(typeof(StaticAudioClip))]
        private static class StaticAudioClipPatch
        {
            [HarmonyPatch("OnAwake")]
            [HarmonyPostfix]
            private static void OnAwakePostfix(StaticAudioClip __instance)
            {
                Msg("Muting potential explosion awake");

                __instance.RunInUpdates(5, () =>
                {
                    Msg("Muting potential explosion pre");

                    if (__instance.Slot.ActiveSelf || __instance.Slot.Name != explosionSlotName)
                        return;

                    Msg("Muting potential explosion");

                    if (Config.GetValue(MuteBloating) && __instance.URL == NeosAssets.Common.Sound_Effect.Bloating)
                        ValueUserOverride.OverrideForUser(__instance.URL, __instance.World.LocalUser, null);
                    else if (Config.GetValue(MuteExplosion) && __instance.URL == NeosAssets.Common.Sound_Effect.Explosion)
                        ValueUserOverride.OverrideForUser(__instance.URL, __instance.World.LocalUser, null);
                });
            }
        }

        [HarmonyPatch(typeof(ComponentBase<Component>))]
        private static class UnlitMaterialPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnChanges")]
            private static void OnChangesPostfix(Component __instance)
            {
                if (__instance.Slot.ActiveSelf || !(__instance is UnlitMaterial) || __instance.Slot.Name != explosionSlotName)
                    return;

                Msg("Hiding potential flames");

                __instance.RunInUpdates(5, () =>
                {
                    var material = (UnlitMaterial)__instance;
                    if (Config.GetValue(HideFlames) && (material.Texture.Target.Asset as StaticTexture2D)?.URL == NeosAssets.Common.Particles.Effects.Flame_Atlas8x8)
                        ValueUserOverride.OverrideForUser(material.TintColor, __instance.World.LocalUser, color.Black);
                });
            }
        }*/
    }
}