using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DesertMod.Projectiles.Bosses
{
    class DesertBossProjectileSpiritDagger : ModProjectile
    {
        Player target;
        Vector2 pos;
        int tailLength = 4;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spirit Dagger");
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

            projectile.timeLeft = 500;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Dust.NewDust(projectile.position, projectile.width, projectile.height, 217,
                projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 150, default(Color), 1f);
            Lighting.AddLight(projectile.Center, 0.3f, 1f, 1f);
            if (target != null)
            {
                target.position = pos;
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            this.target = target;
            pos = target.position;
            base.OnHitPlayer(target, damage, crit);
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            // Draw the ghosting trail
            for (int i = 0; i < tailLength; i++)
            {
                Texture2D texture = ModContent.GetTexture("DesertMod/Projectiles/Bosses/DesertBossProjectileSpiritDagger");
                Vector2 dir = projectile.velocity;
                dir.Normalize();
                Vector2 pos = projectile.Center - dir * 15f * (i + 1);
                Rectangle? rec = new Rectangle?();
                float fadeout = 1f / ((float)i + 2f);
                Color color = new Color(fadeout, fadeout, fadeout, fadeout);
                float rotation = projectile.rotation;
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                ((SpriteBatch)Main.spriteBatch).Draw(texture, pos - Main.screenPosition, rec, color, rotation, origin, 1f, SpriteEffects.None, 0.0f);
            }
            return true;
        }
    }
}
