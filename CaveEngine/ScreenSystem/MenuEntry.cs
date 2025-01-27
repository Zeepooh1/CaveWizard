﻿﻿/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

 using System;
 using Microsoft.Xna.Framework;
 using Microsoft.Xna.Framework.Graphics;

 namespace CaveEngine.ScreenSystem
{
    public enum EntryType
    {
        Screen,
        Separator,
        ExitItem,
        Back,
        ToMainMenu
    }

    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public sealed class MenuEntry
    {
        private Vector2 _baseOrigin;

        private float _height;
        private MenuScreen _menu;

        private float _scale;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float _selectionFade;

        private EntryType _type;
        private float _width;

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(MenuScreen menu, string text, EntryType type, GameScreen screen)
        {
            Text = text;
            Screen = screen;
            _type = type;
            _menu = menu;
            _scale = 0.9f;
            Alpha = 1.0f;
        }


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position { get; set; }

        public float Alpha { get; set; }

        public GameScreen Screen { get; private set; }

        public void Initialize()
        {
            SpriteFont font = _menu.ScreenManager.Fonts.MenuSpriteFont;

            _baseOrigin = new Vector2(font.MeasureString(Text).X, font.MeasureString("M").Y) * 0.5f;

            _width = font.MeasureString(Text).X * 0.8f;
            _height = font.MeasureString("M").Y * 0.8f;
        }

        public bool IsExitItem()
        {
            return _type == EntryType.ExitItem;
        }

        public bool IsSelectable()
        {
            return _type != EntryType.Separator;
        }

        public bool IsBackItem()
        {
            return _type == EntryType.Back;
        }

        public bool IsToMainMenu()
        {
            return _type == EntryType.ToMainMenu;
        }
        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public void Update(bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            if (_type != EntryType.Separator)
            {
                float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;
                if (isSelected)
                    _selectionFade = Math.Min(_selectionFade + fadeSpeed, 1f);
                else
                    _selectionFade = Math.Max(_selectionFade - fadeSpeed, 0f);

                _scale = 0.7f + 0.1f * _selectionFade;
            }
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public void Draw()
        {
            SpriteFont font = _menu.ScreenManager.Fonts.MenuSpriteFont;
            SpriteBatch batch = _menu.ScreenManager.SpriteBatch;

            // Draw the selected entry   
            var col = new Color(235, 204, 255);
            var colSel = new Color(203, 164, 229);
            //var colSep = new Color(164, 190, 229);
            var colSep = new Color(164, 229, 203);


            Color color = _type == EntryType.Separator ? colSep : Color.Lerp(col, colSel, _selectionFade);
            color *= Alpha;

            // Draw text, centered on the middle of each line.
            batch.DrawString(font, Text, Position - _baseOrigin * _scale + Vector2.One, Color.DarkSlateGray * Alpha * Alpha, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
            batch.DrawString(font, Text, Position - _baseOrigin * _scale, color, 0, Vector2.Zero, _scale, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public int GetHeight()
        {
            return (int)_height;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public int GetWidth()
        {
            return (int)_width;
        }
    }
}