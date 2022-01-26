using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Tsp.Ui
{
    public static class CanvasExtensions
    {
        public static void RemoveElementsById(this Canvas canvas, string uId)
        {
            var itemsToRemove = canvas.Children.Cast<UIElement>().Where(ui => ui.Uid.Equals(uId)).ToArray();
            foreach (var uiItem in itemsToRemove)
            {
                canvas.Children.Remove(uiItem);
            }
        }

        public static int CountElements(this Canvas canvas, string uId)
        {
            return canvas.Children.Cast<UIElement>().Count(ui => ui.Uid.Equals(uId));
        }

        public static void SetElement(this Canvas canvas, UIElement element, Point location)
        {
            Canvas.SetLeft(element, location.X);
            Canvas.SetTop(element, location.Y);
            
            canvas.Children.Add(element);
        }
    }
}
