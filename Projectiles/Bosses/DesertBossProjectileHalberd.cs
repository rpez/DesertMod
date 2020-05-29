using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.Projectiles.Bosses
{
    class DesertBossProjectileHalberd : ModProjectile
    {
        private int aiPhase = 0;

        private float halberdRotation;
        private float rotationSpeed = -1f;
        private float alpha = 0.0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Blade");
        }

        public override void SetDefaults()
        {
            projectile.width = 398;
            projectile.height = 76;
            projectile.scale = 1.0f;

            projectile.aiStyle = -1;

            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;

            projectile.timeLeft = 500;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            //if (aiPhase <= 50)
            //{
            //    alpha += 0.02f;
            //    return new Color(alpha, alpha, alpha, alpha);
            //}
            //else 
            return Color.White;
        }

        public override void AI()
        {
            NPC npc = Main.npc[(int)projectile.knockBack];

            // Factors for calculations
            double deg = halberdRotation; // The degrees
            double rad = deg * (Math.PI / 180); // Convert degrees to radians
            double dist = 200; // Distance away from the npc

            projectile.position.X = npc.Center.X - (int)(Math.Cos(rad) * dist);
            projectile.position.Y = npc.Center.Y - (int)(Math.Sin(rad) * dist);

            halberdRotation += rotationSpeed;

            aiPhase++;

            //projectile.rotation = halberdRotation;

            //if (aiPhase <= 50)
            //{
            //    projectile.velocity *= 0.95f;
            //}

            //else if (aiPhase > 50)
            //{
            //    if (projectile.velocity.Length() <= 30f) projectile.velocity *= 1.05f;
            //    halberdRotation += rotationSpeed;
            //    //Dust.NewDust(projectile.position, projectile.width, projectile.height, 217,
            //    //    projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default(Color), 1f);
            //    //Lighting.AddLight(projectile.Center, 0.3f, 1f, 1f);
            //}
        }
    }
}
