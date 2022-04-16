using BaseX;
using FrooxEngine;
using HarmonyLib;
using NeosModLoader;

namespace PacifistAprilFoolsExplosion
{
    public class PacifistAprilFoolsExplosion : NeosMod
    {
        public static ModConfiguration Config;

        private const string explosionSlotName = "Explosion";

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> HideFlames = new ModConfigurationKey<bool>("HideFlames", "Hide the Violent Explosion's flame particles.", () => true);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> MuteBloating = new ModConfigurationKey<bool>("MuteBloating", "Mute the Violent Explosion's bloating sound effect.", () => false);

        [AutoRegisterConfigKey]
        private static ModConfigurationKey<bool> MuteExplosion = new ModConfigurationKey<bool>("MuteExplosion", "Mute the Violent Explosion's explosion sound effect.", () => true);

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

        [HarmonyPatch(typeof(StaticAudioClip))]
        private static class StaticAudioClipPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnAwake")]
            private static void OnAwakePostfix(StaticAudioClip __instance)
            {
                __instance.RunInUpdates(1, () =>
                {
                    if (__instance.Slot.PersistentSelf || __instance.Slot.Name != explosionSlotName)
                        return;

                    if (Config.GetValue(MuteBloating) && __instance.URL == NeosAssets.Common.Sound_Effect.Bloating)
                        __instance.URL.OverrideForUser(__instance.World.LocalUser, null);

                    if (Config.GetValue(MuteExplosion) && __instance.URL == NeosAssets.Common.Sound_Effect.Explosion)
                        __instance.URL.OverrideForUser(__instance.World.LocalUser, null);
                });
            }
        }

        [HarmonyPatch(typeof(UnlitMaterial))]
        private static class UnlitMaterialPatch
        {
            [HarmonyPostfix]
            [HarmonyPatch("OnAwake")]
            private static void OnAwakePostfix(UnlitMaterial __instance)
            {
                __instance.RunInUpdates(1, () =>
                {
                    if (__instance.Slot.PersistentSelf || !Config.GetValue(HideFlames) || __instance.Slot.Name != explosionSlotName)
                        return;

                    if ((__instance.Texture.Target as StaticTexture2D)?.URL == NeosAssets.Common.Particles.Effects.Flame_Atlas8x8)
                        __instance.TintColor.OverrideForUser(__instance.World.LocalUser, color.Black);
                });
            }
        }
    }
}