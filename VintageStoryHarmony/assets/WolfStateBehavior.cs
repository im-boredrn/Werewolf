using Vintagestory.API.Common;      // Core API, ModSystem, EntityBehavior
using Vintagestory.API.Server;      // ICoreServerAPI, IServerPlayer, server-side events
using Vintagestory.API.Client;      // Optional, only if you do client-side stuff like graphics/sounds
using Vintagestory.API.MathTools;   // For Vec3d, positions, etc.
using Vintagestory.API.Config;      // For Lang.Get() or game config
using Vintagestory.API.Common.Entities;

namespace VintageStoryHarmony.assets
{
    public class WolfStateBehavior : EntityBehavior
    {
        public static bool IsWerewolf;

        public WolfStateBehavior(Entity entity) : base(entity) { }

        // Toggle werewolf mode and adjust stats

        public void ToggleWolf(ICoreAPI api)
        {
            IsWerewolf = !IsWerewolf;
            entity.World.Api.Logger.Notification("Werewolf mode = " + IsWerewolf);
            api.Logger.Notification($"Toggle called. IsWerewolf = {IsWerewolf}");
            ApplyStats();
        }

        private void ApplyStats()
        {
            if (IsWerewolf)
            {
                entity.Stats.Set("walkspeed", "wolf", 2.5f, true);
                entity.Stats.Set("jumppower", "wolf", 2.5f, true);
                entity.Stats.Set("healingeffectivness", "wolf", 2.5f, true);
            }
            else
            {
                entity.Stats.Remove("walkspeed", "wolf");
                entity.Stats.Remove("jumppower", "wolf");
                entity.Stats.Remove("healingeffectivness", "wolf");
            }
        }
        public override string PropertyName() => "wolfState";
    }
}