using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VintageStoryHarmony;

namespace WereWolf.assets.Werewolf.Configuration
{
    internal class WereWolfModSettings
    {

        // Ensures config is auto reloaded after update

        //Movement
        public static float Speed => WereWolfModSystem.Config?.WerewolfSpeed ?? 1.3f;
        public static float Jump => WereWolfModSystem.Config?.WereWolfJump ?? 1.3f;


        //Core Stats
        public static float Damage => WereWolfModSystem.Config?.WerewolfDamage ?? 1f; // Harmony
        public static float DamageReduction => WereWolfModSystem.Config?.WereWolfDamageReduction ?? 0.5f; // Harmony
        //Health
        public static float MaxHealth => WereWolfModSystem.Config?.WereWolfMaxHealth ?? 2.0f;
        public static float HealingEffectivness => WereWolfModSystem.Config?.WereWolfHealingEffectivness ?? 2.0f;
        public static float NightRegen => WereWolfModSystem.Config?.WereWolfNightRegen ?? 0.025f;
        public static float DayRegen => WereWolfModSystem.Config?.WereWolfDayRegen ?? 0.05f;
        public static long TransformCooldownMS => WereWolfModSystem.Config?.WereWolfTransformCoolDown ?? 5;


        //Misc
        public static float RangedAcc => WereWolfModSystem.Config?.WereWolfRangedAcc ?? 0.10f;
        public static bool DisableFallDamage => WereWolfModSystem.Config?.WereWolfDisableFallDamage ?? false;

        //Wolf Abilities
        public static float ForageDropRate => WereWolfModSystem.Config?.WereWolfForageDropRate ?? 2.0f ;
        public static float WildCropDropRate => WereWolfModSystem.Config?.WereWolfWildCropDropRate ?? 2.0f;
        public static float AnimalSeekingRange => WereWolfModSystem.Config?.WereWolfAnimalSeekingRange ?? 2.0f;
        public static float AnimalLootDropRate => WereWolfModSystem.Config?.WereWolfAnimalLootDropRate ?? 2.0f;
        public static float AnimalHarvestingTime => WereWolfModSystem.Config?.WereWolfAnimalHarvestingTime ?? 2.0f;
        public static float BowDrawingStrength => WereWolfModSystem.Config?.WereWolfBowDrawingStrength ?? 2.0f  ;
        public static float EnabledMinBrightness => WereWolfModSystem.Config?.WereWolfRangedAcc ?? 0.8f;


    }
}
