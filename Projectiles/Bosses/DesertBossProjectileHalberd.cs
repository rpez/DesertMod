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
        private int aiUpdatesPerSecond = 60;

        // Halberd state values
        private float halberdRotation;
        private float alpha = 0.0f;

        private NPC npc;
        private float windupRadians;
        private float swingRadians;
        private int windupWindow;
        private int swingWindow;

        // Halberd movement variables
        private float windupSpeed = 1f;
        private float windupAcceleration = 0f;
        private float windupDeceleration = 0f;
        private float windupAngle = 90f;

        private float swingSpeed = -15f;
        private float swingAcceleration = 0f;
        private float swingDeceleration = 0f;
        private float swingAngle = 270f;

        private float distanceFromCenter = 150f;
        private float swingRotationOffset = 0f;

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

            projectile.timeLeft = 1000;
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
            if (aiPhase == 0) CalculateValues();

            double rad = halberdRotation * (Math.PI / 180);

            if (aiPhase < windupWindow)
            {
                // Rotate around boss
                projectile.position.X = npc.Center.X - (int)(Math.Cos(rad) * distanceFromCenter) - projectile.width / 2;
                projectile.position.Y = npc.Center.Y - (int)(Math.Sin(rad) * distanceFromCenter) - projectile.height / 2;

                // Rotate projectile sprite
                Vector2 bossToProjectile = projectile.Center - npc.Center;
                bossToProjectile.Normalize();
                projectile.rotation = -(float)(Math.Atan2(-bossToProjectile.Y, bossToProjectile.X) + Math.PI);

                halberdRotation += windupSpeed;
            }

            if (aiPhase > windupWindow)
            {
                // Rotate around boss
                projectile.position.X = npc.Center.X - (int)(Math.Cos(rad) * distanceFromCenter) - projectile.width / 2;
                projectile.position.Y = npc.Center.Y - (int)(Math.Sin(rad) * distanceFromCenter) - projectile.height / 2;

                // Rotate projectile sprite
                Vector2 bossToProjectile = projectile.Center - npc.Center;
                bossToProjectile.Normalize();
                projectile.rotation = -(float)(Math.Atan2(-bossToProjectile.Y, bossToProjectile.X) + Math.PI);

                halberdRotation += swingSpeed;
            }

            if (aiPhase > windupWindow + swingWindow)
            {
                projectile.timeLeft = 0;
            }

            aiPhase++;
        }

        private void CalculateValues()
        {
            npc = Main.npc[(int)projectile.ai[0]];
            windupRadians = windupAngle * 180f / (float)Math.PI;
            swingRadians = swingAngle * 180f / (float)Math.PI;
            windupWindow = (int) (windupAngle / Math.Abs(windupSpeed));
            swingWindow = (int) (swingAngle / Math.Abs(swingSpeed));
        }
    }
}
