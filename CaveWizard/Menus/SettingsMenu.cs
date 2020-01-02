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
                AddMenuItem("", EntryType.Separator, null);
                AddMenuItem("", EntryType.Separator, null);
                AddMenuItem("Back", EntryType.Back, null);
                button.ClickedOn += VolumeCheckBoxClicked;
                _firstLoad = false;
            }

            base.LoadContent();
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