using HarmonyLib;
using System;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Vintagestory;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Client.NoObf;
using Vintagestory.GameContent;
using WereWolf.assets.Coresystems;
using WereWolf.assets.Coresystems.Infections;
using WereWolf.assets.Coresystems.StatRelated;
using WereWolf.assets.Keybinds;
using WereWolf.assets.Werewolf.Configuration;
using static WereWolf.assets.Coresystems.PlayerData;

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
        private long clienttickListenerId;
        public bool ManualFormActive = false;
        public Forms ManualForm;
        private Forms lastForm;
        private bool initialized = false;


        public static WerewolfConfig? Config;

        public static WereWolfModSystem? Instance; // optional for easy access

        private IClientNetworkChannel? clientChannel;
        private IServerNetworkChannel? serverChannel;




        public const string PLUGIN_GUID = "kortah.werewolf";
        public const string PLUGIN_NAME = "Werewolf!";

        // private WolfVisionRenderer? wolfVisionRenderer;

        public override void Start(ICoreAPI api)
        {

            Instance = this;


            harmony = new Harmony(PLUGIN_GUID);
            harmony.PatchAll();

            base.Start(api);


            //Shader Loading






            // Config

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
                    WereWolfTransformCoolDown = 5,
                    WereWolfEnabledMinBrightness = 0.8f
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




        public override void StartServerSide(ICoreServerAPI api) // SERVER SIDE
        {
            Mod.Logger.Notification("Server:WEREWOLF MOD LOADED !");

            sapi = api;
            // Tick Listener
            tickListenerId = api.Event.RegisterGameTickListener(OnServerTick, 1000);
            api.Logger.Notification($"WEREWOLF CONFIG LOADED");

            serverChannel = sapi.Network.RegisterChannel("werewolf:toggleform")
       .RegisterMessageType<ToggleFormPacket>()
       .SetMessageHandler<ToggleFormPacket>((player, packet) =>
       {

           TransformationController.TrySetForm(player, packet.TargetForm, TransformationReason.ManualToggle);
           sapi.Logger.Warning($"Server received toggle packet. TargetForm = {packet.TargetForm}");

       });

            api.Event.PlayerJoin += (IServerPlayer player) =>
            {
                sapi.Event.EnqueueMainThreadTask(() =>
                {
                    if (!player.Entity.HasBehavior<EntityBehaviorInfection>())
                    {
                        player.Entity.AddBehavior(new EntityBehaviorInfection(player.Entity));
                        sapi.Logger.Notification($"Infection behavior attached to {player.PlayerName}");
                        var form = PlayerData.GetForm(player.Entity);
                        StatsManager.ApplyStats(player.Entity, form);
                    }
                }, "AttachInfectionBehavior, InitialStatsApply");
            };
        
        }


        public override void StartClientSide(ICoreClientAPI api) // CLIENT SIDE

        {
            Mod.Logger.Notification("Client:WEREWOLF MOD LOADED !");
            Capi = api;


            api.Logger.Warning($"KEYBIND ran on: {api.Side}");


            clienttickListenerId = api.Event.RegisterGameTickListener(OnClientTick, 1000);   // client tick listener

            api.Input.RegisterHotKey("toggleform", "Toggle Form", GlKeys.G, HotkeyType.CharacterControls);

            api.Input.SetHotKeyHandler("toggleform", (G) =>
            {
                api.Logger.Warning("Keybind pressed!");

                var entity = api.World.Player?.Entity;
                if (entity == null)
                {
                    api.Logger.Warning("Keybind pressed but entity is null!");
                    return false;
                }

                api.Logger.Warning("Keybind pressed! Player entity detected.");
                // Determine target form: toggle between WereWolf and VulpisHuman
                var currentForm = PlayerData.GetForm(entity);
                var targetForm = currentForm == PlayerData.Forms.WereWolf
                                 ? PlayerData.Forms.VulpisHuman
                                 : PlayerData.Forms.WereWolf;

                api.Logger.Warning($"Determined target form: {targetForm}");


                // Send packet to server requesting transformation
                SendToggleFormPacket(targetForm);

                api.Logger.Warning($"Sending toggle form packet. TargetForm = {targetForm}");

                return true; // key handled / consumed

            });

            clientChannel = Capi.Network.RegisterChannel("werewolf:toggleform")
                   .RegisterMessageType<ToggleFormPacket>();


            // api.Event.RegisterRenderer(wolfVisionRenderer, EnumRenderStage.AfterPostProcessing, "WolfVision", 0.85, 0.85, typeof(WolfVisionRenderer));




        }

        private void OnServerTick(float dt)
        {

            foreach (IServerPlayer player in sapi?.World.AllOnlinePlayers ?? Array.Empty<IServerPlayer>())
            {
                var entity = player?.Entity as EntityPlayer;
                if (entity == null || entity.World == null) continue; // skip if null

              //  entity.World.Logger.Warning($"Player {entity.GetName()} spawned, entity code: {entity.Code?.Path}, pos: {entity.Pos}");

                bool night = WolfTime.isNight(entity);
                bool day = !night;

                var beastform = PlayerData.GetForm(entity);
                // Safe logging
                // LOG SPAMMERS JUST FOR TESTING  sapi?.Logger.Warning($"Hour: {entity.World.Calendar?.HourOfDay ?? -1} | Night: {night}");

                entity.World.Logger.Warning($"SERVER sees form: {PlayerData.GetForm(entity)}");
             
                TransformationController.ProcessTransformation(player, dt);
                if (PlayerData.GetForm(entity) != Forms.UnchangedHuman)
                {
                    Regen.ApplyRegen(entity);
                }

            }



        }
        private void OnClientTick(float dt)
        {


            var mod = WereWolfModSystem.Instance;
            var entity = mod?.Capi?.World.Player?.Entity as EntityPlayer;
            if (entity == null) return;

          //  entity.World.Logger.Warning($"Player {entity.GetName()} spawned, entity code: {entity.Code?.Path}, pos: {entity.Pos}");
          //  entity.World.Logger.Warning($"CLIENT tick, checking for entity {entity.GetName()}");

            var form = PlayerData.GetForm(entity);

            if (!initialized)
            {
                lastForm = form;
                initialized = true;
                return;
            }

            if (form != lastForm)
            {
                if (form == Forms.WereWolf)
                    PlayPlayerSound("sounds/werewolf/werewolf-transformation1", entity, 0.8f, 1f);
                else
                    PlayPlayerSound("sounds/human/human-transform1", entity, 1f, 1.1f);

                lastForm = form;
            }

            Capi?.Logger.Warning($"CLIENT sees form: {form}");


        }
        void SendToggleFormPacket(PlayerData.Forms targetForm)
        {
            var packet = new ToggleFormPacket { TargetForm = targetForm };
       //     Capi?.Logger.Warning($"Sending toggle form packet. TargetForm = {targetForm}");

            clientChannel?.SendPacket(packet); 
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
            Config.WereWolfEnabledMinBrightness = Math.Clamp(Config.WereWolfEnabledMinBrightness, 0f, 1f);
        }

      public void PlayPlayerSound(string soundCode, EntityPlayer entity, float pitchMin, float pitchMax)
        {
            var capi = WereWolfModSystem.Instance?.Capi;
            var X = entity.Pos.X;
            var Y = entity.Pos.Y;
            var Z = entity.Pos.Z;

            if (capi == null) return;
         
            float pitch = pitchMin + (float)capi.World.Rand.NextDouble() * (pitchMax - pitchMin);
            
            capi.World.PlaySoundAt(
                   new AssetLocation("vulpis:" + soundCode),
                   entity,
                   null,
                   false,
                   16f,
                   pitch
            ); 
      }
    }
}