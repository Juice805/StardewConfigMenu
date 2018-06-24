﻿using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewConfigMenu.Components;
using Microsoft.Xna.Framework.Input;

namespace StardewConfigMenu {
	public class MenuPage: IClickableMenu {

		private MenuController controller;
		private List<ModSheet> Sheets = new List<ModSheet>();
		private DropdownComponent modSelected;

		internal MenuPage(MenuController controller, int x, int y, int width, int height) : base(x, y, width, height, false) {
			this.controller = controller;

			var modChoices = new List<string>();

			for (int i = 0; i < this.controller.ModOptionsList.Count; i++) {

				// Create mod page and add it, hide it initially
				this.Sheets.Add(new ModSheet(this.controller.ModOptionsList[i], (int) (this.xPositionOnScreen + Game1.pixelZoom * 15), (int) (this.yPositionOnScreen + Game1.pixelZoom * 55), this.width - (Game1.pixelZoom * 15), this.height - Game1.pixelZoom * 65));
				this.Sheets[i].Visible = false;

				// Add names to mod selector dropdown
				modChoices.Add(this.controller.ModOptionsList[i].ModManifest.Name);
			}

			modSelected = new DropdownComponent(modChoices, "", (int) Game1.smallFont.MeasureString("Stardew Configuration Menu Framework").X, (int) (this.xPositionOnScreen + Game1.pixelZoom * 15), (int) (this.yPositionOnScreen + Game1.pixelZoom * 30));
			modSelected.Visible = true;
			if (Sheets.Count > 0)
				Sheets[modSelected.SelectionIndex].Visible = true;

			AddListeners();

		}

		static public void SetActive() {
			var gameMenu = (GameMenu) Game1.activeClickableMenu;
			if (MenuController.pageIndex != null)
				gameMenu.currentTab = (int) MenuController.pageIndex;
		}

		public override void receiveRightClick(int x, int y, bool playSound = true) { }

		internal void AddListeners() {
			RemoveListeners();

			ControlEvents.MouseChanged += MouseChanged;
			modSelected.OptionSelected += DisableBackgroundSheets;
		}

		internal void RemoveListeners(bool children = false) {
			if (children) {
				modSelected.Visible = false;
				Sheets.ForEach(x => { x.RemoveListeners(true); });
			}

			modSelected.OptionSelected -= DisableBackgroundSheets;
			ControlEvents.MouseChanged -= MouseChanged;

		}

		private void DisableBackgroundSheets(int selected) {
			for (int i = 0; i < Sheets.Count; i++) {
				Sheets[i].Visible = (i == selected);
			}
		}

		protected virtual void MouseChanged(object sender, EventArgsMouseStateChanged e) {
			if (GameMenu.forcePreventClose) { return; }
			if (!(Game1.activeClickableMenu is GameMenu)) { return; } // must be main menu
			if ((Game1.activeClickableMenu as GameMenu).currentTab != MenuController.pageIndex) { return; } //must be mod tab

			var currentSheet = Sheets.Find(x => x.Visible);

			if (e.NewState.ScrollWheelValue > e.PriorState.ScrollWheelValue) {
				if (currentSheet != null)
					currentSheet.receiveScrollWheelAction(1);
			} else if (e.NewState.ScrollWheelValue < e.PriorState.ScrollWheelValue) {
				if (currentSheet != null)
					currentSheet.receiveScrollWheelAction(-1);
			}


			if (e.PriorState.LeftButton == ButtonState.Released) {
				if (e.NewState.LeftButton == ButtonState.Pressed) {
					// clicked
					if (currentSheet != null)
						currentSheet.receiveLeftClick(e.NewPosition.X, e.NewPosition.Y);
					modSelected.ReceiveLeftClick(e.NewPosition.X, e.NewPosition.Y);
				}
			} else if (e.PriorState.LeftButton == ButtonState.Pressed) {
				if (e.NewState.LeftButton == ButtonState.Pressed) {
					if (currentSheet != null)
						currentSheet.leftClickHeld(e.NewPosition.X, e.NewPosition.Y);
					modSelected.LeftClickHeld(e.NewPosition.X, e.NewPosition.Y);
				} else if (e.NewState.LeftButton == ButtonState.Released) {
					if (currentSheet != null)
						currentSheet.releaseLeftClick(e.NewPosition.X, e.NewPosition.Y);
					modSelected.ReleaseLeftClick(e.NewPosition.X, e.NewPosition.Y);
				}
			}

		}

		//private OptionComponent tester = new SliderComponent("Hey", 0, 10, 1, 5, true);

		public override void draw(SpriteBatch b) {
			//base.draw(b);
			//tester.draw(b);
			if (!(Game1.activeClickableMenu is GameMenu)) { return; } // must be main menu
			if ((Game1.activeClickableMenu as GameMenu).currentTab != MenuController.pageIndex) { return; } //must be mod tab



			Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, null, false);

			base.drawHorizontalPartition(b, (int) (this.yPositionOnScreen + Game1.pixelZoom * 40));

			if (Sheets.Count > 0)
				Sheets[modSelected.SelectionIndex].draw(b);

			// draw mod select dropdown last, should cover mod settings
			modSelected.Draw(b);
			SpriteText.drawString(b, "Mod Options", modSelected.X + modSelected.Width + Game1.pixelZoom * 5, modSelected.Y);

			if (Sheets.Count > 0 && (Game1.getMouseX() > this.modSelected.X && Game1.getMouseX() < this.modSelected.X + this.modSelected.Width) && (Game1.getMouseY() > this.modSelected.Y && Game1.getMouseY() < this.modSelected.Y + this.modSelected.Height) && !modSelected.IsActiveComponent) {
				IClickableMenu.drawHoverText(Game1.spriteBatch, Utilities.GetWordWrappedString(this.controller.ModOptionsList[modSelected.SelectionIndex].ModManifest.Description), Game1.smallFont);
			}
		}
	}
}
