using System.Collections.Generic;
using UnityEngine;

namespace SSMPEssentials.Utils.Tests
{
    internal class TestManager
    {
        List<BaseTest> tests = [];

        public TestManager()
        {
            tests.Add(new WarpTest());
        }

        public void Update()
        {
            foreach (var test in tests)
            {
                if (Input.GetKeyDown(test.KeyCode))
                {
                    test.Execute();
                }
            }
        }
    }
}
