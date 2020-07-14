using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DesertMod.Projectiles.Boss
{
    class DesertBossProjectileSpiritDagger : ModProjectile
    {
        private int aiPhase = 0;

        private int tailLength = 4; // How many ghosting sprites does the trail have
        private float tailSpread = 15f; // How much space between trail parts

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Dagger");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 18;
            projectile.scale = 1.0f;

            projectile.aiStyle = -1;

            projectile.friendly = false;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;

            projectile.timeLeft = 500;
        }

        public override void AI()
        {
            if (aiPhase == 0)
            {
                projectile.rotation = -(float)(Math.Atan2(-projectile.velocity.Y, projectile.velocity.X) - 90f / 180f * Math.PI);
            }
            
            // Glow and dust trail
            Dust.NewDust(projectile.position, projectile.width, projectile.height, 217,
                projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default, 1f);
            Lighting.AddLight(projectile.Center, 0.3f, 1f, 1f);

            aiPhase++;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.GetTexture("DesertMod/Projectiles/Boss/DesertBossProjectileSpiritDagger");
            Vector2 dir = projectile.velocity;
            dir.Normalize();

            // Draw the ghosting trail
            for (int i = 0; i < tailLength; i++)
            {
                // Parameters
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                // TODO: Fix autistic hard coded piece of garbage held together by duct tape and bubblegum that miraculously works but will probably fail
                // if anything is changed
                Vector2 pos = projectile.Center - dir * (tailSpread * (i + 1) + (projectile.height * 1.5f)) + new Vector2(projectile.width * 0.5f - 4f, 0f);
                Rectangle? rec = new Rectangle?();
                float fadeout = 1f / ((float)i + 2f);
                Color color = new Color(fadeout, fadeout, fadeout, fadeout);
                float rotation = projectile.rotation;

                // Draw sprite
                ((SpriteBatch)Main.spriteBatch).Draw(texture, pos - Main.screenPosition, rec, color, rotation, origin, 1f, SpriteEffects.None, 0.0f);
            }

            return true;
        }
    }
}
