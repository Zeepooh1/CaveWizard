using System;
using CaveEngine.ScreenSystem;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace CaveWizard.Game
{
    public class Ground : WorldObject, IDrawable
    {
       public int DrawOrder { get; }
        public bool Visible { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public Ground(ScreenManager screenManager, string propName, Vector2 pos, World world, int columns, int rows) : base(screenManager, propName, new Vector2(6f, 2f), new Vector2(6f, 2f), columns, rows)
        {
            ObjectBody = world.CreateRectangle(_objectBodySize.X, _objectBodySize.Y, 1f, pos);
            ObjectBody.BodyType = BodyType.Static;
            ObjectBody.SetRestitution(0f);
            ObjectBody.SetFriction(0.5f);
        }
        

        
        
        public void Draw(GameTime gameTime)
        {
            _screenManager.SpriteBatch.Draw(Texture2D, ObjectBody.Position, null, Color.White, ObjectBody.Rotation,
                _objectTextureOrigin * new Vector2(_currColumn, _currRow) + _textureHalf, _objectTextureMetersSize / _objectTextureSize, SpriteEffects.FlipVertically, 0f);
            
        }

        public void RemoveFromWorld(World world)
        {
            world.Remove(ObjectBody);
        }
    }
}