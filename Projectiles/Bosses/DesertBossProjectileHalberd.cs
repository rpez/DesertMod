using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace DesertMod.Projectiles.Bosses
{
    class DesertBossProjectileHalberd : ModProjectile
    {
        private int aiPhase = 0;

        private float halberdRotation;
        private float rotationSpeed = -0.2f;
        private float alpha = 0.0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Blade");
        }

        public override void SetDefaults()
        {
            projectile.width = 76;
            projectile.height = 398;
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

            halberdRotation = projectile.rotation;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (aiPhase <= 50)
            {
                alpha += 0.02f;
                return new Color(alpha, alpha, alpha, alpha);
            }
            else return Color.White;
        }

        public override void AI()
        {
            aiPhase++;

            if (aiPhase <= 50)
            {
                projectile.velocity *= 0.95f;
            }

            else if (aiPhase > 50)
            {
                projectile.velocity *= 1.05f;
                halberdRotation += rotationSpeed;
                projectile.rotation = halberdRotation;
                //Dust.NewDust(projectile.position, projectile.width, projectile.height, 217,
                //    projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default(Color), 1f);
                //Lighting.AddLight(projectile.Center, 0.3f, 1f, 1f);
            }
        }
    }
}
