using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CaveWizard.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CaveWizard.Globals;
namespace CaveWizard {
    public class Game1 : Microsoft.Xna.Framework.Game {
        private List<IDrawable> _drawables;
        private List<IUpdateable> _updateables;
        private Player _player;
        public Game1() {
            GlobalDevices._GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {

            _drawables = new List<IDrawable>();
            _updateables = new List<IUpdateable>();
            _player = new Player(Content, "Textures/1", new Vector2(0f, 0f));
            base.Initialize();
        }

        protected override void LoadContent() {
            GlobalDevices._SpriteBatch = new SpriteBatch(GraphicsDevice);
            _drawables.Add(_player);
            _updateables.Add(_player);
            
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            GlobalDevices._MouseState = Mouse.GetState();
            foreach (IUpdateable updateable in _updateables) {
                updateable.Update(gameTime);
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GlobalDevices._SpriteBatch.Begin();
            foreach (IDrawable drawable in _drawables) {
                drawable.Draw(gameTime);
            }
            GlobalDevices._SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}