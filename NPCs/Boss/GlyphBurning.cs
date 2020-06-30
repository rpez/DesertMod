using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.NPCs.Boss
{
    class GlyphBurning : Glyph
    {
        // AI tick counter
        private int aiPhase = 0;

        private NPC boss;

        private float rotationAroundBoss = 0;
        private float distanceFromCenter = 300;
        private float rotationSpeed = 1;

        private int ray;
        private bool shootRay = true;
        private bool rayActive = false;

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
            if (aiPhase == 0) boss = Main.npc[(int)npc.ai[0]];

            npc.TargetClosest(true);

            double rad = rotationAroundBoss * (Math.PI / 180);

            npc.position.X = boss.Center.X - (int)(Math.Cos(rad) * distanceFromCenter) - npc.width / 2;
            npc.position.Y = boss.Center.Y - (int)(Math.Sin(rad) * distanceFromCenter) - npc.height / 2;

            // Shoot ray if it is not active
            if (!rayActive)
            {
                Player player = Main.player[npc.target];
                if (shootRay)
                {
                    ray = Projectile.NewProjectile(npc.Center, Vector2.Zero, mod.ProjectileType("DesertBossProjectileFreezeRay"), 0, 0f);
                    Main.projectile[ray].ai[0] = npc.whoAmI;
                    Main.projectile[ray].ai[1] = player.whoAmI;
                    
                    shootRay = false;
                    rayActive = true;
                }
            }

            // Kill ray when glyph is killed
            if (rayActive && npc.life <= 0)
            {
                Main.projectile[ray].timeLeft = 0;
            }

            rotationAroundBoss += rotationSpeed;
            aiPhase++;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }
    }
}
