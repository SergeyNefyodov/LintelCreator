using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARTools
{
    public class JumperCreatorModel
    {
        public static JumperCreatorViewModel viewModel { get; set; }
        private Element wallType
        {
            get
            { return viewModel.SelectedWallType; }
        }
        private ExternalCommandData commandData { get => viewModel.commandData; }
        private UIApplication uiapp
        {
            get => commandData.Application;
        }
        private UIDocument uidoc
        {
            get => uiapp.ActiveUIDocument;
        }
        private Document doc
        {
            get => uidoc.Document;
        }
        private ElementId viewId
        {
            get => doc.ActiveView.Id;
        }
        internal List<Element> GetWallTypes()
        {
            List<Element> walls = new FilteredElementCollector(doc, viewId)
                .OfClass(typeof(Wall)).ToList();
            List<ElementId> wallTypeIds = new List<ElementId>();
            foreach (Element wall in walls)
            {
                ElementId id = wall.GetTypeId();
                if (!wallTypeIds.Contains(id))
                {
                    wallTypeIds.Add(id);
                }
            }
            List<Element> wallTypes = wallTypeIds.Select(e => doc.GetElement(e)).ToList();
            return wallTypes;
        }

        internal List<Family> GetJumperFamilies()
        {
            List<Family> elems = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(f => f.FamilyCategoryId.IntegerValue == (int)BuiltInCategory.OST_GenericModel)
                .ToList();

            return elems;
        }
        internal List<Element> GetJumperTypes(Family fam)
        {
            return fam.GetFamilySymbolIds().Select(e => doc.GetElement(e)).ToList();
        }
        internal List<Element> GetDoorTypes()
        {
            List<Element> walls = new FilteredElementCollector(doc, viewId)
                .OfClass(typeof(Wall)).Where(w => w.GetTypeId() == wallType.Id).ToList();
            ElementClassFilter ecf = new ElementClassFilter(typeof(FamilyInstance));
            ElementCategoryFilter ecatf = new ElementCategoryFilter(BuiltInCategory.OST_GenericModel, true);
            List<ElementId> doorwindowsids = new List<ElementId>();
            foreach (Element wall in walls)
            {
                IList<ElementId> ids = wall.GetDependentElements(ecf);
                foreach (var id in ids)
                {
                    Element e = doc.GetElement(id);
                    if (ecatf.PassesFilter(e))
                    {
                        ElementId typeId = e.GetTypeId();
                        if (!doorwindowsids.Contains(typeId))
                        {
                            doorwindowsids.Add(typeId);
                        }
                    }
                }
                }
                List<Element> doorTypes = new List<Element>();
                foreach (ElementId id in doorwindowsids)
                {
                    Element e = doc.GetElement(id);
                    doorTypes.Add(e);
                }
                return doorTypes;
            }
            internal void MakeJumpers()
            {
                using (Transaction t = new Transaction(doc, "Создание перемычек"))
                {
                    t.Start();

                    foreach (JumperMapperViewModel mapper in viewModel.Mappers)
                    {
                        if (!mapper.IsChecked || mapper.SelectedJumperType == null)
                            continue;

                        List<Element> doors = new FilteredElementCollector(doc, viewId)
                            .OfClass(typeof(FamilyInstance))
                            .Where(e => e.GetTypeId() == mapper.DoorType.Id)
                            .Where(e => (e as FamilyInstance).Host.GetTypeId() == wallType.Id)
                            .ToList();
                        FamilySymbol jumperType = mapper.SelectedJumperType as FamilySymbol;
                        jumperType?.Activate();

                        foreach (Element door in doors)
                        {
                            double d = door.get_Parameter(BuiltInParameter.GENERIC_HEIGHT).AsDouble();
                            LocationPoint lp = door.Location as LocationPoint;
                            XYZ xyz = lp.Point;
                            Wall w = (door as FamilyInstance).Host as Wall;
                            Line l = (w.Location as LocationCurve).Curve as Line;
                            XYZ xyz2 = l.Project(xyz).XYZPoint;
                            FamilyInstance fi = doc.Create.NewFamilyInstance(xyz2, jumperType, w, doc.GetElement(w.LevelId) as Level, StructuralType.NonStructural);
                            fi.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM)?.Set(d);
                        }
                    }
                    t.Commit();
                }
            }
        }
    }
