using CaveEngine.ScreenSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace CaveWizard.Game
{
    public abstract class WorldObject
    {
        public Body ObjectBody { get; set; }
        protected readonly Texture2D Texture2D;
        protected Vector2 _objectBodySize;
        protected Vector2 _objectTextureMetersSize;
        protected Vector2 _objectTextureSize;
        protected Vector2 _objectTextureOrigin;
        protected ScreenManager _screenManager;
        protected int _currentFrame;
        protected int _columns;
        protected int _rows;
        protected int _currColumn;
        protected int _currRow;
        protected int _texutreRow;
        protected int _textureColumn;
        protected Vector2 _textureHalf;

        public WorldObject(ScreenManager screenManager, string propName, Vector2 objectBodySize, Vector2 objectTextureMetersSize, int columns, int rows)
        {
            _currentFrame = 0;
            _screenManager = screenManager;
            _objectBodySize = objectBodySize;
            _columns = columns;
            _rows = rows;
            _currColumn = 0;
            _currRow = 0;
            if (propName != null)
            {
                Texture2D = _screenManager.Content.Load<Texture2D>(propName);
                _objectTextureMetersSize = objectTextureMetersSize;
                _objectTextureSize = new Vector2(Texture2D.Width, Texture2D.Height);
                _objectTextureOrigin = new Vector2(_objectTextureSize.X / _columns, _objectTextureSize.Y / _rows);
                _textureHalf = (_objectTextureOrigin / 2f);
            }
        }
        




    }
}