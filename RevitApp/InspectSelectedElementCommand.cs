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
         StringBuilder data = new StringBuilder();
         Reference selectedObjRef = sel.PickObject(ObjectType.Element, "Select one element");

         Element selectedElement;
         if (selectedObjRef != null) {
            selectedElement = doc.GetElement(selectedObjRef.ElementId);
         } else {
            throw new Exception("Please select one Element");
         }

         ParameterSet selectedElemParams = selectedElement.Parameters;

         data.Append($"Element ID: {selectedElement.Id} \nElement Category Name: {selectedElement.Category.Name}\n\n");

         try {
            foreach (Parameter p in selectedElemParams) {
               data.Append($"Parameter name: {p.Definition.Name}\n" +
                   $"Parameter Type: {p.Definition.ParameterType}\n" +
                   $"Parameter Group: {p.Definition.ParameterGroup}\n" +
                   $"Value: {p.AsValueString()}\n" +
                   $"Is Formula: {p.IsReadOnly}\n");

               if ((doc.GetElement(p.Element.Id).GetType() != typeof(FamilyInstance))) {
                  data.Append($"Parameter ?(instance/type): Type\n\n");
               } else {
                  if ((doc.GetElement(p.Element.Id).GetType() != typeof(FamilySymbol))) {
                     data.Append($"Parameter ?(instance/type): Instance\n\n");
                  }
               }
            }
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"\Users\TDima\Desktop\RevitApp\RevitApp\RevitApp\Resources\out.txt", true)) {
               file.WriteLine(data.ToString());
               file.Flush();
            }
            return Result.Succeeded;

         } catch (Exception e) {
            TaskDialog.Show("Exception caught", e.Message);
            return Result.Failed;
         }
      }
   }
}
