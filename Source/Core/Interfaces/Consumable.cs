using System;

namespace FRED.Core.Interfaces {
    public interface Consumable {
        public bool Get(String name, Action<object?> callback);
        public bool IsReady();
        public void OnReady(Action callback);
    }
}
