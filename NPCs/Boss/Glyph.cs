using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.NPCs.Boss
{
    class Glyph : ModNPC
    {
        // AI tick counter
        public int aiPhase = 0;

        public NPC boss;
        public Vector2 attachPos;
        public bool isActive = true;
        public bool attached = true;

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

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }

        public override void AI()
        {
            if (aiPhase == 0)
            {
                boss = Main.npc[(int)npc.ai[0]];
                attachPos = new Vector2(npc.ai[2], npc.ai[3]);
            }

            if ((int)npc.ai[1] == 0)
            {
                isActive = false;
                if (!attached)
                {

                }
                else
                {
                    npc.immortal = true;
                    npc.Center = boss.Center + attachPos;
                }
            }
            else
            {
                isActive = true;
                npc.immortal = false;
                attached = false;
            }

            aiPhase++;
        }
    }
}
