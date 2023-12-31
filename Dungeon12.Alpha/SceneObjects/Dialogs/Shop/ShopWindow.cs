﻿namespace Dungeon12.Drawing.SceneObjects.Dialogs.Shop
{
    using Dungeon;
    using Dungeon.Control.Keys;
    using Dungeon12.Drawing.SceneObjects.Map;
    using Dungeon12.Drawing.SceneObjects.UI;
    using Dungeon12.Map;
    using Dungeon12.Merchants;
    using Dungeon.View.Interfaces;
    using Dungeon12.Drawing.SceneObjects.Inventories;
    using Dungeon12.Drawing.SceneObjects.Main.CharacterInfo;
    using System;

    public class ShopWindow : DraggableControl<ShopWindow>
    {
        public override int LayerLevel => 50;

        public override bool AbsolutePosition => true;

        protected override Key[] OverrideKeyHandles => new Key[] { Key.Escape };

        public ShopWindow(string title, PlayerSceneObject playerSceneObject, Merchant shop, Action<ISceneObject> destroyBinding, Action<ISceneControl> controlBinding, GameMap gameMap)
        {
            shop.FillBackpacks();

            Global.Freezer.World = this;

            this.Top = 2;
            this.Left = 0;

            var charInfo = new CharacterInfoWindow(gameMap, playerSceneObject, this.ShowInScene, false,false)
            {
                Left = 16 + 5,
                DisableDrag = true
            };
            charInfo.Top = 0;

            var shopWindow = new ShopWindowContent(title, shop, playerSceneObject, charInfo.Inventory)
            {
                Left = 5,
            };

            shopWindow.BindCharacterInventory(charInfo.Inventory);

            this.Width = 28;
            this.Height = 17;

            this.AddChild(shopWindow);
            this.AddChild(charInfo);
            this.AddChild(new InventoryDropItemMask(playerSceneObject, charInfo.Inventory, gameMap)
            {
                Top = -2
            });
        }

        public override void KeyDown(Key key, KeyModifiers modifier, bool hold) => Close();

        private void Close()
        {
            this.Destroy?.Invoke();
            Global.Freezer.World = null;
            Global.Interacting = false;
        }
    }
}