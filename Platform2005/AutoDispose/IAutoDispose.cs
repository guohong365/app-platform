namespace Platform.AutoDispose
{
    using System;

    public interface IAutoDispose
    {
        void Dispose();

        Guid Index { get; }

        bool IsCheckOut { get; set; }

        bool IsTimeout { get; }
    }
}
