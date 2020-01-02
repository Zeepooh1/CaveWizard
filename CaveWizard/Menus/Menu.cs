using System;
using CaveEngine.ScreenSystem;
using CaveEngine.Utils;
using CaveWizard.Globals;

namespace CaveWizard.Menus
{
    public class Menu : MenuScreen
    {
        private IMenubale _parentScreen;
        public Menu(string menuTitle, IMenubale parentScreen) : base(menuTitle)
        {
            _parentScreen = parentScreen;

            GlobalDevices._GameState = GameState.MENU;
            AddMenuItem("", EntryType.Separator, null);
            AddMenuItem("", EntryType.Separator, null);
            AddMenuItem("", EntryType.Separator, null);
            AddMenuItem("", EntryType.Separator, null);
            AddMenuItem("Resume", EntryType.Back, (GameScreen)_parentScreen );
            AddMenuItem("Return to Main Menu", EntryType.ToMainMenu, (GameScreen) _parentScreen);
            TransitionOffTime = TimeSpan.Zero;
            TransitionOnTime = TimeSpan.Zero;
        }

        ~Menu()
        {
                _parentScreen.MenuDestroyed();
        }
    }
}