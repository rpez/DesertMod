using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.Projectiles
{
    class DebugProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Dagger");
        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;

            projectile.aiStyle = -1;
            aiType = ProjectileID.Bullet;

            projectile.friendly = true; 
            projectile.hostile = false;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;

            projectile.timeLeft = 20;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return projectile.ai[0] == 0 ? Color.Red : Color.Blue;
        }

        public override void AI()
        {

        }
    }
}
