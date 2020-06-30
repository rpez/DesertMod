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
        private int aiPhase = 0;

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
    }
}
