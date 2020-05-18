using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DesertMod.Items.Boss
{
	public class DesertBossSummonItem : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Ominous Sand"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			Tooltip.SetDefault("It's coarse and rough and irritating and it gets everywhere.");
		}

		public override void SetDefaults() 
		{
			item.width = 26;
			item.height = 26;
			item.maxStack = 20;
			item.rare = ItemRarityID.Blue;
			item.useAnimation = 45;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.consumable = true;
		}

		public override void AddRecipes() 
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override bool CanUseItem(Player player)
		{
			return player.ZoneDesert;
		}

		public override bool UseItem(Player player)
		{
			Main.PlaySound(SoundID.Roar, player.position);
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DesertBoss"));
			}
			return true;
		}
	}
}