using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace DesertMod.Projectiles.Bosses
{
    class DesertBossProjectileSpiritDagger : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Dagger");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 42;
            projectile.scale = 1.0f;

            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;

            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;

            projectile.timeLeft = 500;

            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (projectile.damage == 0) return new Color(0.2f, 0.2f, 0.2f, 0.2f);
            else return Color.White;
        }

        public override void AI()
        {
            projectile.velocity.Y += projectile.ai[0];
            if (projectile.damage != 0) Dust.NewDust(projectile.position, projectile.width, projectile.height, 217,
                projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default(Color), 1f);
            Lighting.AddLight(projectile.Center, 0.3f, 1f, 1f);
        }
    }
}
