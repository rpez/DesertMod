﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
            // Run base AI and if not active do not execute glyph specific AI
            base.AI();
            if (!isActive)
            {
                DeactivateWalls();
                return;
            }

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

            aiPhase++;
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
