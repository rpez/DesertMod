using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DesertMod.Projectiles.Bosses
{
    class DesertBossProjectileFreezeRay : ModProjectile
    {
        int aiPhase = 0;

        Player target;
        NPC npc;
        Vector2 freezePos;
        float rayPartRotationIncrement = 1f;

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
            }

            // The projectilestays on target
            projectile.position = target.position;

            // Freeze the target movement
            if (target != null)
            {
                //target.position = freezePos;
            }

            aiPhase++;
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
            float offset = 90f / 180f * (float)Math.PI;

            // Draw the sprites forming the tether
            while (!reached)
            {
                // Accumulate
                i++;
                accumulated += projectile.height;

                // Sprite parameters
                Vector2 pos = target.Center - dir * projectile.height * i;
                Rectangle? rec = new Rectangle(0, i % 4 * projectile.height, projectile.height, projectile.height);
                Color color = Color.White;
                float rotOffset = Main.rand.Next(2) == 0 ? rayPartRotationIncrement * (float)aiPhase % 60f : -rayPartRotationIncrement * (float)aiPhase % 60f;
                offset = offset + rotOffset;
                float rotation = -(float)(Math.Atan2(-dir.Y, dir.X) - offset); // Rotate towards target
                Vector2 origin = new Vector2(projectile.height * 0.5f, projectile.width * 0.5f);

                // Draw sprite
                ((SpriteBatch)Main.spriteBatch).Draw(texture, pos - Main.screenPosition, rec, color, rotation, origin, 1f, SpriteEffects.None, 0.0f);

                // If the tether is long enough stop drawing
                if (accumulated + projectile.height > distance) reached = true;
            }
            return true;
        }
    }
}
