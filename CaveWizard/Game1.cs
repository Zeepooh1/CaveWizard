using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CaveEngine.Utils;
using CaveWizard.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CaveWizard.Globals;
using tainicom.Aether.Physics2D.Dynamics;

namespace CaveWizard {
    public class Game1 : Microsoft.Xna.Framework.Game {
        private List<IDrawable> _drawables;
        private List<IUpdateable> _updateables;
        private Player _player;
        private Ground _ground;
        private Ground _ground2;
        private List<Ground> _grounds;
        private World _world;
        private BasicEffect _spriteBatchEffect;
        private Camera2D _camera;

        public Game1() {
            GlobalDevices._GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {

            _drawables = new List<IDrawable>();
            _updateables = new List<IUpdateable>();
            _world = new World();
            _spriteBatchEffect = new BasicEffect(GraphicsDevice);
            _spriteBatchEffect.TextureEnabled = true;
            _player = new Player(Content, "Textures/1", new Vector2(0f, 2.5f), _world);
            _camera = new Camera2D(GraphicsDevice);
            _camera.TrackingBody = _player.PlayerBody;
            //_camera.Zoom = 4f;

            _ground = new Ground(Content, "Textures/ground", new Vector2(0f, 0f), _world);
            _ground2 = new Ground(Content, "Textures/ground", new Vector2(6f, 0f), _world);
            _grounds = new List<Ground>();
            base.Initialize();
        }

        protected override void LoadContent() {
            GlobalDevices._SpriteBatch = new SpriteBatch(GraphicsDevice);
            _drawables.Add(_player);
            _updateables.Add(_player);
            _drawables.Add(_ground);
            _updateables.Add(_ground);
            _drawables.Add(_ground2);
            _updateables.Add(_ground2);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            InputManager.KeyState = Keyboard.GetState();
            InputManager.MouseState = Mouse.GetState();
            foreach (IUpdateable updateable in _updateables) {
                updateable.Update(gameTime);
            }
            // TODO: Add your update logic here
            _world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            _camera.Update(gameTime);
            if (InputManager.IsLeftMouseButtonPressed())
            {
//                Vector2 Cursor = InputManager.MouseState.Position.ToVector2();
//                Cursor = new Vector2(MathHelper.Clamp(Cursor.X, 0f, GraphicsDevice.Viewport.Width), MathHelper.Clamp(Cursor.Y, 0f, GraphicsDevice.Viewport.Height));
                Ground ground = new Ground(Content, "Textures/ground", new Vector2(_grounds.Count * 6f + 12f, 0f), _world);
                _grounds.Add(ground);
                _drawables.Add(ground);
                _updateables.Add(ground);
            }

            if (InputManager.IsRightMouseButtonPressed())
            {
                _ground2.RemoveFromWorld(_world);
                _drawables.Remove(_ground2);
                _updateables.Remove(_ground2);
            }
            InputManager.PrevKeyState = InputManager.KeyState;
            InputManager.PrevMouseState = InputManager.MouseState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            var vp = GraphicsDevice.Viewport;
            _spriteBatchEffect.View = _camera.View;
            _spriteBatchEffect.Projection = _camera.Projection;


            GlobalDevices._SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, RasterizerState.CullClockwise, _spriteBatchEffect);
            foreach (IDrawable drawable in _drawables) {
                drawable.Draw(gameTime);
            }
            GlobalDevices._SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}