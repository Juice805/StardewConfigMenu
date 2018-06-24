﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewConfigFramework.Options;

namespace StardewConfigMenu.Components {
	using ActionType = Action.ActionType;

	internal class SCMButton: SCMControl {
		internal delegate void ButtonPressedEvent();
		internal event ButtonPressedEvent ButtonPressed;

		private ActionType _ActionType;
		private ActionType PreviousActionType = ActionType.OK;
		public virtual ActionType ActionType { get => _ActionType; set => _ActionType = value; }
		internal override bool Visible { get => Button.visible; set => Button.visible = value; }

		protected ClickableTextureComponent Button;
		public override int Width => Button.bounds.Width;
		public override int Height => Button.bounds.Height;
		public override int X { get => Button.bounds.X; set => Button.bounds.X = value; }
		public override int Y { get => Button.bounds.Y; set => Button.bounds.Y = value; }

		internal SCMButton(string label, ActionType type, bool enabled = true) : this(label, type, 0, 0, enabled) { }

		internal SCMButton(string label, ActionType type, int x, int y, bool enabled = true) : base(label, enabled) {
			_ActionType = type;

			Button = GetButtonTile().ClickableTextureComponent(x, y);
			Button.drawShadow = true;
		}

		protected StardewTile GetButtonTile() {
			switch (ActionType) {
				case ActionType.DONE:
					return StardewTile.DoneButton;
				case ActionType.CLEAR:
					return StardewTile.ClearButton;
				case ActionType.OK:
					return StardewTile.OKButton;
				case ActionType.SET:
					return StardewTile.SetButton;
				case ActionType.GIFT:
					return StardewTile.GiftButton;
				default:
					return StardewTile.OKButton;
			}
		}

		public override void ReceiveLeftClick(int x, int y, bool playSound = true) {
			base.ReceiveLeftClick(x, y, playSound);

			if (Button.containsPoint(x, y) && Enabled && IsAvailableForSelection) {
				if (playSound)
					Game1.playSound("breathin");
				ButtonPressed?.Invoke();
			}
		}

		private void CheckForButtonUpdate() {
			if (PreviousActionType != ActionType) {
				Button = GetButtonTile().ClickableTextureComponent(Button.bounds.X, Button.bounds.Y);
				Button.drawShadow = true;
				PreviousActionType = ActionType;
			}
		}

		public override void Draw(SpriteBatch b) {
			base.Draw(b);

			// draw button
			var labelSize = Game1.dialogueFont.MeasureString(Label);

			CheckForButtonUpdate();
			Button.draw(b, Color.White * ((Enabled) ? 1f : 0.33f), 0.88f);

			Utility.drawTextWithShadow(b, Label, Game1.dialogueFont, new Vector2((float) (Button.bounds.Right + Game1.pixelZoom * 4), (float) (Button.bounds.Y + ((Button.bounds.Height - labelSize.Y) / 2))), Enabled ? Game1.textColor : (Game1.textColor * 0.33f), 1f, 0.1f, -1, -1, 1f, 3);
		}
	}
}
