using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Mvvm.ComponentModel;

using FakePOS.Services;
using FakePOS.Messages;
using System.Collections.Generic;

namespace FakePOS.ViewModels
{
    public class ThemeType
    {
        public enum ThemeValue
        {
            Light,
            Dark,
            HighContrast
        }

        private string _name;
        private ThemeValue _themeValue;

        public string Name
        {
            get => _name;
            set { _name = Name; }
        }

        public ThemeValue Value
        {
            get => _themeValue;
            set { _themeValue = Value; }
        }
    }

    [RegisterWithIoc(InstanceMode.Transient)]
    public class SettingsViewModel : ObservableRecipient
    {
        private readonly ISettingsService _settingsService;
        private readonly IMessenger _messenger;

        public List<string> Themes = new List<string>
        {
            "Light","Dark","Windows Default"
        };

        private int _selectedIndex = 2;
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                SetProperty(ref _selectedIndex, value);

                if (_selectedIndex.Equals(0))
                {
                }
                else if (_selectedIndex.Equals(1))
                {
                }
                else if (_selectedIndex.Equals(2))
                {
                }
            }
        }

        private bool _showVersion;
        public bool ShowVersion
        {
            get => _showVersion;
            set
            {
                if (SetProperty(ref _showVersion, value))
                {
                    _settingsService.SetValue(SettingsKeys.ShowVersionInfo, value);

                    if (value)
                        _messenger.Send(new ShellStateMessage(ShellState.VersionOn));
                    else
                        _messenger.Send(new ShellStateMessage(ShellState.VersionOff));
                }
            }
        }

        public SettingsViewModel(ISettingsService settingsService, IMessenger messenger)
        {
            _settingsService = settingsService;
            _messenger = messenger;

            _showVersion = _settingsService.GetValue<bool>(SettingsKeys.ShowVersionInfo);
        }
    }
}
