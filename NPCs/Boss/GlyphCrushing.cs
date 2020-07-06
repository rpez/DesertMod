using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace DesertMod.NPCs.Boss
{
    class GlyphCrushing : Glyph
    {
        private int leftWall;
        private int rightWall;
        private Vector2 target;

        private bool wallsActive = false;
        private float wallSpeed = 1f;
        private float wallDistance = 2000f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crushing Glyph");
            Main.npcFrameCount[npc.type] = 1;
        }

        public override void SetDefaults()
        {
            npc.width = 36;
            npc.height = 44;

            npc.aiStyle = -1;

            npc.lifeMax = 1500;
            npc.damage = 10;
            npc.defense = 10;
            npc.knockBackResist = 0f;

            npc.noGravity = true;
            npc.noTileCollide = true;

            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.ScaleExpertStats(numPlayers, bossLifeScale);
        }

        public override void AI()
        {
            // Run base AI and if not active do not execute glyph specific AI
            base.AI();
            if (!isActive) return;

            if (aiPhase == 0) target = new Vector2(npc.ai[4], npc.ai[5]);

            // Summon walls if they are not active
            if (!wallsActive)
            {
                leftWall = Projectile.NewProjectile(npc.Center + new Vector2(-wallDistance, 0f), new Vector2(wallSpeed, 0f), mod.ProjectileType("DesertBossProjectileCrushingWall"), 50, 10f);
                Main.projectile[leftWall].ai[0] = target.X;
                Main.projectile[leftWall].ai[1] = target.Y;
                rightWall = Projectile.NewProjectile(npc.Center + new Vector2(wallDistance, 0f), new Vector2(-wallSpeed, 0f), mod.ProjectileType("DesertBossProjectileCrushingWall"), 50, 10f);
                Main.projectile[rightWall].ai[0] = target.X;
                Main.projectile[rightWall].ai[1] = target.Y;

                wallsActive = true;
            }

            // Kill walls when glyph is killed
            if (wallsActive && npc.life <= 0)
            {
                Main.projectile[leftWall].timeLeft = 0;
                Main.projectile[rightWall].timeLeft = 0;
            }

            aiPhase++;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }
    }
}
