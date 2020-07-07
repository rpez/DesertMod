using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;

namespace DesertMod.NPCs.Boss
{
    [AutoloadBossHead]
    public class DesertBoss : ModNPC
    {
        private enum BossPhase { HEALTHY, DAMAGED, RAGED }

        private Vector2 chargeDirection;

        // AI
        private int aiPhase = 0;
        private BossPhase currentPhase = BossPhase.HEALTHY;
        private bool initialize = true;

        // Behavior flags
        private bool follow = true;
        private bool hover = false;
        private bool charge = false;
        private bool goHigh = false;
        private bool shootPattern1 = false;
        private bool shootPattern2 = false;
        private bool shootPattern3 = false;

        private int glyphPetrifying = -1;
        private bool glyphPetrifyingActive = false;
        private bool glyphPetrifyingDead = false;

        private int glyphCrushing = -1;
        private bool glyphCrushingActive = false;
        private bool glyphCrushingDead = false;

        private int glyphBurning = -1;
        private bool glyphBurningActive = false;
        private bool glyphBurningDead = false;

        // Movement
        private float hoverDistanceFromPlayer = 1000f;
        private float fastSpeedDistance = 300f;
        private float normalSpeed = 7f;
        private float fastSpeed = 13f;
        private float chargeSpeed = 20f;
        private float minCharge = 500f;
        private float currentCharge = 0f;
        private Vector2 chargeStartPos;

        // Attacks
        private int daggerDamage = 100;
        private float daggerSpeed = 30f;
        private int halberdDamage = 500;

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

            npc.lifeMax = 10000;
            npc.damage = 100;
            npc.defense = 75;
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
            npc.netAlways = true;

            // Initialize
            if (initialize)
            {
                DesertMod.instance.ShowDebugUI();

                glyphPetrifying = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("GlyphPetrifying"));
                Main.npc[glyphPetrifying].ai[0] = npc.whoAmI;
                Main.npc[glyphPetrifying].ai[1] = 0;
                Main.npc[glyphPetrifying].ai[2] = -100f;
                Main.npc[glyphPetrifying].ai[3] = -100f;

                glyphCrushing = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("GlyphCrushing"));
                Main.npc[glyphCrushing].ai[0] = npc.whoAmI;
                Main.npc[glyphCrushing].ai[1] = 0;
                Main.npc[glyphCrushing].ai[2] = 0f;
                Main.npc[glyphCrushing].ai[3] = -100f;

                glyphBurning = NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("GlyphBurning"));
                Main.npc[glyphBurning].ai[0] = npc.whoAmI;
                Main.npc[glyphBurning].ai[1] = 0;
                Main.npc[glyphBurning].ai[2] = 100f;
                Main.npc[glyphBurning].ai[3] = -100f;

                initialize = false;
            }

            // Add "tick" to the phase counter of AI
            aiPhase++;

            // Check boss phase
            CheckBossPhase();

            // Check glyph states
            CheckGlyphStatus();

            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

            // NPC initial rotation
            npc.rotation = 0.0f;
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


            // BEHAVIOR
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
                // Follow target and charge three times
                if (!goHigh)
                {
                    if (aiPhase % 50 == 0 && aiPhase % 200 != 0 && aiPhase % 250 != 0)
                    {
                        InitializeCharge(towardsPlayer);
                    }
                    if (!charge)
                    {
                        follow = true;
                    }
                }
                else
                {
                    if (aiPhase % 150 == 0)
                    {
                        shootPattern2 = true;
                    }
                    else if (aiPhase % 50 == 0)
                    {
                        shootPattern1 = true;
                    }
                }

                // Switch hover to follow
                if (aiPhase % 700 == 0)
                {
                    follow = goHigh;
                    goHigh = !goHigh;
                    hover = false;
                    charge = false;
                }

                // Toggle petrifying glyph
                if (aiPhase == 2100)
                {
                    if (!glyphPetrifyingActive) ToggleGlyph(true, false, false);
                    else ToggleGlyph(false, false, false);

                    aiPhase = 0;
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
                    int pro = Projectile.NewProjectile(bossCenter, Vector2.Zero, mod.ProjectileType("DesertBossProjectileHalberd"), halberdDamage, 5f);
                    Main.projectile[pro].ai[0] = npc.whoAmI;
                    Main.projectile[pro].ai[1] = 0f;
                }
                if (aiPhase == 200) charge = false;
                if (aiPhase == 210)
                {
                    charge = true;
                    chargeDirection = towardsPlayer;
                    int pro = Projectile.NewProjectile(bossCenter, Vector2.Zero, mod.ProjectileType("DesertBossProjectileHalberd"), halberdDamage, 5f);
                    Main.projectile[pro].ai[0] = npc.whoAmI;
                    Main.projectile[pro].ai[1] = 1f;
                }
                if (aiPhase > 260)
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

            // Execute behaviour according to flags
            if (follow)
            {
                MoveTowards(npc, target - towardsPlayer, (float)(distance > fastSpeedDistance ? fastSpeed : normalSpeed), 30f);
            }
            if (hover)
            {
                MoveTowards(npc, target - towardsPlayer * hoverDistanceFromPlayer, (float)(distance > fastSpeedDistance ? fastSpeed : normalSpeed), 30f);
            }
            if (charge)
            {
                npc.velocity = chargeDirection * chargeSpeed;
                currentCharge += npc.velocity.Length();
                if (Vector2.Distance(npc.Center, chargeStartPos) > minCharge * 0.5f && currentCharge > minCharge) charge = false;
            }
            if (goHigh)
            {
                MoveTowards(npc, target + new Vector2(0, -500f), fastSpeed, 20f);
            }
            if (shootPattern1)
            {
                Projectile.NewProjectile(bossCenter, towardsPlayer * daggerSpeed, mod.ProjectileType("DesertBossProjectileSpiritDagger"), daggerDamage, 0f);

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

        // Toggles glyphs
        private void ToggleGlyph(bool petrifying, bool crushing, bool burning)
        {
            // Petrify glyph
            if (petrifying)
            {
                if (!glyphPetrifyingActive && !glyphPetrifyingDead)
                {
                    Main.npc[glyphPetrifying].ai[1] = 1;
                    glyphPetrifyingActive = true;
                }
            }
            else if (glyphPetrifyingActive)
            {
                Main.npc[glyphPetrifying].ai[1] = 0;
                glyphPetrifyingActive = false;
            }

            // Crushing glyph
            if (crushing)
            {
                if (!glyphCrushingActive && !glyphCrushingDead)
                {
                    Main.npc[glyphCrushing].ai[1] = 1;
                    Main.npc[glyphCrushing].ai[4] = npc.Center.X;
                    Main.npc[glyphCrushing].ai[5] = npc.Center.Y;
                    glyphCrushingActive = true;
                }
            }
            else if (glyphCrushingActive) 
            {
                Main.npc[glyphCrushing].ai[1] = 0;
                glyphCrushingActive = false;
            }

            // Burning glyph
            if (burning)
            {
                if (!glyphBurningActive && !glyphBurningDead)
                {
                    Main.npc[glyphBurning].ai[1] = 1;
                    glyphBurningActive = true;
                }
            }
            else if (glyphBurningActive)
            {
                Main.npc[glyphBurning].ai[1] = 0;
                glyphBurningActive = false;
            }
        }

        // Check glyph states
        // npc.ai[i] != 0 means glyph is dead (where i: 0 = petrifying, 1 = crushing, 2 = burning)
        private void CheckGlyphStatus()
        {
            // Petrifying
            if (!glyphPetrifyingDead && (int)npc.ai[0] != 0)
            {
                glyphPetrifyingDead = true;
                glyphPetrifyingActive = false;
                IncreaseStats(1.0f, 1.5f, 1.0f);
            }
            // Crushing
            if (!glyphCrushingDead && (int)npc.ai[1] != 0)
            {
                glyphCrushingDead = true;
                glyphCrushingActive = false;
                IncreaseStats(1.3f, 1.3f, 1.0f);
            }
            // Burning
            if (!glyphBurningDead && (int)npc.ai[2] != 0)
            {
                glyphBurningDead = true;
                glyphBurningActive = false;
                IncreaseStats(1.3f, 1.3f, 1.5f);
            }
        }

        // Check boss phase
        private void CheckBossPhase()
        {
            if (npc.life >= npc.lifeMax / 2) currentPhase = BossPhase.HEALTHY;
            else if (npc.life <= npc.lifeMax / 2 && npc.life >= npc.lifeMax / 3) currentPhase = BossPhase.DAMAGED;
            else currentPhase = BossPhase.RAGED;
        }

        // Scale boss attack damages, defense and speed
        private void IncreaseStats(float atk = 1.0f, float def = 1.0f, float spd = 1.0f)
        {
            npc.damage = (int)(npc.damage * atk);
            daggerDamage = (int)(daggerDamage * atk);
            halberdDamage = (int)(halberdDamage * atk);

            npc.defense = (int)(npc.defense * def);

            normalSpeed = normalSpeed * spd;
            fastSpeed = fastSpeed * spd;
        }

        // Set up values for charge
        private void InitializeCharge(Vector2 direction)
        {
            charge = true;
            follow = false;
            chargeDirection = direction;
            chargeStartPos = npc.Center;
            currentCharge = 0f;
        }

        // Move npc towards a target at a certain speed and turn resistance
        private void MoveTowards(NPC npc, Vector2 target, float speed, float turnResistance)
        {
            Vector2 move = target - npc.Center;
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