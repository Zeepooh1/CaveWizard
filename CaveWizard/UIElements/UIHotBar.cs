using CaveEngine.DrawingSystem;
using CaveEngine.ScreenSystem;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaveWizard.UIElements
{
    public class UIHotBar : UIElement
    {
        public UIHotBar()
        {
            Vector2 pos = new Vector2(GlobalDevices._GraphicsDeviceManager.GraphicsDevice.Viewport.Width, 0);
            Texture = GlobalDevices._ScreenManager.Content.Load<Texture2D>("Textures/ItemToolBar");
            Vector2 offset = new Vector2(Texture.Width, Texture.Height) / 2;
            float scale = 3f;
            TextureOrigin = offset;
            Position = pos + (offset * scale * (new Vector2(-1 , 1)));
            TextureScale = scale;
        }


        public override void Update(GameTime gameTime)
        {
            
        }

        public override void Draw(GameTime gameTime)
        {
            GlobalDevices._ScreenManager.SpriteBatch.Draw(Texture, Position, null, Color.White, 0f, TextureOrigin, 
                TextureScale, SpriteEffects.None, 0f);
        }
    }
}