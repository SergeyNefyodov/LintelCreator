using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace ARTools
{
    public class JumperCreatorViewModel : INotifyPropertyChanged
    {
        internal ExternalCommandData commandData;
        public JumperCreatorViewModel(ExternalCommandData cData)
        {
            commandData = cData;
            model = new JumperCreatorModel();
            JumperCreatorModel.viewModel = this;
            JumperMapperViewModel.viewModel = this;
            JumperMapperViewModel.model = model;
            FillView = new RelayCommand(fillViewExecute, canFillViewExecuted);
            ClearView = new RelayCommand(clearViewExecute, canClearViewExecuted);
            CreateJumpers = new RelayCommand(createJumpersExecute, canCreateJumpersExecuted);
        }
        private ObservableCollection<JumperMapperViewModel> mappers = new ObservableCollection<JumperMapperViewModel>();
        public ObservableCollection<JumperMapperViewModel> Mappers
        {
            get => mappers;
            set
            {
                mappers = value;
                OnPropertyChanged();
            }
        }
        private List<ElementId> familyTypesIds;
        public List<ElementId> FamilyTypesIds
        {
            get => familyTypesIds;
            set
            {
                familyTypesIds = value;
                OnPropertyChanged();
            }
        }
        public List<Element> WallTypes
        {
            get
            {
                return model.GetWallTypes();
            }            
        }
        private Element selectedWallType;
        public Element SelectedWallType
        {
            get => selectedWallType;
            set
            {
                selectedWallType = value;
                OnPropertyChanged();
            }
        }
        private bool areAllSelected = false;
        public bool AreAllSelected
        {
            get => areAllSelected;
            set
            {
                areAllSelected = value;
                foreach (var item in mappers)
                {
                    item.IsChecked = value;
                }
                OnPropertyChanged();
            }
        }
        public static ICommand FillView { get; set; }
        private void fillViewExecute(object p)
        {
            var doorTypes = model.GetDoorTypes();
            foreach (var doorType in doorTypes)
            {
                JumperMapperViewModel mapper = new JumperMapperViewModel(doorType);
                var v = JumperMapperViewModel.JumperFamilies;
                mappers.Add(mapper);
            }
        }
        public bool canFillViewExecuted(object p)
        {
            return SelectedWallType != null && Mappers.Count==0 ? true : false;
        }
        public static ICommand ClearView { get; set; }
        private void clearViewExecute(object p)
        {
            mappers.Clear();
        }
        private bool canClearViewExecuted(object p)
        {
            return mappers.Count == 0 ? false : true;
        }

        public static ICommand CreateJumpers { get; set; }
        private void createJumpersExecute(object p)
        {
            model.MakeJumpers();
            RaiseHideRequest();
        }
        private bool  canCreateJumpersExecuted(object p)
        {            
            for (int i = 0; i < mappers.Count; i++)
            {
                if (mappers[i].IsChecked)
                {
                    return true;
                }
            }
            return false;
        }
        internal JumperCreatorModel model { get; set; }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
