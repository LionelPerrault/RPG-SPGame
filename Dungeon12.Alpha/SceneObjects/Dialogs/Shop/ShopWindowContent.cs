﻿namespace Dungeon12.Drawing.SceneObjects.Dialogs.Shop
{
    using Dungeon12.Drawing.SceneObjects.Inventories;
    using Dungeon12.Drawing.SceneObjects.Map;
    using Dungeon12.SceneObjects; using Dungeon.SceneObjects;
    using Dungeon.Drawing;

    public class ShopWindowContent : EmptySceneControl
    {
        public override bool AbsolutePosition => true;

        public override bool CacheAvailable => false;

        public void BindCharacterInventory(Inventory inventory)
        {
            inventory.Refresh(ShopTab.Current.ShopInventory);
            ShopTab.OnChange = tab =>
            {                
                inventory.Refresh(tab.ShopInventory);
            };
        }

        public ShopWindowContent(string title, Dungeon12.Merchants.Merchant merchant, PlayerSceneObject playerSceneObject, Inventory another)
        {
            this.Image = "Dungeon12.Resources.Images.ui.vertical_title(17x15).png";

            this.Width = 15;
            this.Height = 17;

            var txt = this.AddTextCenter(new DrawText(title), true, false);
            txt.Top += 0.2;

            foreach (var category in merchant.Categories)
            {
                var index = merchant.Categories.IndexOf(category);
                var tab = new ShopTab(this, another,merchant, category, playerSceneObject, index == 0)
                {
                    AbsolutePosition = true,
                    CacheAvailable = false,
                    Left = index * 3,
                    Top = 1.5,
                    ZIndex = this.ZIndex
                };
                this.AddChild(tab);

                if (index == 0)
                {
                    tab.Open();
                }
            }
        }
    }
}