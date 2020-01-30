using System;
using System.Collections.Generic;
using System.Diagnostics;
using CaveEngine.ScreenSystem;
using CaveEngine.WorldSystem;
using CaveWizard.Globals;
using CaveWizard.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace CaveWizard.Game
{
    public class Ground : TexturedWorldObject
    {
       public int DrawOrder { get; }
        public bool Visible { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public Ground(ScreenManager screenManager, string propName, Vector2 pos, World world, int columns, int rows, List<List<char>> level, Int32 i, Int32 j, Level sourceLevel) : base(screenManager, propName, new Vector2(1f, 1f), new Vector2(1f, 1f), columns, rows, sourceLevel)
        {
            ObjectBody = world.CreateRectangle(_objectBodySize.X, _objectBodySize.Y, 1f, pos);
            ObjectBody.BodyType = BodyType.Static;
            ObjectBody.SetRestitution(0f);
            ObjectBody.SetFriction(0.5f);
            if (j == 0)
            {
                _currColumn = 0;
            }
            else if (level[i][j - 1] == '.')
            {
                if (j != level[i].Count - 1)
                {
                    if (level[i][j + 1] == '.')
                    {
                        _currColumn = 1;
                    }
                }
                else
                {
                    _currColumn = 0;
                }
            }
            else if (j == level[i].Count - 1)
            {
                _currColumn = 2;
            }
            else if (level[i][j + 1] == '.')
            {
                _currColumn = 2;
            }
            else
            {
                _currColumn = 1;
            }

            if (rows > 1)
            {
                _currColumn = j;
                _currRow = i;
            }
            
        }
        

        
        
        public void Draw(GameTime gameTime)
        {
            int width = Texture2D.Width / _columns;
            int height = Texture2D.Height / _rows;

            Rectangle sourceRectangle = new Rectangle(width * _currColumn, height * _currRow, width, height);
            
            _screenManager.SpriteBatch.Draw(Texture2D, ObjectBody.Position, sourceRectangle, Color.White, ObjectBody.Rotation,
                _textureHalf, _objectTextureMetersSize / (_objectTextureSize / new Vector2(_columns, _rows)), SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally, 0f);
            
        }

        public void RemoveFromWorld(World world)
        {
            world.Remove(ObjectBody);
        }
    }
}