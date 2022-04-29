using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelCreationPlugin1
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class CreationModel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            
            List<Level> ListLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();


            Level level1 = ListLevel
                .Where(x => x.Name.Equals("Уровень 1"))
                .FirstOrDefault();

            Level level2 = ListLevel
               .Where(x => x.Name.Equals("Уровень 2"))
               .FirstOrDefault();

            double width = UnitUtils.ConvertToInternalUnits(10000, UnitTypeId.Millimeters);
            double depth = UnitUtils.ConvertToInternalUnits(5000, UnitTypeId.Millimeters);
            double dx = width / 2;
            double dy = depth / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Wall creation");
            transaction.Start();
            for (int i = 0; i < 4; i++)
            {
                Curve curve = Line.CreateBound(points[i], points[i+1]);
                Wall wall = Wall.Create(doc, curve, level1.Id, false);
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }
            
            //CreateWalls(doc, level1);

            AddDoor(doc, level1, walls[0]);
            AddWindow1(doc, level1, walls[1]);
            AddWindow2(doc, level1, walls[2]);
            AddWindow3(doc, level1, walls[3]);
            transaction.Commit();             

            return Result.Succeeded;
        }


        //private void CreateWalls(Document doc, Level level1)
        //{


        //    Wall.Create(doc, curve, level1.Id, true);
        //}


        private void AddDoor(Document doc, Level level1, Wall wall)
        {
            FamilySymbol doorType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Doors)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0915 x 2134 мм"))
                .Where(x => x.FamilyName.Equals("Одиночные-Щитовые"))
                .FirstOrDefault();

            LocationCurve hostCurve = wall.Location as LocationCurve;
            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;

            if (!doorType.IsActive)
            {
                doorType.Activate();
            }

            doc.Create.NewFamilyInstance(point, doorType, wall, level1, StructuralType.NonStructural);

        }





        private void AddWindow1(Document doc, Level level1, Wall wall)
        {
            FamilySymbol windowType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0406 x 1220 мм"))
                .Where(x => x.FamilyName.Equals("Фиксированные"))
                .FirstOrDefault();

            LocationCurve hostCurve = wall.Location as LocationCurve;
            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;

            if (!windowType.IsActive)
            {
                windowType.Activate(); 
            }


            FamilyInstance window = doc.Create.NewFamilyInstance(point, windowType, wall, level1, StructuralType.NonStructural);
            Parameter offset = window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM);
            offset.Set(UnitUtils.ConvertToInternalUnits(1000, UnitTypeId.Millimeters));          

        }


        private void AddWindow2(Document doc, Level level1, Wall wall)
        {
            FamilySymbol windowType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0915 x 1830 мм"))
                .Where(x => x.FamilyName.Equals("Фиксированные"))
                .FirstOrDefault();

            LocationCurve hostCurve = wall.Location as LocationCurve;
            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;

            if (!windowType.IsActive)
            {
                windowType.Activate();
            }


            FamilyInstance window = doc.Create.NewFamilyInstance(point, windowType, wall, level1, StructuralType.NonStructural);
            Parameter offset = window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM);
            offset.Set(UnitUtils.ConvertToInternalUnits(800, UnitTypeId.Millimeters));
        }


        private void AddWindow3(Document doc, Level level1, Wall wall)
        {
            FamilySymbol windowType = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilySymbol))
                .OfCategory(BuiltInCategory.OST_Windows)
                .OfType<FamilySymbol>()
                .Where(x => x.Name.Equals("0915 x 0610 мм"))
                .Where(x => x.FamilyName.Equals("Фиксированные"))
                .FirstOrDefault();

            LocationCurve hostCurve = wall.Location as LocationCurve;
            XYZ point1 = hostCurve.Curve.GetEndPoint(0);
            XYZ point2 = hostCurve.Curve.GetEndPoint(1);
            XYZ point = (point1 + point2) / 2;

            if (!windowType.IsActive)
            {
                windowType.Activate();
            }


            FamilyInstance window = doc.Create.NewFamilyInstance(point, windowType, wall, level1, StructuralType.NonStructural);
            Parameter offset = window.get_Parameter(BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM);
            offset.Set(UnitUtils.ConvertToInternalUnits(1000, UnitTypeId.Millimeters));
        }
    }
}
