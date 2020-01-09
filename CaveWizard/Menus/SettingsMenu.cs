using System;
using CaveEngine.ScreenSystem;
using CaveWizard.Globals;

namespace CaveWizard.Menus
{
    public class SettingsMenu : MenuScreen
    {
        private bool _firstLoad;
        public SettingsMenu(string menuTitle) : base(menuTitle)
        {
            _firstLoad = true;
        }
        public override void LoadContent()
        {
            if (_firstLoad)
            {
                AddMenuItem("", EntryType.Separator, null);
                AddMenuItem("", EntryType.Separator, null);
                AddMenuItem("", EntryType.Separator, null);
                AddMenuItem("", EntryType.Separator, null);
                MenuButton button = AddMenuButton(ButtonType.CheckBox, "Textures/Checkbox", "Volume", GameSettings._Volume);
                MenuButton fullScreenButton = AddMenuButton(ButtonType.CheckBox, "Textures/CheckBox", "Full screen",
                    GameSettings._FullScreen);
                AddMenuItem("", EntryType.Separator, null);
                AddMenuItem("", EntryType.Separator, null);
                AddMenuItem("Back", EntryType.Back, null);
                button.ClickedOn += VolumeCheckBoxClicked;
                fullScreenButton.ClickedOn += FullScreenCheckBoxClicked;
                _firstLoad = false;
            }

            base.LoadContent();
        }

        private void FullScreenCheckBoxClicked(object sender, EventArgs e)
        {
            MenuCheckBox buttonSender = (MenuCheckBox) sender;
            buttonSender.Change();
            GameSettings._FullScreen = buttonSender.IsChecked;
            GameSettings.SaveSettings();
            GlobalDevices._GraphicsDeviceManager.IsFullScreen = GameSettings._FullScreen;
            GlobalDevices._GraphicsDeviceManager.ApplyChanges();
        }


        private void VolumeCheckBoxClicked(object sender, EventArgs e)
        {
            MenuCheckBox buttonSender = (MenuCheckBox) sender;
            buttonSender.Change();
            GameSettings._Volume = buttonSender.IsChecked;
            GameSettings.SaveSettings();
        }
    }
}