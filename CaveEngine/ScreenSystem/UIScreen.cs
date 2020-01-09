using System;
using System.Collections.Generic;
using CaveEngine.DrawingSystem;
using CaveEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using tainicom.Aether.Physics2D.Dynamics;

namespace CaveEngine.ScreenSystem
{
    public class UIScreen : IDrawable
    {
        private List<UIElement> _uiElements;
        private Effect _grayScaleEffect;

        private GraphicsDeviceManager _graphicsDeviceManager;
        private ScreenManager _screenManager;
        public UIScreen(GraphicsDeviceManager graphicsDeviceManager, ScreenManager screenManager)
        {
            _graphicsDeviceManager = graphicsDeviceManager;
            _screenManager = screenManager;
            _uiElements = new List<UIElement>();
        }

        public void AddUIElement(string texName, Vector2 pos, float scale)
        {
            UIElement element = new UIElement();
            element.Texture = _screenManager.Content.Load<Texture2D>(texName);
            Vector2 offset = new Vector2(element.Texture.Width, element.Texture.Height) / 2;
            element.TextureOrigin = offset;
            element.Position = pos + (offset * scale * (new Vector2(-1 , 1)));
            element.TextureScale = scale;
            _uiElements.Add(element);
        }
        
        public void Initialize()
        {
            
        }
        public void LoadContent(ContentManager content)
        {
            _grayScaleEffect = content.Load<Effect>("Shaders/grayScale");
        }

        public void Update()
        {
        }

        public void Draw(GameTime gameTime)
        {
            //_grayScaleEffect.CurrentTechnique.Passes[0].Apply();
            foreach (var uiElement in _uiElements)
            {
                _screenManager.SpriteBatch.Draw(uiElement.Texture, uiElement.Position, null, Color.White, 0f, uiElement.TextureOrigin, 
                    uiElement.TextureScale, SpriteEffects.None, 0f);
            }
          
        }

        public int DrawOrder { get; }
        public bool Visible { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}