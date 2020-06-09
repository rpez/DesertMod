using Terraria;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace DesertMod.Projectiles.Bosses
{
    class DesertBossProjectileHalberd : CustomHitboxProjectile
    {
        // Halberd state values
        private int aiPhase = 0;
        private float halberdRotation;
        private float halberdRotationOffset;
        private float alpha = 0.0f;

        // Initialized in the code
        private NPC npc;
        private int[] windupWindow = new int[3];
        private int[] swingWindow = new int[3];
        private float currentSpeed = 0f;
        private float rotationOffsetIncrement;
        private int fadeOutThreshold;
        private float fadeIncrement;
        private float extensionIncrement;

        // Hitbox
        private Vector2 bladeHitboxSize;
        private float bladeHitboxOffset;
        private Vector2 offSetDirection;
        private float bladeHitBoxScaleDivisor = 4f;

        /* 
         * ATTACK ANIMATION VARIABLES (Modify these to alter the movement of the attack)
         * Both the windup and the swing part of the attack have 3 stages: Acceleration, Constant Speed and Deceleration
         * First the acceleration will be applied until the speed is reached, after which the constant speed is applied
         * as long as it takes to reach the constantSpeedDistance. Lastly, the deceleration is applied until speed is 0.
         * ISSUE: Acceleration and deceleration may override constant speed, making the constant speed window size of 0.
         */
        private float windupSpeed = 2f;
        private float windupAcceleration = 0.2f;
        private float windupDeceleration = 0.02f;
        private float constantWindupSpeedDistance = 15f;

        private float swingSpeed = 30f;
        private float swingAcceleration = 3f;
        private float swingDeceleration = 2f;
        private float constantSwingSpeedDistance = 20f;

        private bool leftToRight = true; // In which direction the attack will go
        private float distanceFromCenter = 150f; // How far the projectile is from the npc
        private float extensionDistance = 100f; // How far the projectile will reach from the npc during the swing
        private float swingRotationOffset = 45f; // How much the blade will tilt during the swing
        private int fadeTime = 50; // How much time the fade in and fade out will take

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Blade");
            Main.projFrames[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 398;
            projectile.height = 398;
            projectile.scale = 1.0f;

            projectile.aiStyle = -1;

            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;

            projectile.timeLeft = 1000;
        }

        public override bool Collides(Entity entity)
        {
            return Collides(bladeHitboxOffset, offSetDirection, new Vector2(projectile.position.X + projectile.width / 2, projectile.position.Y + projectile.height / 2), bladeHitboxSize, entity.position, entity.Size);
        }

        // Check custom collision
        public bool Collides(float offset, Vector2 offsetDir, Vector2 projectilePos, Vector2 projectileDim, Vector2 boxPos, Vector2 boxDim)
        {
            Vector2 newOrigin = projectilePos + offsetDir * offset;
            Vector2 leftTop = new Vector2(boxPos.X - boxDim.X / 2, boxPos.Y - boxDim.Y / 2);
            Vector2 rightTop = new Vector2(boxPos.X + boxDim.X / 2, boxPos.Y - boxDim.Y / 2);
            Vector2 leftBottom = new Vector2(boxPos.X - boxDim.X / 2, boxPos.Y + boxDim.Y / 2);
            Vector2 rightBottom = new Vector2(boxPos.X + boxDim.X / 2, boxPos.Y + boxDim.Y / 2);
            return PointIsInRectangle(newOrigin, projectileDim, leftTop)
                || PointIsInRectangle(newOrigin, projectileDim, rightTop)
                || PointIsInRectangle(newOrigin, projectileDim, leftBottom)
                || PointIsInRectangle(newOrigin, projectileDim, rightBottom);
        }

        // Return true if point is in rectangle, else false
        private bool PointIsInRectangle(Vector2 origin, Vector2 recDim, Vector2 point)
        {
            bool inXRight = point.X < origin.X + recDim.X / 2;
            bool inXLeft = point.X > origin.X - recDim.X / 2;
            bool inYBottom = point.Y < origin.Y + recDim.Y / 2;
            bool inYTop = point.Y > origin.Y - recDim.Y / 2;
            return inXRight && inXLeft && inYBottom && inYTop;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            // Fade in and out for the projectile
            if (aiPhase <= fadeTime)
            {
                alpha += fadeIncrement;
                return new Color(alpha, alpha, alpha, alpha);
            }
            else if (aiPhase > fadeOutThreshold)
            {
                alpha -= fadeIncrement;
                return new Color(alpha, alpha, alpha, alpha);
            }
            else
                return Color.White;
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
            float oneEighty = leftToRight ? (float)Math.PI : 0f;
            projectile.rotation = -(float)(Math.Atan2(-bossToProjectile.Y, bossToProjectile.X) + oneEighty + halberdRotationOffset / 180f * Math.PI);

            // Projectile hitbox offset direction
            offSetDirection = new Vector2(-bossToProjectile.Y, bossToProjectile.X);

            // Modify speed depending on movement values
            // windupWindow and swingWindow include the different aiPhase thresholds for the attack phases respectively
            // TODO: Better DRY-code
            if (aiPhase < windupWindow[0])
            {
                currentSpeed += windupAcceleration;
                halberdRotation += currentSpeed;
            }
            else if (aiPhase < windupWindow[1])
            {
                currentSpeed = windupSpeed;
                halberdRotation += currentSpeed;
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

                halberdRotationOffset += rotationOffsetIncrement;
                distanceFromCenter += extensionIncrement;
            }
            else if (aiPhase < swingWindow[1])
            {
                currentSpeed = swingSpeed;
                halberdRotation += currentSpeed;

                halberdRotationOffset += rotationOffsetIncrement;
                distanceFromCenter += extensionIncrement;
            }
            else if (aiPhase < swingWindow[2])
            {
                currentSpeed -= swingDeceleration;
                halberdRotation += currentSpeed;

                halberdRotationOffset += rotationOffsetIncrement;
                distanceFromCenter += extensionIncrement;
            }
            else if (aiPhase < fadeOutThreshold + fadeTime)
            {
                // Empty for fade out purposes, possibly implement other effects later
            }
            else
            {
                projectile.timeLeft = 0;
            }

            // Add tick to ai phase
            aiPhase++;
        }

        // Initializes one-time calculated values
        private void InitializeValues()
        {
            npc = Main.npc[(int)projectile.ai[0]]; // Reference to the boss
            leftToRight = (int)projectile.ai[1] == 0; // Attack direction (0 = left to right, 1 = rigt to left)

            projectile.frame = leftToRight ? 0 : 1;

            distanceFromCenter = leftToRight ? distanceFromCenter : -distanceFromCenter;
            extensionDistance = leftToRight ? extensionDistance : -extensionDistance;

            windupSpeed = leftToRight ? windupSpeed : -windupSpeed;
            windupAcceleration = leftToRight ? windupAcceleration : -windupAcceleration;
            windupDeceleration = leftToRight ? windupDeceleration : -windupDeceleration;

            swingSpeed = leftToRight ? -swingSpeed : swingSpeed;
            swingAcceleration = leftToRight ? -swingAcceleration : swingAcceleration;
            swingDeceleration = leftToRight ? -swingDeceleration : swingDeceleration;

            swingRotationOffset = leftToRight ? swingRotationOffset : -swingRotationOffset;

            GetWindow(windupWindow, windupSpeed, windupAcceleration, windupDeceleration, constantWindupSpeedDistance);
            GetWindow(swingWindow, swingSpeed, swingAcceleration, swingDeceleration, constantSwingSpeedDistance, windupWindow[2]);

            rotationOffsetIncrement = swingRotationOffset / (float)(swingWindow[2] - windupWindow[2]);
            extensionIncrement = extensionDistance / (float)(swingWindow[2] - windupWindow[2]);
            fadeIncrement = 1f / (float)fadeTime;
            fadeOutThreshold = swingWindow[2];

            bladeHitboxSize = new Vector2(projectile.Size.Y / bladeHitBoxScaleDivisor, projectile.Size.Y / bladeHitBoxScaleDivisor);
            bladeHitboxOffset = bladeHitboxSize.Y * (bladeHitBoxScaleDivisor - 1f) / 2f;
        }

        // Calculates the frame windows for the different attack phases
        private void GetWindow(int[] window, float speed, float acceleration, float deceleration, float constDistance, int offset = 0)
        {
            float accelerationTime = Math.Abs(speed) / Math.Abs(acceleration);
            float decelerationTime = Math.Abs(speed) / Math.Abs(deceleration);
            float constantSpeedTime = constDistance / Math.Abs(speed);
            window[0] = offset + (int)accelerationTime;
            window[1] = window[0] + (int)constantSpeedTime;
            window[2] = window[1] + (int)decelerationTime;
        }
    }
}
