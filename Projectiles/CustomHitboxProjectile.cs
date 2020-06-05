using Terraria;
using Terraria.ModLoader;

namespace DesertMod.Projectiles
{
    public class CustomHitboxProjectile : ModProjectile
    {
        public override bool CanHitPlayer(Player target)
        {
            return CanHit(target);
        }

        public override bool CanHitPvp(Player target)
        {
            return CanHit(target);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return CanHit(target);
        }

        public bool CanHit(Entity entity)
        {
            if (Collides(entity))
            {
                return true;
            }
            return false;
        }

        public virtual bool Collides(Entity entity)
        {
            return true;
        }
    }
}