﻿using Terraria;
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

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Petrifying Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 42;
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
            if (aiPhase == 0)
            {
                npc = Main.npc[(int)projectile.ai[0]];
                target = Main.player[(int)projectile.ai[1]];
                freezePos = target.position;
            }

            projectile.position = target.position;

            if (target != null)
            {
                target.position = freezePos;
            }

            aiPhase++;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            if (npc == null) return false;

            bool reached = false;
            Vector2 dir = projectile.Center - npc.Center;
            float distance = dir.Length();
            dir.Normalize();
            int i = 0;
            float acculumated = 0f;
            Texture2D texture = ModContent.GetTexture("DesertMod/Projectiles/Bosses/DesertBossProjectileSpiritDagger");
            float offset = 90f / 180f * (float)Math.PI;

            // Draw the tether
            while (!reached)
            {
                i++;
                acculumated += texture.Height;
                Vector2 pos = projectile.Center - dir * texture.Height * i;
                Rectangle? rec = new Rectangle?();
                Color color = Color.White;
                float rotation = -(float)(Math.Atan2(-dir.Y, dir.X) - offset);
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                ((SpriteBatch)Main.spriteBatch).Draw(texture, pos - Main.screenPosition, rec, color, rotation, origin, 1f, SpriteEffects.None, 0.0f);
                if (acculumated + texture.Height > distance) reached = true;
            }
            return true;
        }
    }
}
