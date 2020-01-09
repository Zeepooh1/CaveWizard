using System;
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
using CaveWizard.Menus;
using IniParser;
using IniParser.Exceptions;
using IniParser.Model;
using Microsoft.Xna.Framework.Audio;
using tainicom.Aether.Physics2D.Dynamics;

namespace CaveWizard {
    public class Game1 : Microsoft.Xna.Framework.Game {
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
            
            GlobalDevices._GameState = GameState.MAINMENU;
            GameSettings.InitParser();
            GlobalDevices._GraphicsDeviceManager.IsFullScreen = GameSettings._FullScreen;
            GlobalDevices._GraphicsDeviceManager.ApplyChanges();
            SoundEffects.LoadSoundEffects(ScreenManager.Content);
            Level level = new Level();
            MenuScreen menuScreen = new MenuScreen("CaveWizard");

            menuScreen.AddMenuItem("Main Menu", EntryType.Separator, null);
            menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("", EntryType.Separator, null);
            menuScreen.AddMenuItem("Level", EntryType.Screen, level);
            menuScreen.AddMenuItem("Settings", EntryType.Screen, new SettingsMenu("Settings"));
         //   menuScreen.AddMenuButton(ButtonType.CheckBox, "Textures/Checkbox", "Test");
            menuScreen.AddMenuItem("Exit", EntryType.ExitItem, null);
            ScreenManager.AddScreen(menuScreen);

            //_camera.Zoom = 4f;
            
        }
        
    }
}