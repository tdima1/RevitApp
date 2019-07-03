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

         data.Append($"Element ID: {selectedElement.Id} </br>\nElement Category Name: {selectedElement.Category.Name}</br></br>\n");
         data.Append("<style> span { position: absolute; margin-left: 15%; } </style>");

         try {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"\Users\TDima\Desktop\RevitApp\RevitApp\RevitApp\Resources\out.htm", true)) {
               file.WriteLine($"<h1><center> {data.ToString()} </center></h1></br></br>\n\n");
               data = new StringBuilder();

               foreach (Parameter p in selectedElemParams) {
                  data.Append($"Parameter name: <span style=\"margin-left: 15%\" position=\"absolute\">{p.Definition.Name}</span></br>\n" +
                      $"Parameter Type: <span style=\"margin-left: 15%\" position=\"absolute\">{p.Definition.ParameterType}</span></br>\n" +
                      $"Parameter Group: <span style=\"margin-left: 15%\" position=\"absolute\">{p.Definition.ParameterGroup}</span></br>\n" +
                      $"Value: <span>{p.AsValueString()}</span></br>\n" +
                      $"Is Formula: <span>{p.IsReadOnly}</span></br>\n");

                  if ((doc.GetElement(p.Element.Id).GetType() != typeof(FamilyInstance))) {
                     data.Append($"Parameter ?(instance/type): <span style=\"margin-left: 15%\">Type</span></br></br>\n\n");
                  } else {
                     if ((doc.GetElement(p.Element.Id).GetType() != typeof(FamilySymbol))) {
                        data.Append($"Parameter ?(instance/type): <span style=\"margin-left: 15%\">Instance</span></br></br>\n\n");
                     }
                  }
               }

               file.WriteLine($"<p> {data.ToString()} </p></br>\n");
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
