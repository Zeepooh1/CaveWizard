using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveEngine.ScreenSystem
{
    public class MenuCheckBox : MenuButton, IInvokable
    {
        private int _checkmark;
        public bool IsChecked;
        private Vector2 _checkBoxTextureSize;

        public MenuCheckBox(Texture2D sprite, bool flip, Vector2 position, GameScreen screen, int checkmark, bool isChecked, int entriesIndex) :
            base(sprite, flip, position, screen, entriesIndex)
        {
            _checkmark = checkmark;
            this.IsChecked = isChecked;
            _checkBoxTextureSize = new Vector2(_sprite.Width, _sprite.Height);

        }

        public override void Draw()
        {
            SpriteBatch batch = _screen.ScreenManager.SpriteBatch;
            var col = new Color(235, 204, 255);
            var colSel = new Color(203, 164, 229);
            Color color = Color.Lerp(col, colSel, _selectionFade);
            
            int width =  _sprite.Width / 2; 
            int height = _sprite.Height;
            int check;
            if (_checkmark == 0)
            {
                if (IsChecked)
                {
                    check = 0;
                }
                else
                {
                    check = 1;
                }
            }
            else
            {
                if (IsChecked)
                {
                    check = 1;
                }
                else
                {
                    check = 0;
                }
            }
            Rectangle sourceRectangle = new Rectangle(width * check, height * 0, width, height);


            batch.Draw(_sprite, Position, sourceRectangle, color, 0f, _baseOrigin,new Vector2(30f) / (_checkBoxTextureSize / new Vector2(2, 1)), _flip ? SpriteEffects.FlipVertically : SpriteEffects.None, 0f);
            
        }

        public void Change()
        {
            IsChecked ^= true;
        }
    }
}