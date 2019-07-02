using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApp
{
    [Transaction(TransactionMode.Manual)]
    class InspectSelectedElementCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            Element selectedElement = null;
            string data;
            //BindingMap bindings = doc.ParameterBindings;
            //DefinitionBindingMapIterator it = bindings.ForwardIterator();
            //
            //Dictionary<StorageType, List<ParameterType>> dictStorageTypeList =
            //        new Dictionary<StorageType, List<ParameterType>>();

            Reference selectedObjRef = sel.PickObject(ObjectType.Element, "Select one element");
            sel.Dispose();
            if (selectedObjRef != null)
            {
                selectedElement = doc.GetElement(selectedObjRef.ElementId);
            }

            ParameterSet selectedElemParams = selectedElement.Parameters;

            data = $"Element ID: {selectedElement.Id} \nElement Category Name: {selectedElement.Category.Name}\n\n";

            try
            {
                foreach (var p in selectedElemParams)
                {
                    data += $"Parameter name: {(p as Parameter).Definition.Name}\n" +
                        $"Parameter Type: {(p as Parameter).Definition.ParameterType}\n" +
                        $"Parameter Group: {(p as Parameter).Definition.ParameterGroup}\n" +
                        $"Value: {(p as Parameter).AsDouble()}\n";

                    data += ((p as Parameter).Element is FamilyInstance) ? "Parameter ?(instance/type): Instance\n" : "Parameter Type: Type\n";
                    data += $"Parameter formula: {(p as GlobalParameter).GetFormula()}\n\n";
                    
                    
                    
                }
                TaskDialog.Show("Data", data);
                return Result.Succeeded;
            } catch (Exception e)
            {
                TaskDialog.Show("Exception caught", e.Message);
                return Result.Failed;
            }
        }
    }
}
