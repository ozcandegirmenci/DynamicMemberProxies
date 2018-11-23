using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicMemberProxies.Tests
{
    /// <summary>
    /// Contains unit tests related with the <see cref="RuntimePropertyProxy"/>
    /// </summary>
    [TestClass]
    public class RuntimePropertyProxyTests
    {
        #region Types

        /// <summary>
        /// Test class
        /// </summary>
        private class Test
        {
            private List<string> _Items = new List<string>() { "Ozcan", "Degirmenci" };

            public int InstancePublicValueType { get; set; }

            private string InstancePrivateReferenceType { get; set; }

            public string this[int index]
            {
                get { return _Items[index]; }
                set { _Items[index] = value; }
            }

            public static bool StaticPublicValueType { get; set; }

            public static string StaticPublicReferenceType { get; set; }

            static Test()
            {
                StaticPublicReferenceType = "Ozcan";
                StaticPublicValueType = true;
            }

            public Test()
            {
                InstancePublicValueType = 1;
                InstancePrivateReferenceType = "Ozcan";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests to access a public instance property which is type is a value type
        /// </summary>
        [TestMethod]
        public void InstancePublicValueType()
        {
            var type = typeof(Test);
            var instance = new Test();

            var propertyInfo = type.GetProperty("InstancePublicValueType");
            var proxy = (RuntimePropertyProxy)propertyInfo;
            var result = proxy.GetValue<int>(instance);
            Assert.AreEqual(1, result);

            proxy.SetValue(instance, 5);
            Assert.AreEqual(5, instance.InstancePublicValueType);
        }

        /// <summary>
        /// Tests to access a private instance property which is type is a reference type
        /// </summary>
        [TestMethod]
        public void InstancePrivateReferenceType()
        {
            var type = typeof(Test);
            var instance = new Test();

            var propertyInfo = type.GetProperty("InstancePrivateReferenceType", BindingFlags.NonPublic | BindingFlags.Instance);
            var proxy = (RuntimePropertyProxy)propertyInfo;
            var result = proxy.GetValue<string>(instance);
            Assert.AreEqual("Ozcan", result);

            proxy.SetValue(instance, "Degirmenci");
            Assert.AreEqual("Degirmenci", proxy.GetValue<string>(instance));
        }

        /// <summary>
        /// Tests to access a public static property which is type is a reference type
        /// </summary>
        [TestMethod]
        public void StaticPublicReferenceType()
        {
            var type = typeof(Test);;

            var propertyInfo = type.GetProperty("StaticPublicReferenceType", BindingFlags.Public | BindingFlags.Static);
            var proxy = (RuntimePropertyProxy)propertyInfo;
            var result = proxy.GetValue<string>(null);
            Assert.AreEqual("Ozcan", result);

            proxy.SetValue(null, "Degirmenci");
            Assert.AreEqual("Degirmenci", proxy.GetValue<string>(null));
        }

        /// <summary>
        /// Tests to access a public static property which is type is a value type
        /// </summary>
        [TestMethod]
        public void StaticPublicValueType()
        {
            var type = typeof(Test);

            var propertyInfo = type.GetProperty("StaticPublicValueType", BindingFlags.Public | BindingFlags.Static);
            var proxy = (RuntimePropertyProxy)propertyInfo;
            var result = proxy.GetValue<bool>(null);
            Assert.AreEqual(true, result);

            proxy.SetValue(null, false);
            Assert.AreEqual(false, proxy.GetValue<bool>(null));
        }

        /// <summary>
        /// Tests to access a public instance index property
        /// </summary>
        [TestMethod]
        public void InstancePublicIndex()
        {
            var type = typeof(Test);
            var instance = new Test();

            var propertyInfo = type.GetProperty("Item");
            var proxy = (RuntimePropertyProxy)propertyInfo;
            var result = proxy.GetValue<string>(instance, 0);
            Assert.AreEqual("Ozcan", result);

            proxy.SetValue(instance, "Degirmenci", new object[] { 0 });
            Assert.AreEqual("Degirmenci", proxy.GetValue<string>(instance, 0));
        }

        #endregion
    }
}
