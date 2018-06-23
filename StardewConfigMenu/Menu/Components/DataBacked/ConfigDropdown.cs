﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewConfigFramework.Options;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace StardewConfigMenu.Components.DataBacked {
	internal class ModDropDownComponent: DropDownComponent {
		readonly private Selection ModData;

		public override bool Enabled {
			get {
				if (!ModData.Enabled)
					return ModData.Enabled;
				else
					return dropDownOptions.Count != 0;
			}
		}

		public override string Label {
			get {
				return ModData.Label;
			}
		}

		public override int SelectionIndex {
			get {
				if (dropDownOptions.Count > 0 && _dropDownDisplayOptions[0] != dropDownOptions[this.ModData.SelectionIndex]) {
					_dropDownDisplayOptions.Insert(0, dropDownOptions[this.ModData.SelectionIndex]);
					_dropDownDisplayOptions.RemoveAt(this.ModData.SelectionIndex + 1);
				}

				return this.ModData.SelectionIndex;
			}
			set {
				if (SelectionIndex == value)
					return;

				_dropDownDisplayOptions.Insert(0, dropDownOptions[value]);
				_dropDownDisplayOptions.RemoveAt(value + 1);
				this.ModData.SelectionIndex = value;
			}
		}

		protected override IReadOnlyList<string> dropDownOptions {
			get {
				if (this.ModData == null) // for base intialization
					return new List<string>();
				else
					return ModData.GetLabels();
			}
		}

		protected override List<string> dropDownDisplayOptions {
			get {
				if (ModData.Choices.Count == 0) {
					_dropDownDisplayOptions.Clear();
				} else {
					var options = dropDownOptions;
					var toRemove = _dropDownDisplayOptions.Except(options).ToList();
					var toAdd = options.Except(_dropDownDisplayOptions).ToList();

					_dropDownDisplayOptions.RemoveAll(x => toRemove.Contains(x));
					_dropDownDisplayOptions.AddRange(toAdd);
				}

				dropDownBackground.bounds.Height = this.dropDown.bounds.Height * this.ModData.Choices.Count;
				return _dropDownDisplayOptions;
			}
		}

		public ModDropDownComponent(Selection option, int width) : this(option, width, 0, 0) { }

		public ModDropDownComponent(Selection option, int width, int x, int y) : base(option.Label, width, x, y) {
			this.ModData = option;
		}

		protected override void SelectDisplayedOption(int DisplayedSelection) {
			if (this.ModData.Choices.Count == 0)
				return;

			var selected = dropDownDisplayOptions[DisplayedSelection];
			ModData.SelectionIndex = this.ModData.IndexOfLabel(selected);
			base.SelectDisplayedOption(DisplayedSelection);
		}

		public override void Draw(SpriteBatch b) {
			base.Draw(b);

			if (this.ModData.Choices.Count == 0)
				return;

			if (!this.IsActiveComponent && (Game1.getMouseX() > this.X) && (Game1.getMouseX() < this.Width + this.X) && (Game1.getMouseY() > this.Y) && (Game1.getMouseY() < this.Height + this.Y)) {
				if (this.ModData.SelectedChoice.HoverText != null) {
					string optionDescription = Utilities.GetWordWrappedString(ModData.SelectedChoice.HoverText);
					IClickableMenu.drawHoverText(b, optionDescription, Game1.smallFont);
				}
			}
		}
	}
}
