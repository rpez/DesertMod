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

        // WALL ANIMATION VARIABLES

        private int rayPartVariation = 10; // How many randomized values in the lists
        private float wallHeight = 30;

        // Lists for randomized values
        private List<float> rayPartRotation = new List<float>();
        private List<float> rayPartRotationIncrement = new List<float>();
        private List<Vector2> rayPartMovement = new List<Vector2>();
        private List<Vector2> rayPartMovementIncrement = new List<Vector2>();

        private int rotationChangeCounter = 0; // Counter for randomization change
        private bool moveBack = false; // Prevents parts from going astray
        private float partScale = 5f;

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
                for (int i = 0; i < rayPartVariation; i++)
                {
                    rayPartRotation.Add((float)Main.rand.Next(360));
                    rayPartRotationIncrement.Add(Main.rand.Next(2) == 0 ? 1f : -1f);
                    rayPartMovement.Add(new Vector2(0f, 0f));
                    rayPartMovementIncrement.Add(new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)));
                }
            }

            if ((startPos - projectile.Center).Length() >= distance)
            {
                projectile.timeLeft = 0;
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
            Vector2 dir = new Vector2(0f, 1f);
            Vector2 scaledSize = projectile.Size * partScale;

            Texture2D texture = ModContent.GetTexture("DesertMod/Projectiles/Boss/DesertBossProjectileCrushingWall");
            Vector2 offset = -dir * scaledSize.Y * (float)wallHeight / 2f;

            // Draw the sprites forming the tether
            for (int i = 0; i < wallHeight; i++)
            {
                // Accumulate
                int indexMod = i % rayPartVariation;

                // Sprite parameters
                Vector2 pos = projectile.Center + dir * scaledSize.Y * i + offset;
                Color color = lightColor;
                float rotation = rayPartRotation[indexMod] / 180f * (float)Math.PI;
                Vector2 origin = new Vector2(projectile.height * 0.5f, projectile.width * 0.5f);

                Rectangle? rec = new Rectangle(0, i % 4 * projectile.height, projectile.width, projectile.height);

                // Draw sprite
                ((SpriteBatch)Main.spriteBatch).Draw(texture, pos - Main.screenPosition + rayPartMovement[indexMod],
                    rec, color, rotation, origin, partScale, SpriteEffects.None, 0.0f);
            }

            return true;
        }
    }
}
