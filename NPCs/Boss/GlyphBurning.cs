using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using System;

namespace DesertMod.NPCs.Boss
{
    class GlyphBurning : Glyph
    {
        // Updated in code
        private bool burnOn = false;
        private bool hover = false;
        private bool move = true;
        public Vector2 lastHoverVelocity;

        // Adjustable variables
        private float burnDistance = 2000f;
        private int burnDamage = 10;
        private int burnInterval = 500;
        public float targetingSpeed = 2f;
        public float targetingTurnResistance = 2f;

        // Shader timers
        private int filterTimer = 0;
        private int filterBuildupTime = 200;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Glyph");
            Main.npcFrameCount[npc.type] = 1;
        }

        public override void AI()
        {
            if (initialize)
            {
                hoverOffset = new Vector2(0, -200f);
                initialize = false;
            }

            // Run base AI and if not active do not execute glyph specific AI
            base.AI();
            if (!isActive)
            {
                DeactivateShaders();
                return;
            }

            // Toggle burn periodically
            if (aiPhase % burnInterval == 0)
            {
                burnOn = !burnOn;
                move = !burnOn;
                hover = burnOn;
                if (Main.netMode != NetmodeID.Server)
                {
                    // Toggle shaders
                    if (burnOn)
                    {
                        npc.velocity = Vector2.Zero;
                        ActivateShaders();
                    }
                    else
                    {
                        DeactivateShaders();
                    }
                }
            }

            // Update shaders
            if (Main.netMode != NetmodeID.Server && Filters.Scene["GlyphBurningGlow"].IsActive() && Filters.Scene["GlyphBurningDistort"].IsActive())
            {
                float progress = (float)filterTimer;
                float buildup = (float)filterTimer / (float)filterBuildupTime;
                if (buildup > 1f) buildup = 1f;

                // Intensity is the buildup of the overexposure glow, progress is just time
                Filters.Scene["GlyphBurningGlow"].GetShader().UseIntensity(buildup).UseProgress(progress);
                Filters.Scene["GlyphBurningDistort"].GetShader().UseIntensity(buildup).UseProgress(progress);
            }

            // If burning
            if (burnOn)
            {
                if (aiPhase % 20 == 0)
                {
                    Main.PlaySound(SoundID.Item82, npc.Center);
                }
                // Raycast each player and deal dmg if hit
                foreach (Player player in players)
                {
                    Vector2 dir = player.Center - npc.Center;
                    if (dir.Length() < burnDistance)
                    {
                        dir.Normalize();
                        bool reached = false;
                        bool hitWall = false;
                        Vector2 curPos = npc.Center + dir * 25f;

                        // Basically check every tile between npc and player(s)
                        while (!reached && !hitWall)
                        {
                            Tile tile = Main.tile[(int)curPos.X / 16, (int)curPos.Y / 16];
                            if (tile != null && Main.tileSolid[tile.type] && tile.collisionType == 1)
                            {
                                hitWall = true; // Wall hit
                            }
                            if ((npc.Center - curPos).Length() >= (npc.Center - player.Center).Length())
                            {
                                reached = true; // Player hit
                            }
                            curPos += dir * 0.5f;
                        }

                        // Deal damage if player is hit
                        if (reached)
                        {
                            int direction = npc.Center.X > player.Center.X ? -1 : 1;
                            player.Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), burnDamage, direction);
                        }
                    }
                }
            }

            if (move)
            {
                MoveTowards(npc, boss.Center + hoverOffset, targetingSpeed, targetingTurnResistance);
            }

            if (hover)
            {
                npc.velocity -= lastHoverVelocity;
                float vel = (float)Math.Cos(aiPhase / 180f * Math.PI);
                lastHoverVelocity = new Vector2(Main.rand.NextFloat(-0.3f, 0.3f), vel * 0.5f);
                npc.velocity += lastHoverVelocity;
            }

            filterTimer++;
        }

        // Activate shaders if they are not active and reset timer
        private void ActivateShaders()
        {
            if (!Filters.Scene["GlyphBurningGlow"].IsActive() && !Filters.Scene["GlyphBurningDistort"].IsActive())
            {
                Filters.Scene.Activate("GlyphBurningGlow", npc.Center).GetShader().UseTargetPosition(npc.Center);
                Filters.Scene.Activate("GlyphBurningDistort", npc.Center).GetShader().UseTargetPosition(npc.Center);
                filterTimer = 0;
            }
        }

        // Activate shaders if they are active
        private void DeactivateShaders()
        {
            if (Filters.Scene["GlyphBurningGlow"].IsActive() && Filters.Scene["GlyphBurningDistort"].IsActive())
            {
                Filters.Scene["GlyphBurningGlow"].Deactivate();
                Filters.Scene["GlyphBurningDistort"].Deactivate();
            }
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }

        // Pass death flag to boss and disable shaders
        public override bool CheckDead()
        {
            if (npc.life <= 0)
            {
                if (burnOn)
                {
                    DeactivateShaders();
                    burnOn = false;
                }
                boss.ai[2] = 1;
            }
            return base.CheckDead();
        }
    }
}
