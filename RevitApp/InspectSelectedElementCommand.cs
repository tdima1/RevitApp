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
         const string htmlMarginLeft = "\"margin-left: 30%\"";
         const string htmlPosition = "position=\"relative\"";
         const string htmlWidth = "width= \"5px\"";
         const string htmlAlign = "align:\"left\"";

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
                  data.Append(
                      $"<span style={htmlWidth} {htmlAlign}>Parameter name: </span> <span style={htmlMarginLeft} {htmlPosition}>{p.Definition.Name}</span></br>\n" +
                      $"<span style={htmlWidth} {htmlAlign}>Parameter Type: </span> <span style={htmlMarginLeft} {htmlPosition}>{p.Definition.ParameterType}</span></br>\n" +
                      $"<span style={htmlWidth} {htmlAlign}>Parameter Group: </span> <span style={htmlMarginLeft} {htmlPosition}>{p.Definition.ParameterGroup}</span></br>\n" +
                      $"<span style={htmlWidth} {htmlAlign}>Value: </span> <span style={htmlMarginLeft} {htmlPosition}>{p.AsValueString()}</span></br>\n" +
                      $"<span style={htmlWidth} {htmlAlign}>Is Formula: </span> <span style={htmlMarginLeft} {htmlPosition}>{p.IsReadOnly}</span></br>\n");

                  if ((doc.GetElement(p.Element.Id).GetType() != typeof(FamilyInstance))) {
                     data.Append($"<span style={htmlWidth} {htmlAlign}>Parameter ?(instance/type): </span><span style={htmlMarginLeft} {htmlPosition}>Type</span></br></br>\n\n");
                  } else {
                     if ((doc.GetElement(p.Element.Id).GetType() != typeof(FamilySymbol))) {
                        data.Append($"<span style={htmlWidth} {htmlAlign}>Parameter ?(instance/type): </span><span style={htmlMarginLeft} {htmlPosition}>Instance</span></br></br>\n\n");
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
