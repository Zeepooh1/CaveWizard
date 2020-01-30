using CaveEngine.DrawingSystem;
using CaveWizard.Game;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Collision.Shapes;
using tainicom.Aether.Physics2D.Common;
using tainicom.Aether.Physics2D.Dynamics;
using tainicom.Aether.Physics2D.Dynamics.Contacts;

namespace CaveWizard.UIElements
{
    public class UIHealthBar : UIElement
    {
        private Rectangle _sourceRectangle;
        private Player _player;
        private Color _healthBarColor;
        public UIHealthBar(Player player)
        {
            _player = player;
            Vertices verts = new Vertices();
            verts.Add(new Vector2(5f * (_player.Health / 100f), 1f));
            verts.Add(new Vector2(5f * (_player.Health / 100f), 0f));
            verts.Add(new Vector2(0f, 1f));
            verts.Add(new Vector2(0f, 0f));

            Shape shape = new PolygonShape(verts, 0f);
            Vector2 pos = new Vector2(0, GlobalDevices._GraphicsDeviceManager.GraphicsDevice.Viewport.Height);
            Texture = GlobalDevices._ScreenManager.Assets.TextureFromShape(shape, MaterialType.Squares, Color.White, 0.001f);
                   
            _sourceRectangle =  new Rectangle(0, 0, Texture.Width * (_player.Health / 100), Texture.Height);

            Vector2 offset = new Vector2(Texture.Width, Texture.Height) / 2;
            float scale = 1f;
            TextureOrigin = offset;
            Position = pos + (offset * scale * (new Vector2(1 , -1)));
            TextureScale = scale;
            _healthBarColor = Color.Green;
            _player.ObjectBody.OnCollision += PlayerObjectBodyOnOnCollision;
        }

        private bool PlayerObjectBodyOnOnCollision(Fixture sender, Fixture other, Contact contact)
        {
            if ("MagicMissile".Equals(other.Tag))
            {
                _player.GotHit();
                _sourceRectangle =  new Rectangle(0, 0, (int)(Texture.Width * ((_player.Health - 25 )  / 100f)), Texture.Height);

            }

            return true;
        }

        public override void Update(GameTime gameTime)
        {
            if (_player.Health <= 50)
            {
                _healthBarColor = Color.Yellow;
            }

            if (_player.Health <= 25)
            {
                _healthBarColor = Color.Red;

            }
        }

        public override void Draw(GameTime gameTime)
        {
            GlobalDevices._ScreenManager.SpriteBatch.Draw(Texture, Position, _sourceRectangle, _healthBarColor, 0f, TextureOrigin, 
                TextureScale, SpriteEffects.None, 0f);
        }
    }
}