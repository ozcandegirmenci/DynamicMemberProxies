using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicMemberProxies.Tests
{
    /// <summary>
    /// Contains unit tests related with the <see cref="RuntimeFieldProxy"/>
    /// </summary>
    [TestClass]
    public class RuntimeFieldProxyTests
    {
        #region Types

        /// <summary>
        /// Test class
        /// </summary>
        private class Test
        {
            public int PublicValueType = 1;
            private int PrivateValueType = 2;
            public string PublicReferenceType = "Ozcan Degirmenci";

            public static int StaticPublicValueType = 1;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests to access a public instance field which is type is a value type
        /// </summary>
        [TestMethod]
        public void InstancePublicValueType()
        {
            var type = typeof(Test);
            var instance = new Test();

            var fieldInfo = type.GetField("PublicValueType");
            var proxy = (RuntimeFieldProxy)fieldInfo;
            var result = proxy.GetValue<int>(instance);
            Assert.AreEqual(1, result);

            proxy.SetValue(instance, 5);
            Assert.AreEqual(5, instance.PublicValueType);
        }

        /// <summary>
        /// Tests to access a private instance field which is type is a value type
        /// </summary>
        [TestMethod]
        public void InstancePrivateValueType()
        {
            var type = typeof(Test);
            var instance = new Test();

            var fieldInfo = type.GetField("PrivateValueType", BindingFlags.NonPublic | BindingFlags.Instance);
            var proxy = (RuntimeFieldProxy)fieldInfo;
            var result = proxy.GetValue<int>(instance);
            Assert.AreEqual(2, result);

            proxy.SetValue(instance, 5);
            Assert.AreEqual(5, proxy.GetValue<int>(instance));
        }

        /// <summary>
        /// Tests to access a public instance field which is type is a reference type
        /// </summary>
        [TestMethod]
        public void InstancePublicReferenceType()
        {
            var type = typeof(Test);
            var instance = new Test();

            var fieldInfo = type.GetField("PublicReferenceType");
            var proxy = (RuntimeFieldProxy)fieldInfo;
            var result = proxy.GetValue(instance)?.ToString();
            Assert.AreEqual("Ozcan Degirmenci", result);

            proxy.SetValue(instance, "Ozcan");
            Assert.AreEqual("Ozcan", proxy.GetValue(instance)?.ToString());
        }

        /// <summary>
        /// Tests to access a public static field which is type is a value type
        /// </summary>
        [TestMethod]
        public void StaticPublicValueType()
        {
            var type = typeof(Test);
            var instance = new Test();

            var fieldInfo = type.GetField("StaticPublicValueType", BindingFlags.Public | BindingFlags.Static);
            var proxy = (RuntimeFieldProxy)fieldInfo;
            var result = proxy.GetValue<int>(instance);
            Assert.AreEqual(1, result);

            proxy.SetValue(instance, 5);
            Assert.AreEqual(5, proxy.GetValue<int>(instance));
        }

        #endregion
    }
}
