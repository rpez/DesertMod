using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Linq;

namespace DesertMod.NPCs.Boss
{
    class Glyph : ModNPC
    {
        // AI tick counter
        public int aiPhase = 0;

        // List of all players
        public Player[] players;
        public int[] playerIDs;

        // Values initialized in code
        public NPC boss;
        public Vector2 attachPos;
        public bool isActive = true;
        public bool attached = true;
        public bool hover = true;

        // Adjustable variables shared by all glyphs
        public float returnSpeed = 30f;
        public float turnResistance = 10f;

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
            npc.netAlways = true;

            if (aiPhase == 0)
            {
                boss = Main.npc[(int)npc.ai[0]];
                attachPos = new Vector2(npc.ai[2], npc.ai[3]);

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

                // Save player ids to separate array
                playerIDs = players.Select(player => player.whoAmI).ToArray();
            }

            // npc.ai[1] determines whether glyph should be active
            if ((int)npc.ai[1] == 0)
            {
                Vector2 targetPos = boss.Center + attachPos;
                isActive = false;
                if (!attached)
                {
                    Vector2 move = targetPos - npc.Center;
                    float length = move.Length();
                    if (length > returnSpeed)
                    {
                        move *= returnSpeed / length;
                    }
                    move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
                    length = move.Length();
                    if (length > returnSpeed)
                    {
                        move *= returnSpeed / length;
                    }
                    npc.velocity = move;

                    // TODO: fix workaround
                    if (Vector2.Distance(npc.Center, targetPos) <= 10f) attached = true;
                }
                else
                {
                    foreach (int id in playerIDs)
                    {
                        npc.immune[id] = 2;
                    }
                    npc.Center = targetPos;
                }
            }
            else
            {
                isActive = true;
                attached = false;
            }



            aiPhase++;
        }
    }
}
