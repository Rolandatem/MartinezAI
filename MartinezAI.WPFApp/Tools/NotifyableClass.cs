using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MartinezAI.WPFApp.Tools;

public abstract class NotifyableClass : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    // Standard property notification (by name or caller)
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    // Lambda-based property notification for dependencies: OnPropertyChanged(() => FullName)
    protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
    {
        if (propertyExpression.Body is MemberExpression memberExp)
            OnPropertyChanged(memberExp.Member.Name);
        else
            throw new ArgumentException("Invalid property expression", nameof(propertyExpression));
    }

    // Overload: set field, notify property change, returns true if changed
    protected bool OnPropertyChanged<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}