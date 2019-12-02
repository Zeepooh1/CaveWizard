using System;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace CaveWizard.Game
{
    public class Ground : IDrawable, IUpdateable
    {
       public int DrawOrder { get; }
        public bool Visible { get; }
        public bool Enabled { get; }
        public int UpdateOrder { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;

        private readonly Texture2D _texture2D;
        private Body _groundBody;
        private Vector2 _groundBodySize;
        private Vector2 _groundTextureMetersSize;
        private Vector2 _groundTextureSize;
        private Vector2 _groundTextureOrigin;


        public Ground(ContentManager contentManager, string propName, Vector2 pos, World world)
        {
            _texture2D = contentManager.Load<Texture2D>(propName);

            _groundBodySize = new Vector2(6f, 2f);
            _groundTextureMetersSize = new Vector2(_groundBodySize.X, _groundBodySize.Y);
            _groundTextureSize = new Vector2(_texture2D.Width, _texture2D.Height);
            _groundTextureOrigin = _groundTextureSize / 2;

            _groundBody = world.CreateRectangle(_groundBodySize.X, _groundBodySize.Y, 1f, pos);
            _groundBody.BodyType = BodyType.Static;
            _groundBody.SetRestitution(0f);
            _groundBody.SetFriction(0.5f);
        }
        
        
        public void Update(GameTime gameTime)
        {
        }

        
        
        public void Draw(GameTime gameTime)
        {
            GlobalDevices._SpriteBatch.Draw(_texture2D, _groundBody.Position, null, Color.White, _groundBody.Rotation,
                _groundTextureOrigin, _groundTextureMetersSize / _groundTextureSize, SpriteEffects.FlipVertically, 0f);
            
        }

        public void RemoveFromWorld(World world)
        {
            world.Remove(_groundBody);
        }
    }
}