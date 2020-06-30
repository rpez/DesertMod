using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace DesertMod.Projectiles.Boss
{
    class DesertBossProjectileCrushingWall : ModProjectile
    {
        private int aiPhase = 0;

        private Vector2 startPos;
        private Vector2 target;
        private float distance;
        private int[] spriteScale = new int[2] { 24, 24 };

        // WALL ANIMATION VARIABLES

        private int wallPartVariation = 10; // How many randomized values in the lists
        private float wallHeight = 30;

        // Lists for randomized values
        private List<float> wallPartRotation = new List<float>();
        private List<float> wallPartRotationIncrement = new List<float>();
        private List<Vector2> wallPartMovement = new List<Vector2>();
        private List<Vector2> wallPartMovementIncrement = new List<Vector2>();

        private int rotationChangeCounter = 0; // Counter for randomization change
        private bool moveBack = false; // Prevents parts from going astray
        private float partScale = 5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crushing Wall of Stones");
            Main.projFrames[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 120;
            projectile.height = 3600;
            projectile.scale = 1.0f;

            projectile.aiStyle = -1;

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
                startPos = projectile.Center;
                target = new Vector2(projectile.ai[0], projectile.ai[1]);
                distance = (target - projectile.Center).Length();
                for (int i = 0; i < wallPartVariation; i++)
                {
                    wallPartRotation.Add((float)Main.rand.Next(360));
                    wallPartRotationIncrement.Add(Main.rand.Next(2) == 0 ? 1f : -1f);
                    wallPartMovement.Add(new Vector2(0f, 0f));
                    wallPartMovementIncrement.Add(new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)));
                }
            }

            if ((startPos - projectile.Center).Length() >= distance)
            {
                projectile.timeLeft = 0;
            }

            aiPhase++; // Add AI tick

            // WALL ANIMATION
            // Check if it is time to get new randomized values
            rotationChangeCounter++;
            if (rotationChangeCounter >= 5)
            {
                for (int i = 0; i < wallPartRotationIncrement.Count; i++)
                    wallPartRotationIncrement[i] = Main.rand.Next(2) == 0 ? wallPartRotationIncrement[i] : -wallPartRotationIncrement[i];
                if (!moveBack)
                {
                    for (int i = 0; i < wallPartRotationIncrement.Count; i++)
                        wallPartMovementIncrement[i] = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
                }

                rotationChangeCounter = 0;
                moveBack = !moveBack; // Moveback prevents parts from going too much astray
            }
            // Rotate and move wall parts randomly
            for (int i = 0; i < wallPartRotation.Count; i++)
            {
                // Rotate parts
                wallPartRotation[i] += wallPartRotationIncrement[i];

                // Move parts
                // Moveback changes the movement to be opposite of the most recent one
                wallPartMovement[i] += moveBack ? wallPartMovementIncrement[i] : -wallPartMovementIncrement[i];
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Vector2 dir = new Vector2(0f, 1f);
            Vector2 scaledSize = new Vector2(spriteScale[0], spriteScale[1]) * partScale;

            // NOTE: workaround solution with sprite "hack"
            Texture2D texture = ModContent.GetTexture("DesertMod/Projectiles/Boss/DesertBossProjectileFreezeRay");
            Vector2 offset = -dir * scaledSize.Y * (float)wallHeight / 2f;

            // Draw the sprites forming the tether
            for (int i = 0; i < wallHeight; i++)
            {
                // Accumulate
                int indexMod = i % wallPartVariation;

                // Sprite parameters
                Vector2 pos = projectile.Center + dir * scaledSize.Y * i + offset;
                Color color = lightColor;
                float rotation = wallPartRotation[indexMod] / 180f * (float)Math.PI;
                Vector2 origin = new Vector2(spriteScale[0] * 0.5f, spriteScale[1] * 0.5f);

                Rectangle? rec = new Rectangle(0, i % 4 * spriteScale[0], spriteScale[0], spriteScale[1]);

                // Draw sprite
                ((SpriteBatch)Main.spriteBatch).Draw(texture, pos - Main.screenPosition + wallPartMovement[indexMod],
                    rec, color, rotation, origin, partScale, SpriteEffects.None, 0.0f);
            }

            return true;
        }
    }
}
