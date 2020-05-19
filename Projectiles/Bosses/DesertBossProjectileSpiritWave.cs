using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace DesertMod.Projectiles.Bosses
{
    class DesertBossProjectileSpiritWave : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Wave");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 42;
            projectile.scale = 1.2f;

            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;

            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;

            projectile.timeLeft = 500;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            projectile.velocity.Y += projectile.ai[0];
        }
    }
}
