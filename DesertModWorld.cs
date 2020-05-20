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
            var Downed = new List<string>();
            if (DesertBossDowned) Downed.Add("desertBoss");

            return new TagCompound
            {
                {
                    "Version", 0
                },
                {
                    "Downed", Downed
                }
            };
        }
    }
}
