using System;
using System.Windows.Controls;

public static class NavigationService
{
    public static Action<UserControl> Navigate;

    public static void NavigateTo(UserControl view)
    {
        Navigate?.Invoke(view);
    }
}