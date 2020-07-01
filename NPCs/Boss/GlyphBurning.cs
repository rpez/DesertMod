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

        private Player[] players;

        private float projectileSpeed = 100f;
        private int projectileDamage = 1;
        private float projectileKnockback = 0f;

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
            
            foreach (Player player in players)
            {
                Vector2 dir = player.Center - npc.Center;
                if (dir.Length() < 1000f)
                {
                    dir.Normalize();
                    bool reached = false;
                    bool hitWall = false;
                    Vector2 curPos = npc.Center + dir * 25f;

                    while (!reached && !hitWall)
                    {
                        Tile tile = Main.tile[(int)curPos.X / 16, (int)curPos.Y / 16];
                        if (tile != null && !Main.tileSolid[tile.type])
                        {
                            hitWall = true;
                        }
                        if ((npc.Center - curPos).Length() >= (npc.Center - player.Center).Length())
                        {
                            reached = true;
                        }
                        curPos += dir;
                    }

                    if (reached)
                    {
                        int direction = npc.Center.X > player.Center.X ? -1 : 1;
                        player.Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), 10, direction);
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
