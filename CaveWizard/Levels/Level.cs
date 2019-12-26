using System.Collections.Generic;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Game;
using CaveWizard.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CaveWizard.Levels
{
    public class Level : PhysicsGameScreen, IGameLevel
    {
        private Player _player;
        private List<Ground> _ground;
        private List<Enemy> _enemies;
        
        public Level()
        {
            
        }
        
        public override void LoadContent()
        {
            base.LoadContent();

            World.Gravity = new Vector2(0, -10f);
            
            _player = new Player(ScreenManager, "Textures/blackMageConcept", new Vector2(0f, 2.5f), World, 6, 3);
            _ground = new List<Ground>();
            _ground.Add(new Ground(ScreenManager, "Textures/ground", new Vector2(0f, 0f), World, 1, 1));
            _enemies = new List<Enemy>();            
            
            Camera.TrackingBody = _player.ObjectBody;
            Camera.Zoom = 4f;
            SetUserAgent(_player.ObjectBody, 2, 0);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.BatchEffect.View = Camera.View;
            ScreenManager.BatchEffect.Projection = Camera.Projection;
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null,
                RasterizerState.CullNone, ScreenManager.BatchEffect);
            _player.Draw(gameTime);
            foreach (var ground in _ground)
            {
                ground.Draw(gameTime);
            }
            ScreenManager.SpriteBatch.End();
            
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, _player.InAir.ToString(), Vector2.One, Color.White);
            ScreenManager.SpriteBatch.End();
            
            
            base.Draw(gameTime);
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {

            _player.HandleInput(input, gameTime);

            
            
            base.HandleInput(input, gameTime);
        }

        protected override void HandleCursor(InputHelper input)
        {
            base.HandleCursor(input);

            _player.HandleCursor(input, World, Camera);
            if (input.IsNewMouseButtonPress(MouseButtons.RightButton))
            {
                _ground.Add(new Ground(ScreenManager, "Textures/ground", Camera.ConvertScreenToWorld(input.Cursor), World, 1, 1));

            }

            if (input.IsNewMouseButtonPress(MouseButtons.MiddleButton))
            {
                _enemies.Add(new Enemy(ScreenManager, _player, null,Camera.ConvertScreenToWorld(input.Cursor), World, new Vector2(1.5f, 0.5f), new Vector2(1.5f, 1.5f), 1, 1 ));
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            _player.Update(gameTime);

            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
            }
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }
    }
}