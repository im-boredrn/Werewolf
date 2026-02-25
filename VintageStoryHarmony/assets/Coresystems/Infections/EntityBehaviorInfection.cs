using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using static WereWolf.assets.Coresystems.PlayerData;

namespace WereWolf.assets.Coresystems.Infections
{

    internal class EntityBehaviorInfection : EntityBehavior
    {
        private bool DebugMode = true; // For Debug Mode 
        private EntityPlayer Player => (EntityPlayer)entity; // assignment operator is saying assign the value on the left to the value on the right.
        public EntityBehaviorInfection(Entity entity) : base(entity) // no need to pass Entityplayer entity anymore since we are attaching it to them.
        {
            if (entity.HasBehavior<EntityBehaviorInfection>()) return;

            if (entity.World.Side == EnumAppSide.Server)
            {
                (entity.World.Api as ICoreServerAPI)?.Logger.Warning("Infection behavior attached");
            }
        }

        public override void OnEntityReceiveDamage(DamageSource damageSource, ref float damage) // Onreceive Damage apply infection -- Infection Trigger
        {
            if (entity.World.Side != EnumAppSide.Server)
            {
                return;
            }
            ICoreServerAPI? sapi = entity.World.Api as ICoreServerAPI;

            ApplyInfection(damageSource.SourceEntity, 10);
            if(DebugMode)
                sapi?.SendMessage(
                    Player.Player,
                    GlobalConstants.GeneralChatGroup,
                    $"Current Infection Level is : {GetInfectionLevel()}",
                    EnumChatType.Notification
                );
            
        }
        public override void OnGameTick(float deltaTime) // Processing Logic
        {
            if (entity.World.Side != EnumAppSide.Server) return; // guard server side

            ICoreServerAPI? sapi = entity.World.Api as ICoreServerAPI;

            InfectionNight(); // Waits till next night to force change

            ProcessInfection();
            if (DebugMode)
                sapi?.Logger.Warning("Transform triggered");

        }




        public enum Infectionstatus // may Eventually turn into infection type 
        {
            None,
            Infected
        }

        // Keys for storing data on the player
        public const string infectionStatusKey = "infected";
        public const string infectionLevelKey = "infectionLevel";
        public  const string WasNightKey = "WasNight"; // may need to  assign a watched attribute


        // Threshold at which a player becomes infected
        private const int InfectionThreshold = 10;

        // --- Helpers for infection level ---
        public  int GetInfectionLevel()
        {
            return Player.WatchedAttributes.GetInt(infectionLevelKey, 0);
        }

        public bool GetLastNight()
        {
          return Player.WatchedAttributes.GetAsBool(WasNightKey, false);

        }

        public  void SetInfectionLevel( int level) // Never Directly use unless testing 
        {
            Player.WatchedAttributes.SetInt(infectionLevelKey, level);
            Player.WatchedAttributes.MarkPathDirty(infectionLevelKey);
        }

        // --- Helpers for infection status ---
        public  Infectionstatus CurrentInfection()
        {
            string stored = Player.WatchedAttributes.GetString(infectionStatusKey, Infectionstatus.None.ToString());
            bool success = Enum.TryParse<Infectionstatus>(stored, out var infection);
            return success ? infection : Infectionstatus.None;
        }

        private  void SetInfectionStatus(Infectionstatus status)
        {
            Player.WatchedAttributes.SetString(infectionStatusKey, status.ToString());
            Player.WatchedAttributes.MarkPathDirty(infectionStatusKey);
        }

        // --- Apply infection from an attacker ---
        public  void ApplyInfection(Entity attacker, int amount = 10) // Mutation
        {
            ICoreServerAPI? sapi = entity.World.Api as ICoreServerAPI; // Message

            if ( attacker == null) return;

            string attackerCode = attacker.Code?.Path ?? "unknown";

            int currentLevel = GetInfectionLevel();
            int previousLevel = currentLevel;

            // Example: only wolves cause infection
            if (attacker.Code.Path.Contains("wolf"))
            {
                currentLevel = Math.Min(currentLevel + amount, 100);
                SetInfectionLevel(currentLevel);
            }
            if (DebugMode)
            {
                sapi?.Logger.Warning($"Attacker raw code: {attacker.Code}");
                sapi?.Logger.Warning($"Attacker path: {attacker.Code?.Path}");
                sapi?.Logger.Warning($"Attacker code: {attacker.Code?.Path}, Infection level: {GetInfectionLevel()}");
            }
             
            // if (attackerCode == "Bear") You See How I can scale this later

            // Update infection status if threshold reached
            if (previousLevel < InfectionThreshold && currentLevel >= InfectionThreshold)
            {
                SetInfectionStatus(Infectionstatus.Infected);

                sapi?.SendMessage(
                    Player.Player,
                    GlobalConstants.GeneralChatGroup,
                    "You feel sick...",
                    EnumChatType.Notification

                );
            }
            if (DebugMode)
            {
                sapi?.Logger.Warning($"Attacker: {attackerCode}, Level: {currentLevel}");

                sapi.Logger.Warning(
    $"Hit by: {attacker?.Code?.Path ?? "null"}, PrevLvl: {previousLevel}, NewLvl: {currentLevel}, Threshold: {InfectionThreshold}, Status: {CurrentInfection()}");
            }

        }

        // --- Get infection status, optionally apply attacker effect ---
        public  Infectionstatus GetInfection()
        {            
            return CurrentInfection();
        }

        // --- Process infection effects, like transforming forms ---
        public  void ProcessInfection() 
        {
            ICoreServerAPI? sapi = entity.World.Api as ICoreServerAPI; // Message

            var form = PlayerData.GetForm(Player);

            if (GetInfection() == Infectionstatus.Infected && form == Forms.UnchangedHuman)
            {
                SetForm(Player, Forms.VulpisHuman);
            }
            if (DebugMode)
                sapi?.Logger.Warning($"Processing infection. Status: {GetInfection()}");
        }

        public void InfectionNight()
        {
            ICoreServerAPI? sapi = entity.World.Api as ICoreServerAPI; // Message

            bool lastNight = GetLastNight();
          bool currentNight = WolfTime.isNight(Player);

           if (!lastNight && currentNight)
         {
            //   First tick of night > transform infected human to wolf
            if (GetInfection() == Infectionstatus.Infected && GetForm(Player) == Forms.VulpisHuman)
                {
                   SetForm(Player, Forms.WereWolf);
              }
            }
            // ALWAYS update tracker
            Player.WatchedAttributes.SetBool(WasNightKey, currentNight);
            Player.WatchedAttributes.MarkPathDirty(WasNightKey);
            if (DebugMode)
                sapi?.Logger.Warning($"Night check. Was: {lastNight}, Now: {currentNight}");
        }
        public override string PropertyName()
        {
            return "infection";
        }
    } 
}