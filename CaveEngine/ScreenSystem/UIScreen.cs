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

        public void AddUIElement(UIElement element)
        {
            _uiElements.Add(element);
        }
        
        public void Initialize()
        {
            
        }
        public void LoadContent(ContentManager content)
        {
            _grayScaleEffect = content.Load<Effect>("Shaders/grayScale");
        }

        public void Update(GameTime gameTime)
        {
            foreach (var uiElement in _uiElements)
            {
                uiElement.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            //_grayScaleEffect.CurrentTechnique.Passes[0].Apply();
            foreach (var uiElement in _uiElements)
            {
                uiElement.Draw(gameTime);
            }
          
        }

        public int DrawOrder { get; }
        public bool Visible { get; }
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}