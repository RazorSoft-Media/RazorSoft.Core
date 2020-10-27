// Copyright © 2020 RazorSoft Media, DBA, Lone Star Logistics & Transport, LLC. All Rights Reserved.


using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorSoft.Core.DependencyInjection;

namespace UnitTest.RazorSoft {

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class InjectAttributeTests {

        [TestMethod]
        public void InjectPrivateField() {
            var testme = new TestMe();
            var fieldList = typeof(TestMe).GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                .Where(f => f.CustomAttributes.Any(a => a.AttributeType == typeof(InjectAttribute)))
                .ToList();

            Assert.AreEqual(1, fieldList.Count);

            var dependencyField = fieldList[0];

            var expDependency = new DependencyObject();
            dependencyField.SetValue(testme, expDependency);

            Assert.IsTrue(ReferenceEquals(expDependency, testme.Dependency));
        }


        private class TestMe {
            [Inject]
            private readonly DependencyObject dependency;

            public DependencyObject Dependency => dependency;
        }

        public class DependencyObject {
            public string Hello => "Hello";
        }
    }
}
