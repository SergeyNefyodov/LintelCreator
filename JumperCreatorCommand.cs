using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTools
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class JumperCreatorCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            JumperCreatorView view = new JumperCreatorView();
            JumperCreatorViewModel ViewModel = new JumperCreatorViewModel(commandData);
            JumperCreatorModel.viewModel = ViewModel;
            view.commandData = commandData;
            view.DataContext = ViewModel;
            ViewModel.CloseRequest += (s, e) => view.Close();
            ViewModel.HideRequest += (s, e) => view.Hide();
            ViewModel.ShowRequest += (s, e) => view.ShowDialog();
            view.ShowDialog();
            return Result.Succeeded;
        }
    }
}
