using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
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
         StringBuilder data;
         //BindingMap bindings = doc.ParameterBindings;
         //DefinitionBindingMapIterator it = bindings.ForwardIterator();
         //
         //Dictionary<StorageType, List<ParameterType>> dictStorageTypeList =
         //        new Dictionary<StorageType, List<ParameterType>>();

         Reference selectedObjRef = sel.PickObject(ObjectType.Element, "Select one element");
         sel.Dispose();

         if (selectedObjRef != null) {
            selectedElement = doc.GetElement(selectedObjRef.ElementId);
         }

         ParameterSet selectedElemParams = selectedElement.Parameters;

         data = $"Element ID: {selectedElement.Id} \nElement Category Name: {selectedElement.Category.Name}\n\n";

         //while (it.MoveNext()) {
         //   Definition d = it.Key as Definition;
         //   Binding b = it.Current as Binding;
         //
         //   string sbinding = (b is InstanceBinding) ? "instance" : "type";
         //   data += $"{d.Name} {sbinding}\n\n";
         //}
         //TaskDialog.Show("Data", data);
         //return Result.Succeeded;

         try {
            foreach (var p in selectedElemParams) {
               data += $"Parameter name: {(p as Parameter).Definition.Name}\n" +
                   $"Parameter Type: {(p as Parameter).Definition.ParameterType}\n" +
                   $"Parameter Group: {(p as Parameter).Definition.ParameterGroup}\n" +
                   $"Value: {(p as Parameter).AsDouble()}\n\n";

               //data += (typeof(ElementType) != (p as Parameter).Element.GetType()) ? "Parameter ?(instance/type): Instance\n\n" : "Parameter Type: Type\n\n";
               //data += $"Parameter formula: {(p as FamilyParameter).Formula}\n\n";

            }
         
            TaskDialog.Show("Data", data);
            return Result.Succeeded;
         
         } catch (Exception e) {
            TaskDialog.Show("Exception caught", e.Message);
            return Result.Failed;
         }
      }
   }
}
