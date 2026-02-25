using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using WereWolf.assets.Coresystems;
using static WereWolf.assets.Coresystems.PlayerData;

namespace WereWolf.assets.Coresystems
{
    internal static class Infection
    {
        public enum Infectionstatus
        {
            None,
            Infected
        }

        // Keys for storing data on the player
        public const string infectionStatusKey = "infected";
        public const string infectionLevelKey = "infectionLevel";

        // Threshold at which a player becomes infected
        private const int InfectionThreshold = 10;

        // --- Helpers for infection level ---
        public static int GetInfectionLevel(EntityPlayer player)
        {
            return player.WatchedAttributes.GetInt(infectionLevelKey, 0);
        }

        public static void SetInfectionLevel(EntityPlayer player, int level)
        {
            player.WatchedAttributes.SetInt(infectionLevelKey, level);
            player.WatchedAttributes.MarkPathDirty(infectionLevelKey);
        }

        // --- Helpers for infection status ---
        public static Infectionstatus CurrentInfection(EntityPlayer player)
        {
            string stored = player.WatchedAttributes.GetString(infectionStatusKey, Infectionstatus.None.ToString());
            bool success = Enum.TryParse<Infectionstatus>(stored, out var infection);
            return success ? infection : Infectionstatus.None;
        }

        private static void SetInfectionStatus(EntityPlayer player, Infectionstatus status)
        {
            player.WatchedAttributes.SetString(infectionStatusKey, status.ToString());
            player.WatchedAttributes.MarkPathDirty(infectionStatusKey);
        }

        // --- Apply infection from an attacker ---
        public static void ApplyInfection(EntityPlayer target, Entity attacker, int amount = 10)
        {
            if (target == null || attacker == null) return;

            string attackerCode = attacker.Code?.ToString() ?? "unknown";

            int currentLevel = GetInfectionLevel(target);

            // Example: only wolves cause infection
            if (attackerCode == "wolf")
            {
                currentLevel += amount;
                SetInfectionLevel(target, currentLevel);
            }

            // Update infection status if threshold reached
            if (currentLevel >= InfectionThreshold)
            {
                SetInfectionStatus(target, Infectionstatus.Infected);
            }
        }

        // --- Get infection status, optionally apply attacker effect ---
        public static Infectionstatus GetInfection(Entity entity, Entity? attacker = null)
        {
            var player = entity as EntityPlayer;
            if (player == null) return Infectionstatus.None;

            // Apply infection from attacker if provided
            if (attacker != null)
            {
                ApplyInfection(player, attacker);
            }

            return CurrentInfection(player);
        }

        // --- Process infection effects, like transforming forms ---
        public static void ProcessInfection(EntityPlayer player)
        {
            if (player == null) return;

            var form = PlayerData.GetForm(player);

            if (GetInfection(player) == Infectionstatus.Infected && form == Forms.UnchangedHuman)
            {
                SetForm(player, Forms.VulpisHuman);
            }
        }
    }
}