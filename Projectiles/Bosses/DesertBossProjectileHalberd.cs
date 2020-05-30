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

        // Halberd movement variables
        private float halberdRotation;
        private float rotationSpeed = -1f;
        private float offset = 180f;

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

            projectile.aiStyle = -1;

            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;

            projectile.timeLeft = 500;
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
            bool start = aiPhase == 0;

            // The boss itself
            NPC npc = Main.npc[(int)projectile.ai[0]];

            // Factors for calculations
            double deg = halberdRotation; // The degrees
            double rad = deg * (Math.PI / 180); // Convert degrees to radians
            double dist = 200; // Distance away from the npc

            // Rotate around boss
            projectile.position.X = npc.Center.X - (int)(Math.Cos(rad) * dist) - projectile.width / 2;
            projectile.position.Y = npc.Center.Y - (int)(Math.Sin(rad) * dist) - projectile.height / 2;

            // Rotate projectile sprite
            Vector2 bossToProjectile = projectile.Center - npc.Center;
            bossToProjectile.Normalize();
            projectile.rotation = -(float)(Math.Atan2(-bossToProjectile.Y, bossToProjectile.X) + Math.PI);

            halberdRotation += rotationSpeed;

            aiPhase++;
        }
    }
}
