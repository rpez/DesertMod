using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using DesertMod;
using System;

namespace DesertMod.NPCs.Bosses
{
    [AutoloadBossHead]
    public class DesertBoss : ModNPC
    {
        private enum BossPhase { HEALTHY, DAMAGED, RAGED }

        private Vector2 chargeDirection;

        // AI
        private int aiPhase = 0;
        private BossPhase currentPhase = BossPhase.HEALTHY;

        // Behavior flags
        private bool follow = true;
        private bool hover = true;
        private bool charge = false;
        private bool shootPattern1 = false;
        private bool shootPattern2 = false;
        private bool shootPattern3 = false;

        // Movement
        private float hoverDistanceFromPlayer = 300f;
        private float fastSpeedDistance = 300f;
        private float normalSpeed = 7f;
        private float fastSpeed = 13f;
        private float chargeSpeed = 30f;

        // Attacks
        private int daggerDamage = 1;
        private float daggerSpeed = 25f;
        private int halberdDamage = 10;

        // Animation
        private int frame = 0;
        private double counting;


        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ankh Amet, The Cursed Sphinx");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.width = 202;
            npc.height = 254;

            npc.boss = true;
            npc.aiStyle = -1;

            npc.lifeMax = 1000;
            npc.damage = 10;
            npc.defense = 10;
            npc.knockBackResist = 0f;

            npc.noGravity = true;
            npc.noTileCollide = true;

            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;

            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/DesertBossMusic");

            //bossBag = mod.ItemType("DesertBossTreasureBag");
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.ScaleExpertStats(numPlayers, bossLifeScale);
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.3f);
        }

        public override void AI()
        {
            if (aiPhase == 0) DesertMod.instance.ShowDebugUI();

            if (npc.life >= npc.lifeMax / 2) currentPhase = BossPhase.HEALTHY;
            else if (npc.life <= npc.lifeMax / 2 && npc.life >= npc.lifeMax / 3) currentPhase = BossPhase.DAMAGED;
            else currentPhase = BossPhase.RAGED;

            // Add "tick" to the phase counter of AI
            aiPhase++;

            //WorldGen.PlaceTile((int)npc.position.X, (int)npc.position.Y, TileID.Sand, true, true);

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

            // NPC initial rotation
            npc.rotation = 0.0f;
            npc.netAlways = true;
            npc.TargetClosest(true);

            // Prevents overheal
            if (npc.life >= npc.lifeMax) npc.life = npc.lifeMax;

            // Handles despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y -= 0.1f;
                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                    return;
                }
            }

            // Boss phases
            Vector2 bossCenter = npc.Center;
            Vector2 towardsPlayer = target - bossCenter;
            towardsPlayer.Normalize();
            int distance = (int)Vector2.Distance(target, npc.Center);
            npc.netUpdate = true;

            /* ___________________________________________________________________________
             * 
             * HEALTHY
             * Boss HP bar above half
             * ___________________________________________________________________________
            */
            if (currentPhase == BossPhase.HEALTHY)
            {
                follow = true;
                // Dagger attack
                if (aiPhase % 50 == 0)
                {
                    shootPattern1 = true;
                }
                if (aiPhase % 300 == 0)
                {
                    hover = !hover;
                    shootPattern2 = true;
                }
            }

            /* ___________________________________________________________________________
             * 
             * DAMAGED
             * Boss HP bar below half but above third
             * ___________________________________________________________________________
            */
            else if (currentPhase == BossPhase.DAMAGED)
            {
                follow = true;
                if (aiPhase == 150)
                {
                    charge = true;
                    chargeDirection = towardsPlayer;
                    int pro = Projectile.NewProjectile(bossCenter, Vector2.Zero, mod.ProjectileType("DesertBossProjectileHalberd"), halberdDamage, 0f);
                    Main.projectile[pro].ai[0] = npc.whoAmI;
                    Main.projectile[pro].ai[1] = 0f;
                }
                if (aiPhase == 170) charge = false;
                if (aiPhase == 250)
                {
                    chargeDirection = towardsPlayer;
                    int pro = Projectile.NewProjectile(bossCenter, Vector2.Zero, mod.ProjectileType("DesertBossProjectileHalberd"), halberdDamage, 0f);
                    Main.projectile[pro].ai[0] = npc.whoAmI;
                    Main.projectile[pro].ai[1] = 1f;
                }
                if (aiPhase == 270) charge = false;
                if (aiPhase > 300)
                {
                    aiPhase = 0;
                    charge = false;
                }
            }

            /* ___________________________________________________________________________
             * 
             * RAGED
             * Boss HP bar below third
             * ___________________________________________________________________________
            */
            else if (currentPhase == BossPhase.RAGED)
            {

            }

            // Execute behaviour accordign to flags
            if (follow)
            {
                if (!hover) MoveTowards(npc, target - towardsPlayer, (float)(distance > fastSpeedDistance ? fastSpeed : normalSpeed), 30f);
            }
            if (hover)
            {
                MoveTowards(npc, target - towardsPlayer * hoverDistanceFromPlayer, (float)(distance > fastSpeedDistance ? fastSpeed : normalSpeed), 30f);
            }
            if (charge)
            {
                npc.velocity = chargeDirection * chargeSpeed;
            }
            if (shootPattern1)
            {
                Projectile.NewProjectile(bossCenter, towardsPlayer * daggerSpeed, mod.ProjectileType("DesertBossProjectileSpiritDagger"), daggerDamage, 0f);

                // Workaround ghosting trail TODO: implement better method for this
                for (int i = 1; i < 5; i++)
                {
                    Vector2 dir = towardsPlayer;
                    dir.Normalize();
                    Vector2 pos = bossCenter - dir * 15f * i;
                    Projectile.NewProjectile(pos, towardsPlayer * daggerSpeed, mod.ProjectileType("DesertBossProjectileSpiritDagger"), 0, 0f);
                }

                shootPattern1 = false;
            }
            if (shootPattern2)
            {
                float angle = -10f / 180 * (float)Math.PI;
                float increment = 5f / 180 * (float)Math.PI;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 dir = new Vector2(towardsPlayer.X * (float)Math.Cos(angle + i * increment) - towardsPlayer.Y * (float)Math.Sin(angle + i * increment),
                        towardsPlayer.X * (float)Math.Sin(angle + i * increment) + towardsPlayer.Y * (float)Math.Cos(angle + i * increment));
                    Projectile.NewProjectile(bossCenter, dir * daggerSpeed, mod.ProjectileType("DesertBossProjectileSpiritDagger"), daggerDamage, 0f);

                    // Workaround ghosting trail TODO: implement better method for this
                    for (int k = 1; k < 5; k++)
                    {
                        Vector2 pos = bossCenter - dir * 15f * k;
                        Projectile.NewProjectile(pos, dir * daggerSpeed, mod.ProjectileType("DesertBossProjectileSpiritDagger"), 0, 0f);
                    }
                }
                shootPattern2 = false;
            }
            if (shootPattern3)
            {

                shootPattern3 = false;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (frame == 0)
            {
                counting += 1.0;
                if (counting < 8.0)
                {
                    npc.frame.Y = 0;
                }
                else if (counting < 16.0)
                {
                    npc.frame.Y = frameHeight;
                }
                else if (counting < 24.0)
                {
                    npc.frame.Y = frameHeight * 2;
                }
                else if (counting < 32.0)
                {
                    npc.frame.Y = frameHeight * 3;
                }
                else
                {
                    counting = 0.0;
                }
            }
        }

        public override void NPCLoot()
        {
            base.NPCLoot();
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            base.BossLoot(ref name, ref potionType);
        }

        private void MoveTowards(NPC npc, Vector2 playerTarget, float speed, float turnResistance)
        {
            Vector2 move = playerTarget - npc.Center;
            float length = move.Length();
            if (length > speed)
            {
                move *= speed / length;
            }
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            length = move.Length();
            if (length > speed)
            {
                move *= speed / length;
            }
            npc.velocity = move;
        }
    }
}