using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DesertMod.Projectiles.Boss
{
    class DesertBossProjectileFreezeRay : ModProjectile
    {
        private int aiPhase = 0;

        // Ray functional variables
        private Player target;
        private NPC npc;
        private Vector2 freezePos;

        // RAY ANIMATION VARIABLES

        private int rayPartVariation = 10; // How many randomized values in the lists

        // Lists for randomized values
        private List<float> rayPartRotation = new List<float>();
        private List<float> rayPartRotationIncrement = new List<float>();
        private List<Vector2> rayPartMovement = new List<Vector2>();
        private List<Vector2> rayPartMovementIncrement = new List<Vector2>();

        private int rotationChangeCounter = 0; // Counter for randomization change
        private bool moveBack = false; // Prevents parts from going astray
        private List<int> pulsePositions = new List<int>(); // Pulse indexes
        private int pulseFrequency = 60; // How often pulses are fired
        private int pulseTickSpeed = 2; // How fast pulses are (less is faster)
        private int pulseWidth = 3; // How wide pulses are (in sprites)

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Petrifying Ray");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 24;
            projectile.height = 24;
            projectile.scale = 1.0f;

            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;

            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;

            projectile.timeLeft = 50000;
        }

        public override void AI()
        {
            // Initialize
            if (aiPhase == 0)
            {
                npc = Main.npc[(int)projectile.ai[0]];
                target = Main.player[(int)projectile.ai[1]];
                freezePos = target.position;
                for (int i = 0; i < rayPartVariation; i++)
                {
                    rayPartRotation.Add((float)Main.rand.Next(360));
                    rayPartRotationIncrement.Add(Main.rand.Next(2) == 0 ? 1f : -1f);
                    rayPartMovement.Add(new Vector2(0f, 0f));
                    rayPartMovementIncrement.Add(new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)));
                }    
            }

            // The projectilestays on target
            projectile.position = target.position;

            // Freeze the target movement
            if (target != null)
            {
                //target.position = freezePos;
            }

            aiPhase++; // Add AI tick

            // RAY ANIMATION

            // Check if it is time to get new randomized values
            rotationChangeCounter++;
            if (rotationChangeCounter >= 5)
            {
                for (int i = 0; i < rayPartRotationIncrement.Count; i++)
                    rayPartRotationIncrement[i] = Main.rand.Next(2) == 0 ? rayPartRotationIncrement[i] : -rayPartRotationIncrement[i];
                if (!moveBack)
                {
                    for (int i = 0; i < rayPartRotationIncrement.Count; i++)
                        rayPartMovementIncrement[i] = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
                }
                
                rotationChangeCounter = 0;
                moveBack = !moveBack; // Moveback prevents parts from going too much astray
            }
            // Move pulses forward with set frequence
            if (aiPhase % pulseTickSpeed == 0)
            {
                for (int i = 0; i < pulsePositions.Count; i++)
                {
                    pulsePositions[i]++;
                }
            }
            // Create [pulseWidth] new pulses
            if (aiPhase % pulseFrequency == 0)
            {
                for (int i = 0; i < pulseWidth; i++)
                {
                    // NOTE: if the pulse width is very big, spawning new wave might seem clunky visually as all of the glowing parts emerge immediately
                    pulsePositions.Add(i);
                }
                Main.PlaySound(SoundID.Item116, target.Center);
            }
            // Rotate and move ray parts randomly
            for (int i = 0; i < rayPartRotation.Count; i++)
            {
                // Rotate parts
                rayPartRotation[i] += rayPartRotationIncrement[i];

                // Move parts
                // Moveback changes the movement to be opposite of the most recent one
                rayPartMovement[i] += moveBack ? rayPartMovementIncrement[i] : -rayPartMovementIncrement[i];
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (npc == null) return false; // If no target, do not draw ray

            // Counting for how many sprites are needed
            int i = 0;
            float accumulated = 0f;
            bool reached = false;

            Vector2 dir = target.Center - npc.Center;
            float distance = dir.Length();
            dir.Normalize();
            
            Texture2D texture = ModContent.GetTexture("DesertMod/Projectiles/Boss/DesertBossProjectileFreezeRay");

            // Draw the sprites forming the tether
            while (!reached)
            {
                // Accumulate
                i++;
                accumulated += projectile.height;
                int indexMod = i % rayPartVariation;

                // Sprite parameters
                Vector2 pos = target.Center - dir * projectile.height * i;
                Color color = lightColor;
                float rotation = rayPartRotation[indexMod] / 180f * (float)Math.PI;
                Vector2 origin = new Vector2(projectile.height * 0.5f, projectile.width * 0.5f);

                // If the current part is glowing one, add offset to sprite rectangle
                int recOffset = 0;
                if (pulsePositions.Contains(i))
                {
                    recOffset = 4 * projectile.height;
                    Dust.NewDust(pos, projectile.width, projectile.height, 217, dir.X * 0.5f, dir.Y * 0.5f, 150, default(Color), 1f);
                    Lighting.AddLight(pos, 0.3f, 1f, 1f);
                    color = Color.White; // Glowing parts are not affected by lighting
                }
                Rectangle? rec = new Rectangle(0, i % 4 * projectile.height + recOffset, projectile.width, projectile.height);

                // Draw sprite
                ((SpriteBatch)Main.spriteBatch).Draw(texture, pos - Main.screenPosition + rayPartMovement[indexMod], rec, color, rotation, origin, 1f, SpriteEffects.None, 0.0f);

                // If the tether is long enough stop drawing
                if (accumulated + projectile.height > distance) reached = true;
            }
            // Remove uneccessary pulses
            for (int k = 0; k < pulsePositions.Count; k++)
            {
                if (pulsePositions[k] > i) pulsePositions.RemoveAt(k);
            }

            return true;
        }
    }
}
