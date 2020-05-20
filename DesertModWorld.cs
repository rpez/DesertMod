using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using System.IO;

namespace DesertMod
{
    class DesertModWorld : ModWorld
    {
        public static bool DesertBossDowned = false;

        public override void Initialize()
        {
            DesertBossDowned = true;
        }

        public override TagCompound Save()
        {
            var downed = new List<string>();
            if (DesertBossDowned) downed.Add("desertBoss");

            return new TagCompound
            {
                {
                    "Version", 0
                },
                {
                    "Downed", downed
                }
            };
        }

        public override void Load(TagCompound tag)
        {
            var downed = tag.GetList<string>("Downed");
            DesertBossDowned = downed.Contains("desertBoss");
        }

        public override void LoadLegacy(BinaryReader reader)
        {
            int loadVersion = reader.ReadInt32();
            if (loadVersion == 0)
            {
                BitsByte flags = reader.ReadByte();
                DesertBossDowned = flags[0];
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = DesertBossDowned;
        }

        public override void NetReceive(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            DesertBossDowned = flags[0];
        }
    }
