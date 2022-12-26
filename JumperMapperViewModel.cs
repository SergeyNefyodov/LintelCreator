using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ARTools
{
    public class JumperMapperViewModel : INotifyPropertyChanged
    {
        public JumperMapperViewModel(Element doorType)
        {
            DoorType = doorType;
        }
        private bool isChecked = false;
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged();
            }
        }
        
        public Element DoorType
        {
            get;
        }
        private Family selectedJumperFamily;
        public Family SelectedJumperFamily
        {
            get => selectedJumperFamily;
            set
            {
                selectedJumperFamily = value;
                JumperTypes = model.GetJumperTypes(SelectedJumperFamily);
                OnPropertyChanged();
            }
        }
        private Element selectedJumper;
        public Element SelectedJumperType
        {
            get => selectedJumper;
            set
            {
                selectedJumper = value;
                OnPropertyChanged();
                //var v = JumperTypes;
            }
        }
        public static List<Family> JumperFamilies
        {
            get
            {
                return model.GetJumperFamilies();                
            }
        }
        private List<Element> jumperTypes;
        public List<Element> JumperTypes
        {
            get
            {
                return model.GetJumperTypes(SelectedJumperFamily);
            }
            set
            {
                jumperTypes = value;
                OnPropertyChanged();
            }
        }
        public static JumperCreatorModel model;
        public static JumperCreatorViewModel viewModel;
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
