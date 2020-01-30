using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CaveEngine.DrawingSystem
{
    public abstract class UIElement: IUpdateable, IDrawable
    {

        public bool Enabled { get; }
        public int UpdateOrder { get; }    
        public int DrawOrder { get; }
        public bool Visible { get; }
        
        public Texture2D Texture;
        public Vector2 Position;
        public float TextureScale;
        public Vector2 TextureOrigin;
        public bool Shown;

        public abstract void Update(GameTime gameTime);

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public abstract void Draw(GameTime gameTime);


        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
    
    
}