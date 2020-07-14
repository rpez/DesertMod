using Terraria;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.NPCs.Boss
{
    class GlyphPetrifying : Glyph
    {
        // Updated in code
        private int ray = -1;
        private bool rayActive = false;
        public bool hover = true;
        public Vector2 lastHoverVelocity;
        public Vector2 targetHoverPos;

        // Adjustable variabless
        public float targetingSpeed = 3f;
        public float targetingTurnResistance = 5f;

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
                hoverOffset = new Vector2(0, -500f);
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
            targetHoverPos = Main.player[npc.target].Center + hoverOffset;

            MoveTowards(npc, targetHoverPos, targetingSpeed, targetingTurnResistance);

            if (hover)
            {
                float vel = (float)Math.Cos(aiPhase / 180f * Math.PI);
                lastHoverVelocity = new Vector2(Main.rand.NextFloat(-0.3f, 0.3f), vel);
                npc.velocity += lastHoverVelocity;
            }

            // Shoot ray if it is not active
            if (!rayActive)
            {
                Player player = Main.player[npc.target];
                ray = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("DesertBossProjectileFreezeRay"), 0, 0f);
                Main.projectile[ray].ai[0] = npc.whoAmI;
                Main.projectile[ray].ai[1] = player.whoAmI;
                    
                rayActive = true;
            }
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
