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
        private int[] windupWindow = new int[3];
        private int[] swingWindow = new int[3];
        private float currentSpeed = 0f;
        private int rotationOffsetIncrement;

        // Halberd movement variables
        private float windupSpeed = 2f;
        private float windupAcceleration = 0.1f;
        private float windupDeceleration = 0.02f;
        private float constantWindupSpeedDistance = 20f;

        private float swingSpeed = -15f;
        private float swingAcceleration = -1f;
        private float swingDeceleration = -1f;
        private float constantSwingSpeedDistance = 90f;

        private float distanceFromCenter = 150f;
        private float swingRotationOffset = 90f;

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
            if (aiPhase == 0) InitializeValues();

            double rad = halberdRotation * (Math.PI / 180);

            // Rotate around boss
            projectile.position.X = npc.Center.X - (int)(Math.Cos(rad) * distanceFromCenter) - projectile.width / 2;
            projectile.position.Y = npc.Center.Y - (int)(Math.Sin(rad) * distanceFromCenter) - projectile.height / 2;

            // Rotate projectile sprite
            Vector2 bossToProjectile = projectile.Center - npc.Center;
            bossToProjectile.Normalize();
            projectile.rotation = -(float)(Math.Atan2(-bossToProjectile.Y, bossToProjectile.X) + Math.PI);

            // Modify speed depending on movement values
            if (aiPhase < windupWindow[0])
            {
                currentSpeed += windupAcceleration;
                halberdRotation += currentSpeed;
            }
            else if (aiPhase < windupWindow[1])
            {
                halberdRotation += windupSpeed;
            }
            else if (aiPhase < windupWindow[2])
            {
                currentSpeed -= windupDeceleration;
                halberdRotation += currentSpeed;
            }
            else if (aiPhase < swingWindow[0])
            {
                currentSpeed += swingAcceleration;
                halberdRotation += currentSpeed;
            }
            else if (aiPhase < swingWindow[1])
            {
                halberdRotation += swingSpeed + rotationOffsetIncrement;
            }
            else if (aiPhase < swingWindow[2])
            {
                currentSpeed -= swingDeceleration;
                halberdRotation += currentSpeed;
            }
            else
            {
                projectile.timeLeft = 0;
            } 

            // Add tick to ai phase
            aiPhase++;
        }

        private void InitializeValues()
        {
            npc = Main.npc[(int)projectile.ai[0]];

            GetWindow(windupWindow, windupSpeed, windupAcceleration, windupDeceleration, constantWindupSpeedDistance);
            GetWindow(swingWindow, swingSpeed, swingAcceleration, swingDeceleration, constantSwingSpeedDistance, windupWindow[2]);
            rotationOffsetIncrement = rotationOffsetIncrement / swingWindow[1];
        }

        private void GetWindow(int[] window, float speed, float acceleration, float deceleration, float constDistance, int offset = 0)
        {
            float windupAccelerationTime = (speed / acceleration);
            float windupDecelerationTime = (speed / deceleration);
            float constantSpeedTime = (int)(constDistance / Math.Abs(speed));
            window[0] = offset + (int)windupAccelerationTime;
            window[1] = window[0] + (int)constantSpeedTime;
            window[2] = window[1] + (int)windupDecelerationTime;
        }
    }
}
