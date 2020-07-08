using Terraria;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.NPCs.Boss
{
    class GlyphPetrifying : Glyph
    {
        // Updated in code
        private float rotationAroundBoss = 0;
        private int ray = -1;
        private bool shootRay = true;
        private bool rayActive = false;

        // Adjustable variables
        private float distanceFromCenter = 300;
        private float rotationSpeed = 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Petrifying Glyph");
            Main.npcFrameCount[npc.type] = 1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.ScaleExpertStats(numPlayers, bossLifeScale);
        }

        public override void AI()
        {
            if (initialize)
            {
                hoverOffset = new Vector2(0, -300f);
                initialize = false;
            }

            // Run base AI and if not active do not execute glyph specific AI
            base.AI();
            if (!isActive)
            {
                DeactivateRay();
                return;
            }

            npc.TargetClosest(true);

            //double rad = rotationAroundBoss * (Math.PI / 180);

            //npc.position.X = boss.Center.X - (int)(Math.Cos(rad) * distanceFromCenter) - npc.width / 2;
            //npc.position.Y = boss.Center.Y - (int)(Math.Sin(rad) * distanceFromCenter) - npc.height / 2;

            // Shoot ray if it is not active
            if (!rayActive)
            {
                Player player = Main.player[npc.target];
                if (shootRay)
                {
                    ray = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("DesertBossProjectileFreezeRay"), 0, 0f);
                    Main.projectile[ray].ai[0] = npc.whoAmI;
                    Main.projectile[ray].ai[1] = player.whoAmI;
                    
                    shootRay = false;
                    rayActive = true;
                }
            }

            rotationAroundBoss += rotationSpeed;
        }

        // Pass death flag to boss and kill ray
        public override bool CheckDead()
        {
            if (npc.life <= 0)
            {
                DeactivateRay();
                boss.ai[0] = 1;
            }
            return base.CheckDead();
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }

        // Kill ray projectile
        private void DeactivateRay()
        {
            if (rayActive && ray >= 0)
            {
                Main.projectile[ray].timeLeft = 0;
                ray = -1;
                rayActive = false;
            }
        }
    }
}
