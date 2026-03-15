using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SSMP_Utils.Utils.Tests
{
    internal abstract class BaseTest
    {
        public abstract KeyCode KeyCode { get; }

        public abstract void Execute();
    }
}
