using CaveEngine.ScreenSystem;
using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;

namespace CaveEngine.WorldSystem
{
    public abstract class WorldObject
    {
        public Body ObjectBody { get; set; }
        protected Vector2 _objectBodySize;
        protected Vector2 _objectTextureOrigin;
        protected ScreenManager _screenManager;
        protected int _columns;
        protected int _rows;
        protected int _currColumn;
        protected int _currRow;
        protected Vector2 _textureHalf;
        protected PhysicsGameScreen SourceLevel;

        public WorldObject(ScreenManager screenManager, Vector2 objectBodySize, int columns, int rows, PhysicsGameScreen sourceLevel)
        {
            _screenManager = screenManager;
            _objectBodySize = objectBodySize;
            _columns = columns;
            _rows = rows;
            SourceLevel = sourceLevel;
            _currColumn = 0;
            _currRow = 0;
         
        }
        




    }
}