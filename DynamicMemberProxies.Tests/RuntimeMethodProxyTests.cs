using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicMemberProxies.Tests
{
    /// <summary>
    /// Contains unit tests related with the <see cref="RuntimeMethodProxy"/>
    /// </summary>
    [TestClass]
    public class RuntimeMethodProxyTests
    {
        #region Types

        /// <summary>
        /// Test class
        /// </summary>
        public class Test
        {
            public int Method1()
            {
                return 1;
            }

            public int Method2(int a)
            {
                return a + 1;
            }

            public static int StaticMethod1()
            {
                return 1;
            }

            public static int StaticMethod2(int a)
            {
                return a + 1;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tests calling an instance method which has no parameter
        /// </summary>
        [TestMethod]
        public void InstanceMethodWithNoParameter()
        {
            var type = typeof(Test);
            var instance = new Test();

            var methodInfo = type.GetMethod("Method1");
            var proxy = (RuntimeMethodProxy)methodInfo;

            var result = proxy.Invoke<int>(instance, null);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Tests calling an instance method which has parameter
        /// </summary>
        [TestMethod]
        public void InstanceMethodWithParameter()
        {
            var type = typeof(Test);
            var instance = new Test();

            var methodInfo = type.GetMethod("Method2");
            var proxy = (RuntimeMethodProxy)methodInfo;

            var result = proxy.Invoke<int>(instance, 1);
            Assert.AreEqual(2, result);
        }

        /// <summary>
        /// Tests calling an static method which has no parameter
        /// </summary>
        [TestMethod]
        public void StaticMethodWithNoParameter()
        {
            var type = typeof(Test);

            var methodInfo = type.GetMethod("StaticMethod1");
            var proxy = (RuntimeMethodProxy)methodInfo;
            var result = proxy.Invoke<int>(null, null);
            Assert.AreEqual(1, result);
        }

        /// <summary>
        /// Tests calling an static method which has parameter
        /// </summary>
        [TestMethod]
        public void StaticMethodWithParameter()
        {
            var type = typeof(Test);

            var methodInfo = type.GetMethod("StaticMethod2");
            var proxy = (RuntimeMethodProxy)methodInfo;
            var result = proxy.Invoke<int>(null, 1);
            Assert.AreEqual(2, result);
        }

        #endregion
    }
}
