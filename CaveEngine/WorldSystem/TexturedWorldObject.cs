using System;
using CaveEngine.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveEngine.WorldSystem
{
    public abstract class TexturedWorldObject : WorldObject, IDrawable
    {
        protected SpriteEffects _creatureEffect;
        protected readonly Texture2D Texture2D;
        protected Vector2 _objectTextureMetersSize;
        protected Vector2 _objectTextureSize;
        protected int _currentFrame;
        public int DrawOrder { get; }
        public bool Visible { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        protected TimeSpan AnimationChange;

        protected TexturedWorldObject(ScreenManager screenManager, string propName, Vector2 objectBodySize,
            Vector2 objectTextureMetersSize, int columns, int rows, PhysicsGameScreen sourceLevel) : base(screenManager,
            objectBodySize, columns, rows, sourceLevel)
        {
            _currentFrame = 0;
            _creatureEffect = SpriteEffects.FlipVertically;
            Texture2D = _screenManager.Content.Load<Texture2D>(propName);
            _objectTextureMetersSize = objectTextureMetersSize;
            _objectTextureSize = new Vector2(Texture2D.Width, Texture2D.Height);
            _objectTextureOrigin = new Vector2(_objectTextureSize.X / _columns, _objectTextureSize.Y / _rows);
            _textureHalf = (_objectTextureOrigin / 2f);

            
        }

        public virtual void Draw(GameTime gameTime)
        {
            int width = Texture2D.Width / _columns;
            int height = Texture2D.Height / _rows;

            Rectangle sourceRectangle = new Rectangle(width * _currColumn, height * _currRow, width, height);

            _screenManager.SpriteBatch.Draw(Texture2D, ObjectBody.Position, sourceRectangle, Color.White,
                ObjectBody.Rotation,
                _textureHalf, _objectTextureMetersSize / (_objectTextureSize / new Vector2(_columns, _rows)),
                _creatureEffect, 0f);
        }
    }
}