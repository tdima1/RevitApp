using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitApp
{
    class Main : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            RibbonPanel panel = application.CreateRibbonPanel("Revit App");
            PushButton button = panel.AddItem(new PushButtonData($"Lab 1",
            "Inspect Selected Element", @"RevitApp.dll", $"RevitApp.InspectSelectedElementCommand")) as PushButton;
            panel.AddSeparator();
            return Result.Succeeded;
        }
    }
}
