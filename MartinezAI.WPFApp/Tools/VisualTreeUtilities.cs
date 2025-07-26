using System.Windows;
using System.Windows.Media;

public static class VisualTreeUtilities
{
    public static T? FindChild<T>(DependencyObject parent) where T : DependencyObject
    {
        if (parent == null) return null;

        int count = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is T typedChild)
                return typedChild;

            var result = FindChild<T>(child);
            if (result != null)
                return result;
        }
        return null;
    }

    public static T? FindAncestor<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject? parent = VisualTreeHelper.GetParent(child);

        while (parent != null)
        {
            if (parent is T ancestor)
                return ancestor;

            parent = VisualTreeHelper.GetParent(parent);
        }
        return null;
    }
}