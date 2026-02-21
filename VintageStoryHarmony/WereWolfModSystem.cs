using HarmonyLib;
using System;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using VintageStoryHarmony.assets;
using WereWolf.assets.Werewolf;
using WereWolf.assets.Werewolf.Configuration;
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

        private float regenDebugTimer = 0f;

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


            Config = api.LoadModConfig<WerewolfConfig>("werewolf-config.json");

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
                    WereWolfRegen = 1f,
                    WereWolfRangedAcc = 0.1f,
                    WereWolfDisableFallDamage = true,
                    WereWolfForageDropRate = 2f,
                    WereWolfWildCropDropRate = 2f,
                    WereWolfAnimalSeekingRange = 2f,
                    WereWolfAnimalLootDropRate = 2f,
                    WereWolfAnimalHarvestingTime = 2f,
                    WereWolfBowDrawingStrength = 2f,
                    WereWolfNightRegen = 0.25f,
                    WereWolfDayRegen = 0.05f
                };

                api.StoreModConfig(Config, "werewolf-config.json");
                api.Logger.Notification("Werewolf default config created!");
            }


            Mod.Logger.Notification("WEREWOLF MOD LOADED!");


            api.ChatCommands.Create("reloadwerewolfconfig")
               .WithDescription("reload werewolf config without reloading world")
               .RequiresPrivilege("chat") // allows any player with chat access
               .HandleWith((args) =>
               {
                   WereWolfModSystem.Config = api.LoadModConfig<WerewolfConfig>("werewolf-config.json");


                   return TextCommandResult.Success("Werewolf config reloaded!");





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

                // LOG SPAMMERS JUST FOR TESTING   sapi?.Logger.Warning($"After transform, PlayerData.Form = {PlayerData.GetForm(entity)}");
                // LOG SPAMMERS JUST FOR TESTING   sapi?.Logger.Warning($"ManualActive: {WereWolfModSystem.Instance?.ManualFormActive} | ManualForm: {WereWolfModSystem.Instance?.ManualForm}");

                // Regeneration For Wolf

                if (PlayerData.GetForm(entity) == PlayerData.Forms.WereWolf)
                {
                    ApplyRegen(entity, dt);
                }




            }



        }

        private void ApplyRegen(EntityPlayer entity, float dt)
        {

            bool night = WolfTime.isNight(entity);

            float currentHealth = entity.Stats.GetBlended("currenthealth");
            float maxHealth = entity.Stats.GetBlended("maxhealth");

            if (currentHealth >= maxHealth) return;

            // Regen per second from config
            float regenPerSecond = night ? WereWolfModSettings.NightRegen : WereWolfModSettings.DayRegen;
            float healAmount = regenPerSecond * dt;

            // Apply healing via negative damage to properly update engine and sync to clients
            entity.ReceiveDamage(new Vintagestory.API.Common.DamageSource() { SourceEntity = null }, -healAmount);

            // Optional: debug log every 1s
            regenDebugTimer += dt;
            if (regenDebugTimer >= 1f)
            {
                regenDebugTimer = 0f;
                sapi?.Logger.Notification($"[WerewolfRegen] {entity.Player?.PlayerName} HP: {entity.Stats.GetBlended("health")}/{maxHealth} | Night: {night}");
            }

                }
            
        }
    }










