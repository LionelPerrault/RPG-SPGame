﻿namespace Dungeon12.Drawing.SceneObjects.Main.CharacterInfo
{
    using System;
    using Dungeon;
    using Dungeon.Drawing.SceneObjects;
    using System.Collections.Generic;
    using Force.DeepCloner;
    using Dungeon12.Classes;
    using Dungeon.Control.Pointer;
    using Dungeon12.SceneObjects; using Dungeon.SceneObjects;
    using Dungeon12.Drawing.SceneObjects.Inventories;
    using Dungeon12.Drawing.SceneObjects.Map;
    using Dungeon12.Drawing.SceneObjects.UI;
    using Dungeon12.Entities.Alive;
    using Dungeon12.Items;
    using Dungeon12.Items.Enums;
    using Dungeon.Types;
    using Dungeon.View.Interfaces;
    using Dungeon.Control;
    using Dungeon.Drawing;

    public class ItemWear : DropableControl<InventoryItem>
    {
        protected override ControlEventType[] Handles => new ControlEventType[]
        {
            ControlEventType.ClickRelease,
            ControlEventType.Click,
            ControlEventType.Focus
        };

        private string borderImage = string.Empty;

        public override bool CacheAvailable => false;

        public override bool AbsolutePosition => true;

        private Character character;
        public ItemKind ItemKind;
        private Inventory inventory;
        
        private DressedItem dressItemControl;

        public ItemWear(Inventory inventory, Character character, ItemKind itemKind)
        {
            this.inventory = inventory;
            this.ItemKind = itemKind;
            this.character = character;
            var tall = itemKind == ItemKind.Weapon || itemKind == ItemKind.OffHand;

            this.borderImage = tall
                ? "Dungeon12.Resources.Images.ui.squareWeapon"
                : "Dungeon12.Resources.Images.ui.square";

            this.Width = 2;
            this.Height = tall
                ? 4
                : 2;

            if (itemKind == ItemKind.Deck) // or another small like charm or ring/key
            {
                this.Width = 1;
                this.Height = 1;
            }

            this.Image = SquareTexture();

            this.dressItemControl = new DressedItem(null);
            this.AddChild(this.dressItemControl);

            DressUpCurrent(character, itemKind);
        }

        private void DressUpCurrent(Character character, ItemKind itemKind)
        {
            switch (itemKind)
            {
                case ItemKind.Weapon:
                case ItemKind.Helm:
                case ItemKind.Armor:
                case ItemKind.Boots:
                case ItemKind.OffHand:
                case ItemKind.Deck:
                    {
                        var cloth = character.Clothes.GetProperty<Item>(itemKind.ToString());
                        if (cloth != null)
                        {
                            this.dressItemControl.Dress(cloth);
                        }
                        break;
                    }
                default:
                    break;
            }
        }

        private string SquareTexture(bool focus = false)
        {
            var f = focus
                ? "_f"
                : "";

            return $"{borderImage}{f}.png";
        }

        public override void Focus()
        {
            if (DragAndDropSceneControls.IsDragging)
            {
                base.Focus();
                if (this.DropAvailable)
                {
                    this.Image = SquareTexture(true);
                }
            }
            else
            {
                this.Image = SquareTexture(true);
            }

            ShowTooltip();
        }

        protected override bool CheckDropAvailable(InventoryItem source)
        {
            return source.Item.Kind == this.ItemKind;
        }

        public override void Unfocus()
        {
            this.Image = SquareTexture();
            base.Unfocus();
        }

        protected override void OnDrop(InventoryItem source)
        {
            if (source.Parent is Inventory sourceInventory) // получить таргет (бллядь, это уже второй такой хак, надо переработать к хуям)
            {
                if (source.Item.Kind == this.ItemKind && sourceInventory.Parent is CharacterInfoWindow)
                {
                    WearItem(source, true);
                }
                else
                {
                    source.Destroy?.Invoke();
                    sourceInventory.Refresh();
                }
            }
        }

        public void WearItem(InventoryItem source, bool dropping = false)
        {
            var (success, oldItem) = character.Clothes.PutOn(source.Item);

            if (success)
            {
                if (oldItem != null)
                {
                    character.Backpack.Add(this.dressItemControl.item);
                    inventory.Refresh();
                }

                if (dropping)
                {
                    Global.DrawClient.Drop();
                }

                character.Backpack.Remove(source.Item);
                inventory.Refresh();

                this.dressItemControl.Dress(source.Item);

                source.Destroy?.Invoke();
            }
        }
        
        public override void Click(PointerArgs args)
        {
            if (args.MouseButton == MouseButton.Right)
            {
                DressOffItem();
            }
        }

        private void DressOffItem()
        {
            if (character.Clothes.PutOff(this.ItemKind).success)
            {
                this.character.Backpack.Add(this.dressItemControl.item);
                inventory.Refresh();
                this.dressItemControl.Undress();
            }
        }

        protected override bool ProvidesTooltip => true;

        protected override Tooltip ProvideTooltip(Point position)
        {
            if (dressItemControl.item != default)
                return new ItemTooltip(dressItemControl.item, position);
            else
                return new Tooltip(ItemKind.ToDisplay(), position, DrawColor.WhiteSmoke);
        }
        protected override void CallOnEvent(dynamic obj)
        {
            OnEvent(obj);
        }

        private class DressedItem : EmptyTooltipedSceneObject
        {
            public override bool CacheAvailable => false;

            public override bool AbsolutePosition => true;

            public Item item;

            public DressedItem(Item item) : base(item?.Description) => Dress(item);

            public void Dress(Item itemSource)
            {
                if (itemSource != null)
                {
                    var item = itemSource.DeepClone();

                    this.TooltipText = item.Description;
                    this.item = item;
                    this.Image = item.Tileset;
                    this.ImageRegion = item.TileSetRegion;

                    var tall = item.Kind == ItemKind.Weapon || item.Kind == ItemKind.OffHand;

                    this.Height = tall ? 4 : 2;
                    this.Width = 2;

                    if (item.Kind == ItemKind.Deck)
                    {
                        this.Height = 1;
                        this.Width = 1;
                    }
                }
            }

            public void Undress()
            {
                this.TooltipText = string.Empty;
                this.item = null;
                this.Image = string.Empty;
                this.ImageRegion = new Dungeon.Types.Rectangle(1, 1, 1, 1);
            }

            public bool IsEmpty => item == null;

            protected override void CallOnEvent(dynamic obj)
            {
                OnEvent(obj);
            }
        }
    }
}