using KeyMacroApp.Common;
using KeyMacroApp.Models;
using Prism.Commands;
using Prism.Mvvm;
using ServiceKeyHookWrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace KeyMacroApp.ViewModels
{
    public class ProfileViewModel : BindableBase
    {
        public ObservableCollection<ProfileBindKey> Profiles { get; set; } = new ObservableCollection<ProfileBindKey>();

        private ProfileBindKey? _selectedProfile = null;
        public ProfileBindKey? SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                SetProperty(ref _selectedProfile, value);
                RaisePropertyChanged(nameof(ProfileName));
                ApplyProfile();
            }
        }
        public string ProfileName { get => _selectedProfile?.Name ?? "<None>"; }

        private DelegateCommand? _addCommand;
        public DelegateCommand AddCommand { get => _addCommand ??= new DelegateCommand(OnAddProfile); }

        private DelegateCommand<object>? _renameCommand = null;
        public DelegateCommand<object> RenameCommand { get => _renameCommand ??= new DelegateCommand<object>(OnRename); }
        
        private DelegateCommand<object>? _deleteCommand = null;
        public DelegateCommand<object> DeleteCommand { get => _deleteCommand ??= new DelegateCommand<object>(OnDelete); }

        private void OnDelete(object param)
        {
            if (param is ProfileBindKey profile)
            {
                profile.Delete();
                Profiles.Remove(profile);
            }
        }

        public ProfileViewModel()
        {
            LoadProfiles();

            SelectedProfile = Profiles.FirstOrDefault();
        }

        private void LoadProfiles()
        {
            try
            {
                Directory.CreateDirectory(@".\Profiles");
                var files = Directory.GetFiles(@".\Profiles", "*.xml");

                foreach (var file in files)
                {
                    ProfileBindKey.Load(file, out var profileBindKey);
                    if (profileBindKey != null)
                    {
                        Profiles.Add(profileBindKey);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Print($"[Error]: ProfileViewModel.LoadProfiles()\n{e}");
            }
        }

        private void ApplyProfile()
        {
            APIWrapper.ClearAllMacro(); // Clear all macro current bind key of old profile
            if (SelectedProfile?.BindKeys != null)
            {
                foreach (var bind in SelectedProfile.BindKeys)
                {
                    var macro = MacroData.FindMacro(bind.MacroId);
                    if (macro != null)
                    {
                        var keyInputs = ConvertHelper.ToListInputManaged(macro.KeyHooks);
                        APIWrapper.BindKey(bind.KeyCode, keyInputs);
                    }
                }
            }
        }

        private void OnAddProfile()
        {
            string GenerateNewName()
            {
                int i = 0;
                string name;
                do
                {
                    i++;
                    name = $"New Profile {i}";
                }
                while (Profiles.FirstOrDefault(p => p.Name == name) != null);   // Check duplicate name
                return name;
            }

            var newProfile = new ProfileBindKey(GenerateNewName());
            newProfile.Save();

            Profiles.Add(newProfile);
        }

        private void OnRename(object param)
        {
            if (param is ProfileBindKey profile)
            {
                SelectedProfile = profile;
                profile.IsInEditMode = true;
            }
        }
    }
}
