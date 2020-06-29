using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DesertMod.Projectiles.Bosses
{
    class DesertBossProjectileFreezeRay : ModProjectile
    {
        int aiPhase = 0;
        

        Player target;
        NPC npc;
        Vector2 freezePos;

        int rayPartVariation = 10;
        List<float> rayPartRotation = new List<float>();
        List<float> rayPartRotationIncrement = new List<float>();
        List<Vector2> rayPartMovement = new List<Vector2>();
        List<Vector2> rayPartMovementIncrement = new List<Vector2>();
        int rotationChangeCounter = 0;
        bool moveBack = false;
        List<int> pulsePositions = new List<int>();
        int pulseFrequency = 60;
        int pulseTickSpeed = 2;
        int pulseWidth = 3;

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

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
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

            aiPhase++;
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
                moveBack = !moveBack;
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
                    pulsePositions.Add(i);
                }
            }
            for (int i = 0; i < rayPartRotation.Count; i++)
            {
                rayPartRotation[i] += rayPartRotationIncrement[i];
                rayPartMovement[i] += moveBack ? rayPartMovementIncrement[i] : -rayPartMovementIncrement[i];
            }
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            if (npc == null) return false;

            int i = 0;
            float accumulated = 0f;
            bool reached = false;

            Vector2 dir = target.Center - npc.Center;
            float distance = dir.Length();
            dir.Normalize();
            
            Texture2D texture = ModContent.GetTexture("DesertMod/Projectiles/Bosses/DesertBossProjectileFreezeRay");

            // Draw the sprites forming the tether
            while (!reached)
            {
                // Accumulate
                i++;
                accumulated += projectile.height;
                int indexMod = i % rayPartVariation;

                // Sprite parameters
                Vector2 pos = target.Center - dir * projectile.height * i;
                int recOffset = 0;
                if (pulsePositions.Contains(i))
                {
                    recOffset = 4 * projectile.height;
                    Dust.NewDust(pos, projectile.width, projectile.height, 217, dir.X * 0.5f, dir.Y * 0.5f, 150, default(Color), 1f);
                    Lighting.AddLight(pos, 0.3f, 1f, 1f);
                }
                Rectangle? rec = new Rectangle(0, i % 4 * projectile.height + recOffset, projectile.width, projectile.height);
                Color color = Color.White;
                float rotation = rayPartRotation[indexMod] / 180f * (float)Math.PI;
                Vector2 origin = new Vector2(projectile.height * 0.5f, projectile.width * 0.5f);

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
