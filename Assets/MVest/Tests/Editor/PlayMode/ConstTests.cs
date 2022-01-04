using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using MVest;

namespace MVest.Tests {


    public class ConstTests {
        [Test]
        public void ComparisonMinMax() {
            IConstant<int> min = new Const.MinValue();
            IConstant<int> max = new Const.MaxValue();
            min.Value(out var minVal);
            max.Value(out var maxVal);

            Assert.Less(minVal, maxVal);
        }

    }
}
