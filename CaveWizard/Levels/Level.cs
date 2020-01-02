using System;
using System.Collections.Generic;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Game;
using CaveWizard.Globals;
using CaveWizard.Menus;
using LevelGenerator;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CaveWizard.Levels
{
    public class Level : PhysicsGameScreen, IGameLevel, IMenubale
    {
        private Player _player;
        private List<Ground> _ground;
        private List<Enemy> _enemies;
        private Generator _generator;
        private Menu _menu;
        public Level()
        {
            TransitionOffTime = TimeSpan.Zero;
            TransitionOnTime = TimeSpan.Zero;
        }
        
        public override void LoadContent()
        {
            base.LoadContent();
            GlobalDevices._GameState = GameState.PLAY;
            World.Gravity = new Vector2(0, -10f);
            _generator = new Generator(6);
           _ground = new List<Ground>();
           for (int i = 0; i < _generator.LevelPresented.Count; i++)
           {
               int holesEncountered = 0;
               for (int j = 0; j < _generator.LevelPresented[i].Count; j++)
               {
                   char currElement = _generator.LevelPresented[i][j];
                   if (currElement == '.')
                   {
                       holesEncountered++;
                   }
                   if (currElement == 'S')
                   {
                       _player = new Player(ScreenManager, "Textures/blackMageConcept", new Vector2((float)-j, (float)-i), World, 6, 3);
                   }
                   else if (currElement == '#')
                   {
                       _ground.Add(new Ground(ScreenManager, "Textures/ground", new Vector2((float)-(j ), (float)-(i + (i/2))), World, 3, 1, _generator.LevelPresented, i, j));

                   }
               }
            }
            
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

            foreach (var enemy in _enemies)
            {
                enemy.Draw(gameTime);
            }
            ScreenManager.SpriteBatch.End();
            
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Fonts.DetailsFont, _player.InAir.ToString(), Vector2.One, Color.White);
            ScreenManager.SpriteBatch.End();
            
            
            base.Draw(gameTime);
        }

        public override void HandleInput(InputHelper input, GameTime gameTime)
        {

            if (input.IsNewKeyPress(KeyBinds.GoToMenu))
            {
                ScreenManager.AddScreen(new Menu("Menu", this));
            }

            if (input.IsNewKeyPress(Keys.U))
            {
                LoadContent();
            }
            
            _player.HandleInput(input, gameTime);

            
            
            base.HandleInput(input, gameTime);
        }

        protected override void HandleCursor(InputHelper input)
        {
            base.HandleCursor(input);

            _player.HandleCursor(input, World, Camera);
            if (input.IsNewMouseButtonPress(MouseButtons.RightButton))
            {
                _ground.Add(new Ground(ScreenManager, "Textures/ground", Camera.ConvertScreenToWorld(input.Cursor), World, 3, 1, null, 0, 0));

            }

            if (input.IsNewMouseButtonPress(MouseButtons.MiddleButton))
            {
                _enemies.Add(new Enemy(ScreenManager, _player, "Textures/skeleton",Camera.ConvertScreenToWorld(input.Cursor), World, new Vector2(1f, 0.5f),  new Vector2(1f, 1f), 10, 5 ));
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (GlobalDevices._GameState == GameState.PLAY)
            {
                _player.Update(gameTime);

                foreach (var enemy in _enemies)
                {
                    enemy.Update(gameTime);
                }
            }
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public void MenuDestroyed()
        {
            GlobalDevices._GameState = GameState.PLAY;
        }

        public void ToMainMenu()
        {
            GlobalDevices._GameState = GameState.MAINMENU;
            ExitScreen();
            
        }
    }
}