// Copyright (c) 2017 Kastellanos Nikolaos

/* Original source Farseer Physics Engine:
 * Copyright (c) 2014 Ian Qvist, http://farseerphysics.codeplex.com
 * Microsoft Permissive License (Ms-PL) v1.1
 */

using System;
using System.Collections.Generic;
using CaveEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveEngine.ScreenSystem
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public class MenuScreen : GameScreen
    {
        private const float NumEntries = 15;
        private List<MenuEntry> _menuEntries = new List<MenuEntry>();
        private string _menuTitle;
        private Vector2 _titlePosition;
        private Vector2 _titleOrigin;
        private int _selectedEntry;
        private float _menuBorderTop;
        private float _menuBorderBottom;
        private float _menuBorderMargin;
        private float _menuOffset;
        private float _maxOffset;

        private Texture2D _texScrollButton;
        private Texture2D _texSlider;

        private MenuButton _scrollUp;
        private MenuButton _scrollDown;
        private MenuButton _scrollSlider;
        private List<MenuButton> _menuButtons = new List<MenuButton>();
        private int _selectedButton;
        private bool _scrollLock;

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            _menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.7);
            TransitionOffTime = TimeSpan.FromSeconds(0.7);
            HasCursor = true;
        }

        public void AddMenuItem(string name, EntryType type, GameScreen screen)
        {
            MenuEntry entry = new MenuEntry(this, name, type, screen);
            _menuEntries.Add(entry);
        }

        public MenuButton AddMenuButton(ButtonType type, string tex, string seperatorText, bool isChecked)
        {
            AddMenuItem(seperatorText, EntryType.Separator, null);
            MenuEntry entry = _menuEntries[^1];
            MenuButton button;
            Texture2D sprite = ScreenManager.Content.Load<Texture2D>(tex);
            switch (type)
            {
                case ButtonType.Base:
                    button = new MenuButton(sprite, false, entry.Position + new Vector2(10f, 0f), this, _menuEntries.Count-1);
                    break;
                case ButtonType.CheckBox:
                    button = new MenuCheckBox(sprite, false, entry.Position + new Vector2(10f, 0f), this, 1, isChecked, _menuEntries.Count-1);
                    break;
                default:
                    button = null;
                    break;
            }

            _menuButtons.Add(button);

            return button;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            SpriteFont font = ScreenManager.Fonts.MenuSpriteFont;

            _texScrollButton = ScreenManager.Content.Load<Texture2D>("Common/arrow");
            _texSlider = ScreenManager.Content.Load<Texture2D>("Common/slider");

            float scrollBarPos = viewport.Width / 2f;
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                _menuEntries[i].Initialize();
                scrollBarPos = Math.Min(scrollBarPos,
                                         (viewport.Width - _menuEntries[i].GetWidth()) / 2f);
            }
            scrollBarPos -= _texScrollButton.Width + 2f;

            _titleOrigin = font.MeasureString(_menuTitle) / 2f;
            _titlePosition = new Vector2(viewport.Width / 2f, font.MeasureString("M").Y / 2f + 10f);

            _menuBorderMargin = font.MeasureString("M").Y * 0.8f;
            _menuBorderTop = (viewport.Height - _menuBorderMargin * (NumEntries - 1)) / 2f;
            _menuBorderBottom = (viewport.Height + _menuBorderMargin * (NumEntries - 1)) / 2f;

            _menuOffset = 0f;
            _maxOffset = Math.Max(0f, (_menuEntries.Count - NumEntries) * _menuBorderMargin);

            _scrollUp = new MenuButton(_texScrollButton, false, new Vector2(scrollBarPos, _menuBorderTop - _texScrollButton.Height), this, -1);
            _scrollDown = new MenuButton(_texScrollButton, true, new Vector2(scrollBarPos, _menuBorderBottom + _texScrollButton.Height), this, -1);
            _scrollSlider = new MenuButton(_texSlider, false, new Vector2(scrollBarPos, _menuBorderTop), this, -1);

            _scrollLock = false;
        }

        /// <summary>
        /// Returns the index of the menu entry at the position of the given mouse state.
        /// </summary>
        /// <returns>Index of menu entry if valid, -1 otherwise</returns>
        private int GetMenuEntryAt(Vector2 position)
        {
            int index = 0;
            foreach (MenuEntry entry in _menuEntries)
            {
                float width = entry.GetWidth();
                float height = entry.GetHeight();
                Rectangle rect = new Rectangle((int)(entry.Position.X - width / 2f), (int)(entry.Position.Y - height / 2f), (int)width, (int)height);

                if (rect.Contains((int)position.X, (int)position.Y) && entry.Alpha > 0.1f)
                    return index;

                ++index;
            }
            return -1;
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputHelper input, GameTime gameTime)
        {
            // Mouse or touch on a menu item
            int hoverIndex = GetMenuEntryAt(input.Cursor);
            if (hoverIndex > -1 && _menuEntries[hoverIndex].IsSelectable() && !_scrollLock)
                _selectedEntry = hoverIndex;
            else
                _selectedEntry = -1;

            hoverIndex = GetButtonAt(input.Cursor);
            if (hoverIndex > -1)
            {
                _selectedButton = hoverIndex;
            }
            else
            {
                _selectedButton = -1;
            }

            if (_selectedButton > -1)
            {
                if (_menuButtons[_selectedButton] is IInvokable && input.IsMenuSelect())
                {
                    _menuButtons[_selectedButton].ClickedOn?.Invoke(_menuButtons[_selectedButton], null);
                }
            }

            _scrollSlider.Hover = false;
            if (input.IsCursorValid)
            {
                _scrollUp.Collide(input.Cursor);
                _scrollDown.Collide(input.Cursor);
                _scrollSlider.Collide(input.Cursor);
            }
            else
            {
                _scrollUp.Hover = false;
                _scrollDown.Hover = false;
                _scrollLock = false;
            }

            // Accept or cancel the menu? 
            if (input.IsMenuSelect() && _selectedEntry != -1)
            {
                if (_menuEntries[_selectedEntry].IsExitItem())
                    ScreenManager.Game.Exit();
                else if (_menuEntries[_selectedEntry].IsBackItem())
                {
                    if (_menuEntries[_selectedEntry].Screen != null)
                    {
                        if (_menuEntries[_selectedEntry].Screen is IMenubale)
                        {
                            ((IMenubale)_menuEntries[_selectedEntry].Screen).MenuDestroyed();
                        }
                    }
                    ExitScreen();
                }
                else if (_menuEntries[_selectedEntry].IsToMainMenu())
                {
                    if (_menuEntries[_selectedEntry].Screen != null)
                    {
                        if (_menuEntries[_selectedEntry].Screen is IMenubale)
                        {
                            ((IMenubale)_menuEntries[_selectedEntry].Screen).ToMainMenu();
                        }
                    }
                    ExitScreen();
                }
                else if (_menuEntries[_selectedEntry].Screen != null)
                {
                    ScreenManager.AddScreen(_menuEntries[_selectedEntry].Screen);
                }
                
            }
            else if (input.IsMenuCancel())
                ScreenManager.Game.Exit();

            if (input.IsMenuPressed())
            {
                if (_scrollUp.Hover)
                {
                    _menuOffset = Math.Max(_menuOffset - 200f * (float)gameTime.ElapsedGameTime.TotalSeconds, 0f);
                    _scrollLock = false;
                }
                if (_scrollDown.Hover)
                {
                    _menuOffset = Math.Min(_menuOffset + 200f * (float)gameTime.ElapsedGameTime.TotalSeconds, _maxOffset);
                    _scrollLock = false;
                }
                if (_scrollSlider.Hover)
                {
                    _scrollLock = true;
                }
            }

            if (input.WheelDelta != 0)
            {
                _menuOffset = MathHelper.Clamp(_menuOffset - 32f * (input.WheelDelta / 120f), 0f, _maxOffset);
                _scrollLock = false;
            }


            if (input.IsMenuReleased())
                _scrollLock = false;

            if (_scrollLock)
            {
                _scrollSlider.Hover = true;
                _menuOffset = Math.Max(Math.Min(((input.Cursor.Y - _menuBorderTop) / (_menuBorderBottom - _menuBorderTop)) * _maxOffset, _maxOffset), 0f);
            }
        }

        private int GetButtonAt(Vector2 position)
        {
            int index = 0;
            foreach (MenuButton button in _menuButtons)
            {
                button.Collide(position);
                if (button.Hover)
                    return index;

                ++index;
            }
            return -1;
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            Vector2 position = Vector2.Zero;
            position.Y = _menuBorderTop - _menuOffset;

            // update each menu entry's location in turn
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                position.X = ScreenManager.GraphicsDevice.Viewport.Width / 2f;
                if (ScreenState == ScreenState.TransitionOn)
                    position.X -= transitionOffset * 256;
                else
                    position.X += transitionOffset * 256;

                // set the entry's position
                _menuEntries[i].Position = position;

                if (position.Y < _menuBorderTop)
                    _menuEntries[i].Alpha = 1f - Math.Min(_menuBorderTop - position.Y, _menuBorderMargin) / _menuBorderMargin;
                else if (position.Y > _menuBorderBottom)
                    _menuEntries[i].Alpha = 1f - Math.Min(position.Y - _menuBorderBottom, _menuBorderMargin) / _menuBorderMargin;
                else
                    _menuEntries[i].Alpha = 1f;

                // move down for the next entry the size of this entry
                position.Y += _menuEntries[i].GetHeight();
            }
            for (int i = 0; i < _menuButtons.Count; ++i)
            {
               
                // set the entry's position
                _menuButtons[i].Position = _menuEntries[_menuButtons[i].EntriesIndex].Position + new Vector2(100f, 0);
                // move down for the next entry the size of this entry
            }
            
            Vector2 scrollPos = _scrollSlider.Position;
            scrollPos.Y = MathHelper.Lerp(_menuBorderTop, _menuBorderBottom, _menuOffset / _maxOffset);
            _scrollSlider.Position = scrollPos;
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                bool isSelected = IsActive && (i == _selectedEntry);
                _menuEntries[i].Update(isSelected, gameTime);
            }

            _scrollUp.Update(gameTime);
            _scrollDown.Update(gameTime);
            _scrollSlider.Update(gameTime);
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Fonts.MenuSpriteFont;

            spriteBatch.Begin();
            // Draw each menu entry in turn.
            for (int i = 0; i < _menuEntries.Count; ++i)
            {
                _menuEntries[i].Draw();
            }

            foreach (MenuButton menuButton in _menuButtons)
            {
                menuButton.Draw();
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            Vector2 transitionOffset = new Vector2(0f, (float)Math.Pow(TransitionPosition, 2) * 100f);

            spriteBatch.DrawString(font, _menuTitle, _titlePosition - transitionOffset + Vector2.One, Color.LightGray, 0, _titleOrigin, 1f, SpriteEffects.None, 0);
            //spriteBatch.DrawString(font, _menuTitle, _titlePosition - transitionOffset, new Color(164, 190, 229); , 0, _titleOrigin, 1f, SpriteEffects.None, 0);
            _scrollUp.Draw();
            _scrollSlider.Draw();
            _scrollDown.Draw();
            spriteBatch.End();
        }
    }
}