using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using VintageStoryHarmony.assets;
using WereWolf.assets.Werewolf;
using WereWolf.assets.Werewolf.Configuration;
using Vintagestory.GameContent;
using static VintageStoryHarmony.assets.PlayerData;

namespace VintageStoryHarmony
{

    public class WereWolfModSystem : ModSystem
    {
        // Called on server and client
        // Useful for registering block/entity classes on both sides
        public ICoreClientAPI? Capi { get; private set; }

        private Harmony? harmony;

        private ICoreServerAPI? sapi;
        private long tickListenerId;
        public bool ManualFormActive = false;
        public Forms ManualForm;


        public static WerewolfConfig? Config;

        public static WereWolfModSystem? Instance; // optional for easy access


        public const string PLUGIN_GUID = "kortah.werewolf";
        public const string PLUGIN_NAME = "Werewolf!";
        public override void Start(ICoreAPI api)
        {


            Instance = this;


            harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();

            base.Start(api);

            Mod.Logger.Notification("WEREWOLF MOD LOADED!");


            try
            {
                Config = api.LoadModConfig<WerewolfConfig>("werewolf-config.json");
            }
            catch (Exception e)
            {
                api.Logger.Warning($"Failed to load werewolf-config.json: {e.Message}");
                Config = null;
            }
            // If config is null OR any field is null, create a default one
            if (Config == null)
            {
                Config = new WerewolfConfig
                {
                    
                    WerewolfSpeed = 1.3f,
                    WereWolfJump = 1.3f,
                    WerewolfDamage = 2.0f,
                    WereWolfDamageReduction = 0.5f,
                    WereWolfMaxHealth = 20f,
                    WereWolfHealingEffectivness = 1f,
                    WereWolfRangedAcc = 0.1f,
                    WereWolfDisableFallDamage = true,
                    WereWolfForageDropRate = 2f,
                    WereWolfWildCropDropRate = 2f,
                    WereWolfAnimalSeekingRange = 2f,
                    WereWolfAnimalLootDropRate = 2f,
                    WereWolfAnimalHarvestingTime = 2f,
                    WereWolfBowDrawingStrength = 2f,
                    WereWolfDayRegen = 0.05f,
                    WereWolfNightRegen = 0.025f,
                    WereWolfTransformCoolDown = 5
                };

                api.Logger.Notification("Config loaded and validated. Any invalid values were reset to safe defaults.");
                api.Logger.Notification("Werewolf default config created!");
            }
            ValidateConfig();
            api.StoreModConfig(Config, "werewolf-config.json");

            Mod.Logger.Notification("WEREWOLF MOD LOADED!");


            api.ChatCommands.Create("reloadwerewolfconfig")
               .WithDescription("reload werewolf config without reloading world")
               .RequiresPrivilege("chat") // allows any player with chat access
               .HandleWith((args) =>
               {
                   WereWolfModSystem.Config = api.LoadModConfig<WerewolfConfig>("werewolf-config.json");

                   ValidateConfig();
                   api.StoreModConfig(Config, "werewolf-config.json");
                   return TextCommandResult.Success("Werewolf config reloaded!\n Config loaded and validated. Any invalid values were reset to safe defaults");





               });
        }




        public override void StartServerSide(ICoreServerAPI api)
        {
            Mod.Logger.Notification("Hello from template mod server side: " + Lang.Get("vintagestoryharmony:hello"));



            sapi = api;
            // Tick Listener
            tickListenerId = api.Event.RegisterGameTickListener(OnServerTick, 1000);

            api.Logger.Notification($"WEREWOLF CONFIG LOADED");
        }


        public override void StartClientSide(ICoreClientAPI api)

        {
            Capi = api;
            api.Input.RegisterHotKey("togglewerewolf", "Toggle Werewolf Form", GlKeys.G, HotkeyType.CharacterControls);
            api.Input.SetHotKeyHandler("togglewerewolf", Keybind.OnKeybindPress);

            api.Logger.Warning($"KEYBIND ran on: {api.Side}");

        
        Mod.Logger.Notification("Hello from template mod client side: " + Lang.Get("vintagestoryharmony:hello"));
        }

        private void OnServerTick(float dt)
        {
            foreach (IServerPlayer player in sapi?.World.AllOnlinePlayers ?? Array.Empty<IServerPlayer>())
            {
                var entity = player?.Entity as EntityPlayer;
                if (entity == null || entity.World == null) continue; // skip if null

                bool night = WolfTime.isNight(entity);
                bool day = !night;

                var form = PlayerData.GetForm(entity);

                // Safe logging
                // LOG SPAMMERS JUST FOR TESTING  sapi?.Logger.Warning($"Hour: {entity.World.Calendar?.HourOfDay ?? -1} | Night: {night}");

                // Decide form based on night/day or manual toggle
                Transform.Transformation(entity, entity);
                ApplyRegen(entity);

                // LOG SPAMMERS JUST FOR TESTING   sapi?.Logger.Warning($"After transform, PlayerData.Form = {PlayerData.GetForm(entity)}");
                // LOG SPAMMERS JUST FOR TESTING   sapi?.Logger.Warning($"ManualActive: {WereWolfModSystem.Instance?.ManualFormActive} | ManualForm: {WereWolfModSystem.Instance?.ManualForm}");

                // Regeneration For Wolf






            }



        }





        private void ApplyRegen(EntityPlayer entity)
        {
            var form = PlayerData.GetForm(entity);
            bool night = WolfTime.isNight(entity);
            // bool day = !night; // optional, not used here

            var healthBehavior = entity?.GetBehavior<Vintagestory.GameContent.EntityBehaviorHealth>();

            if (healthBehavior != null && form == Forms.WereWolf && night)
            {
                // Add regen safely, capped at MaxHealth
                healthBehavior.Health = Math.Min(healthBehavior.MaxHealth, healthBehavior.Health + WereWolfModSettings.NightRegen);


            }
            else if (healthBehavior != null && form == Forms.WereWolf && !night)
            {
                healthBehavior.Health = Math.Min(healthBehavior.MaxHealth, healthBehavior.Health + WereWolfModSettings.DayRegen);

            }
        }

             private void ValidateConfig()
        {

            if (Config == null) return;

            // Movement
            Config.WerewolfSpeed = Math.Clamp(Config.WerewolfSpeed, 0.1f, 10f);
            Config.WereWolfJump = Math.Clamp(Config.WereWolfJump, 0.1f, 10f);

            // Core Stats
            Config.WerewolfDamage = Math.Clamp(Config.WerewolfDamage, 0f, 100f);
            Config.WereWolfDamageReduction = Math.Clamp(Config.WereWolfDamageReduction, 0f, 1f);
            Config.WereWolfMaxHealth = Math.Clamp(Config.WereWolfMaxHealth, 1f, 500f);
            Config.WereWolfHealingEffectivness = Math.Clamp(Config.WereWolfHealingEffectivness, 0f, 10f);
            Config.WereWolfRangedAcc = Math.Clamp(Config.WereWolfRangedAcc, 0f, 1f);

            // Regeneration
            Config.WereWolfDayRegen = Math.Clamp(Config.WereWolfDayRegen, 0f, 5f);
            Config.WereWolfNightRegen = Math.Clamp(Config.WereWolfNightRegen, 0f, 5f);

            // Cooldown (minutes)
            Config.WereWolfTransformCoolDown = Math.Clamp(Config.WereWolfTransformCoolDown, 0, 30);

            // Wolf Abilities
            Config.WereWolfForageDropRate = Math.Clamp(Config.WereWolfForageDropRate, 0f, 10f);
            Config.WereWolfWildCropDropRate = Math.Clamp(Config.WereWolfWildCropDropRate, 0f, 10f);
            Config.WereWolfAnimalSeekingRange = Math.Clamp(Config.WereWolfAnimalSeekingRange, 0f, 50f);
            Config.WereWolfAnimalLootDropRate = Math.Clamp(Config.WereWolfAnimalLootDropRate, 0f, 10f);
            Config.WereWolfAnimalHarvestingTime = Math.Clamp(Config.WereWolfAnimalHarvestingTime, 0f, 30f);
            Config.WereWolfBowDrawingStrength = Math.Clamp(Config.WereWolfBowDrawingStrength, 0f, 10f);
        }

    }
    }


    










