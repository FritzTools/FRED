using System;

namespace FRED.Core.Interfaces {
    public interface Changeable {
        public void OnChange(object? sender, EventArgs e);
        public void OnReady(Action callback);
    }
}
