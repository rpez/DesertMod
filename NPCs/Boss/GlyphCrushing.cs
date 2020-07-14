using Terraria;
using Microsoft.Xna.Framework;

namespace DesertMod.NPCs.Boss
{
    class GlyphCrushing : Glyph
    {
        // Updated in code
        private int leftWall = -1;
        private int rightWall = -1;
        private Vector2 target;
        private bool wallsActive = false;

        // Adjustable variables
        private float wallSpeed = 1f;
        private float wallDistance = 2000f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crushing Glyph");
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
                hoverOffset = new Vector2(0, 0);
                initialize = false;
            }

            // Run base AI and if not active do not execute glyph specific AI
            base.AI();
            if (!isActive)
            {
                npc.noTileCollide = true;
                npc.noGravity = true;
                DeactivateWalls();
                return;
            }

            npc.noTileCollide = false;
            npc.noGravity = false;

            // Summon walls if they are not active
            if (!wallsActive)
            {
                leftWall = Projectile.NewProjectile(npc.Center + new Vector2(-wallDistance, 0f), new Vector2(wallSpeed, 0f), mod.ProjectileType("DesertBossProjectileCrushingWall"), 50, 10f);
                Main.projectile[leftWall].ai[0] = npc.Center.X;
                Main.projectile[leftWall].ai[1] = npc.Center.Y;
                rightWall = Projectile.NewProjectile(npc.Center + new Vector2(wallDistance, 0f), new Vector2(-wallSpeed, 0f), mod.ProjectileType("DesertBossProjectileCrushingWall"), 50, 10f);
                Main.projectile[rightWall].ai[0] = npc.Center.X;
                Main.projectile[rightWall].ai[1] = npc.Center.Y;

                wallsActive = true;
            }
        }

        // Pass death flag to boss and kill walls
        public override bool CheckDead()
        {
            if (npc.life <= 0)
            {
                DeactivateWalls();
                boss.ai[1] = 1;
            }
            return base.CheckDead();
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }

        // Kill wall projectiles
        private void DeactivateWalls()
        {
            if (leftWall >= 0)
            {
                Main.projectile[leftWall].timeLeft = 0;
                leftWall = -1;
            }
            if (rightWall >= 0)
            {
                Main.projectile[rightWall].timeLeft = 0;
                rightWall = -1;
            }
            wallsActive = false;
        }
    }
}
