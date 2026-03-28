using UnityEngine;

namespace SSMPEssentials.Utils.Tests
{
    internal abstract class BaseTest
    {
        public abstract KeyCode KeyCode { get; }

        public abstract void Execute();
    }
}
