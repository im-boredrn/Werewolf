using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WereWolf.assets.Werewolf.Configuration
{
    public  class WerewolfConfig
    {
        // Movement
        public float WerewolfSpeed { get; set; } = 1.3f;
        public float WereWolfJump { get; set; } = 1.3f;


        // Core Stats
        public float WerewolfDamage { get; set; } = 2.0f; // Harmony
        public float WereWolfDamageReduction  { get; set; } = 0.80f; // Harmony 
        public float WereWolfMaxHealth { get; set; } = 2.0f;
        public float WereWolfRegen { get; set; } = 2.0f;
        public float WereWolfRangedAcc { get; set; } = 0.10f;
        public bool WereWolfDisableFallDamage { get; set; } = true;



        // Wolf Abilities

        public float WereWolfForageDropRate { get; set; } = 2.0f;
        public float WereWolfWildCropDropRate { get; set; } = 2.0f;
        public float WereWolfAnimalSeekingRange { get; set; } = 2.0f;
        public float WereWolfAnimalLootDropRate { get; set; } = 2.0f;
        public float WereWolfAnimalHarvestingTime { get; set; } = 2.0f;
        public float WereWolfBowDrawingStrength { get; set; } = 2.0f;


    }
}
