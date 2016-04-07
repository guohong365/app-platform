namespace Platform.Task
{
    using System;

    public enum AutoTaskState
    {
        Disabled = 4,
        Handled = 3,
        Handling = 2,
        Suspend = 5,
        UnHandled = 1
    }
}
