using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using Terraria.DataStructures;

namespace DesertMod.NPCs.Boss
{
    class GlyphBurning : Glyph
    {
        // AI tick counter
        private int aiPhase = 0;

        // List of all players
        private Player[] players;

        // Burn variables
        private bool burnOn = false;
        private float burnDistance = 2000f;
        private int burnDamage = 10;
        private int burnInterval = 200;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Burning Glyph");
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
            if (aiPhase == 0)
            {
                // Find all players
                players = new Player[Main.ActivePlayersCount];
                int k = 0;
                for (int i = 0; i < Main.player.Length; i++)
                {
                    Player player = Main.player[i];
                    if (player != null && player.active)
                    {
                        players[k] = player;
                        k++;
                        if (k >= players.Length) i = Main.player.Length;
                    }
                }
            }

            // Toggle burn periodically
            if (aiPhase % burnInterval == 0) burnOn = !burnOn;
            
            // If burning
            if (burnOn)
            {
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
                            if (tile != null && !Main.tileSolid[tile.type])
                            {
                                hitWall = true; // Wall hit
                            }
                            if ((npc.Center - curPos).Length() >= (npc.Center - player.Center).Length())
                            {
                                reached = true; // Player hit
                            }
                            curPos += dir;
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
            
            aiPhase++;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }
    }
}
