using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CaveWizard.Globals;
using CaveWizard.Levels;
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
        
        public ScreenManager ScreenManager { get; set; }

        public Game1() {
            GlobalDevices._GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);
        }

        protected override void Initialize() {
            base.Initialize();

            _drawables = new List<IDrawable>();
            _updateables = new List<IUpdateable>();
            _world = new World();
            _spriteBatchEffect = new BasicEffect(GraphicsDevice);
            _spriteBatchEffect.TextureEnabled = true;
            //_camera.Zoom = 4f;
            
        }

        protected override void LoadContent() {
            GlobalDevices._SpriteBatch = new SpriteBatch(GraphicsDevice);
            Level level = new Level();

            MenuScreen menuScreen = new MenuScreen("CaveWizard");
            menuScreen.AddMenuItem("Main Menu", EntryType.Separator, null);
            menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("Level", EntryType.Screen, level);
            menuScreen.AddMenuItem("Exit", EntryType.ExitItem, null);
            
            ScreenManager.AddScreen(menuScreen);
            // TODO: use this.Content to load your game content here
        }


    }
}