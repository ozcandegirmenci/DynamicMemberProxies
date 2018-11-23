using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using DynamicMemberProxies;

namespace DynamicMethodProxies.Benchmark
{
    /// <summary>
    /// Console application for benchmarking proxy classes versus other methodologies
    /// </summary>
    class Program
    {
        #region Types

        /// <summary>
        /// Base benchmark item
        /// </summary>
        private abstract class BenchmarkItem
        {
            /// <summary>
            /// Gets the name of the benchmark item
            /// </summary>
            public abstract string Name { get; }

            /// <summary>
            /// Gets the time elapsed for this item
            /// </summary>
            public long TimeElapsed { get; private set; }

            /// <summary>
            /// Initialize a new instance of this class
            /// </summary>
            protected BenchmarkItem()
            {
            }

            /// <summary>
            /// Runs benchmark operation
            /// </summary>
            /// <param name="iteration"></param>
            public void Run(int iteration)
            {
                OnPrepare();
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                OnRun(iteration);
                stopWatch.Stop();
                TimeElapsed = stopWatch.ElapsedMilliseconds;
            }

            /// <summary>
            /// Applies internal run operation for the overrided classes
            /// </summary>
            /// <param name="interation"></param>
            /// <returns></returns>
            protected abstract object OnRun(int interation);

            /// <summary>
            /// Prepares item before the run
            /// </summary>
            protected virtual void OnPrepare()
            { }
        }

        /// <summary>
        /// Instance creation benchmark items
        /// </summary>
        private class InstanceCreation
        {
            #region Types

            /// <summary>
            /// new .... benchmark item
            /// </summary>
            internal class NewCreation : BenchmarkItem
            {
                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "New";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    ArrayList item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = new ArrayList();
                    }
                    return item;
                }
            }

            /// <summary>
            /// Activator.CreateInstance benchmark item
            /// </summary>
            internal class ActivatorCreation : BenchmarkItem
            {
                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Activator";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    ArrayList item = null;
                    var type = typeof(ArrayList);
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Activator.CreateInstance(type) as ArrayList;
                    }
                    return item;
                }
            }

            /// <summary>
            /// Dynamic proxy generation benchmark item
            /// </summary>
            internal class DynamicProxyCreation : BenchmarkItem
            {
                private RuntimeConstructorProxy Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "-> Proxy";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    ArrayList item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Proxy.Invoke<ArrayList>();
                    }
                    return item;
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = RuntimeConstructorProxy.Create<ArrayList>();
                }
            }

            /// <summary>
            /// Reflection benchmark item
            /// </summary>
            internal class ReflectionCreation : BenchmarkItem
            {
                private ConstructorInfo Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Reflection";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    ArrayList item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Proxy.Invoke(null) as ArrayList;
                    }
                    return item;
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = typeof(ArrayList).GetConstructors()[0];
                }
            }

            #endregion
        }

        /// <summary>
        /// Method invocation benchmark items
        /// </summary>
        private class MethodInvocation
        {
            /// <summary>
            /// Code execution benchmark item
            /// </summary>
            internal class CodeExecution : BenchmarkItem
            {
                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Code Execution";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = 1.ToString();
                    }
                    return item;
                }
            }

            /// <summary>
            /// Dynamic proxy generation benchmark item
            /// </summary>
            internal class DynamicProxyCall : BenchmarkItem
            {
                private RuntimeMethodProxy Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "-> Proxy";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Proxy.Invoke<string>(1, null);
                    }
                    return item;
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = RuntimeMethodProxy.Create(typeof(int).GetMethod("ToString", new Type[0]));
                }
            }

            /// <summary>
            /// Reflection benchmark item
            /// </summary>
            internal class ReflectionCall : BenchmarkItem
            {
                private MethodInfo Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Reflection";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Proxy.Invoke(1, null) as string;
                    }
                    return item;
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = typeof(int).GetMethod("ToString", new Type[0]);
                }
            }
        }

        /// <summary>
        /// Field get invocation benchmark items
        /// </summary>
        private class FieldGetInvocation
        {
            /// <summary>
            /// Code execution benchmark item
            /// </summary>
            internal class CodeExecution : BenchmarkItem
            {
                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Code Execution";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = string.Empty;
                    }
                    return item;
                }
            }

            /// <summary>
            /// Dynamic proxy generation benchmark item
            /// </summary>
            internal class DynamicProxyCall : BenchmarkItem
            {
                private RuntimeFieldProxy Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "-> Proxy";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Proxy.GetValue<string>(null);
                    }
                    return item;
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = RuntimeFieldProxy.Create(typeof(string).GetField("Empty"));
                }
            }

            /// <summary>
            /// Reflection benchmark item
            /// </summary>
            internal class ReflectionCall : BenchmarkItem
            {
                private FieldInfo Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Reflection";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Proxy.GetValue(null) as string;
                    }
                    return item;
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = typeof(string).GetField("Empty");
                }
            }
        }

        /// <summary>
        /// Field set invocation benchmark items
        /// </summary>
        private class FieldSetInvocation
        {
            /// <summary>
            /// Code execution benchmark item
            /// </summary>
            internal class CodeExecution : BenchmarkItem
            {
                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Code Execution";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    for (int i = 0; i < iteration; i++)
                    {
                        test.PublicField = i;
                    }
                    return test.PublicField.ToString();
                }
            }

            /// <summary>
            /// Dynamic proxy generation benchmark item
            /// </summary>
            internal class DynamicProxyCall : BenchmarkItem
            {
                private RuntimeFieldProxy Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "-> Proxy";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    for (int i = 0; i < iteration; i++)
                    {
                        Proxy.SetValue(test, i);
                    }
                    return test.PublicField.ToString();
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = RuntimeFieldProxy.Create(typeof(TestClass).GetField("PublicField"));
                }
            }

            /// <summary>
            /// Reflection benchmark item
            /// </summary>
            internal class ReflectionCall : BenchmarkItem
            {
                private FieldInfo Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Reflection";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    for (int i = 0; i < iteration; i++)
                    {
                        Proxy.SetValue(test, i);
                    }
                    return test.PublicField.ToString();
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = typeof(TestClass).GetField("PublicField");
                }
            }
        }


        /// <summary>
        /// Property get invocation benchmark items
        /// </summary>
        private class PropertyGetInvocation
        {
            /// <summary>
            /// Code execution benchmark item
            /// </summary>
            internal class CodeExecution : BenchmarkItem
            {
                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Code Execution";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = test.PublicProperty;
                    }
                    return item;
                }
            }

            /// <summary>
            /// Dynamic proxy generation benchmark item
            /// </summary>
            internal class DynamicProxyCall : BenchmarkItem
            {
                private RuntimePropertyProxy Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "-> Proxy";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Proxy.GetValue<string>(test);
                    }
                    return item;
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = RuntimePropertyProxy.Create(typeof(TestClass).GetProperty("PublicProperty"));
                }
            }

            /// <summary>
            /// Reflection benchmark item
            /// </summary>
            internal class ReflectionCall : BenchmarkItem
            {
                private PropertyInfo Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Reflection";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    string item = null;
                    for (int i = 0; i < iteration; i++)
                    {
                        item = Proxy.GetValue(test) as string;
                    }
                    return item;
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = typeof(TestClass).GetProperty("PublicProperty");
                }
            }
        }

        /// <summary>
        /// Property set invocation benchmark items
        /// </summary>
        private class PropertySetInvocation
        {
            /// <summary>
            /// Code execution benchmark item
            /// </summary>
            internal class CodeExecution : BenchmarkItem
            {
                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Code Execution";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    for (int i = 0; i < iteration; i++)
                    {
                        test.PublicProperty = i.ToString();
                    }
                    return test.PublicProperty.ToString();
                }
            }

            /// <summary>
            /// Dynamic proxy generation benchmark item
            /// </summary>
            internal class DynamicProxyCall : BenchmarkItem
            {
                private RuntimePropertyProxy Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "-> Proxy";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    for (int i = 0; i < iteration; i++)
                    {
                        Proxy.SetValue(test, i.ToString());
                    }
                    return test.PublicProperty.ToString();
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = RuntimePropertyProxy.Create(typeof(TestClass).GetProperty("PublicProperty"));
                }
            }

            /// <summary>
            /// Reflection benchmark item
            /// </summary>
            internal class ReflectionCall : BenchmarkItem
            {
                private FieldInfo Proxy;

                /// <summary>
                /// Gets the name of the item
                /// </summary>
                public override string Name => "Reflection";

                /// <summary>
                /// Internal benchmark operation
                /// </summary>
                /// <param name="iteration"></param>
                /// <returns></returns>
                protected override object OnRun(int iteration)
                {
                    var test = new TestClass();
                    for (int i = 0; i < iteration; i++)
                    {
                        Proxy.SetValue(test, i.ToString());
                    }
                    return test.PublicProperty.ToString();
                }

                /// <summary>
                /// Prepares benchmark item for usage
                /// </summary>
                protected override void OnPrepare()
                {
                    Proxy = typeof(TestClass).GetField("PublicProperty");
                }
            }
        }

        /// <summary>
        /// Test class
        /// </summary>
        private class TestClass
        {
            public int PublicField;

            public string PublicProperty { get; set; }
        }

        /// <summary>
        /// Benchmark group operation container and runner
        /// </summary>
        private class BenchmarkGroup : IEnumerable<BenchmarkItem>
        {
            private readonly List<BenchmarkItem> _Items = new List<BenchmarkItem>();
            private static readonly List<ConsoleColor> _Colors = new List<ConsoleColor> {
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.Red,
                ConsoleColor.DarkYellow
            };

            /// <summary>
            /// Gets the name of the group operation
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// Initialize a new instance of this class
            /// </summary>
            /// <param name="name"></param>
            public BenchmarkGroup(string name)
            {
                Name = name;
            }

            /// <summary>
            /// Runs benchmark operation
            /// </summary>
            /// <param name="iteration"></param>
            public void Run(int iteration)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(string.Format("{0} -> Iteration Count: {1}", Name, iteration));
                Console.WriteLine("--------------------------------------");

                for (int i = 0; i < _Items.Count; i++)
                {
                    GC.Collect();
                    _Items[i].Run(iteration);
                }
                Sort();
                
                for (int i = 0; i < _Items.Count; i++)
                {
                    Console.ForegroundColor = _Colors[i];
                    var item = _Items[i];
                    Console.WriteLine(string.Format("{0}: {1} ms", item.Name, item.TimeElapsed));
                }
                Console.WriteLine();
            }

            /// <summary>
            /// Adds a benchmark item
            /// </summary>
            /// <param name="data"></param>
            public void Add(BenchmarkItem data)
            {
                _Items.Add(data);
            }

            /// <summary>
            /// Sorts benchmark items according to their Elapsed time
            /// </summary>
            public void Sort()
            {
                _Items.Sort((x, y) => x.TimeElapsed.CompareTo(y.TimeElapsed));
            }

            /// <summary>
            /// Returns an <see cref="IEnumerator"/> instance which provides enumerating through this class
            /// </summary>
            /// <returns></returns>
            public IEnumerator<BenchmarkItem> GetEnumerator()
            {
                return _Items.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        #endregion

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing and runing tests ....");
            var instanceGroup = new BenchmarkGroup("Instance Creation")
            {
                new InstanceCreation.NewCreation(),
                new InstanceCreation.DynamicProxyCreation(),
                new InstanceCreation.ActivatorCreation(),
                new InstanceCreation.ReflectionCreation()
            };

            var methodGroup = new BenchmarkGroup("Method Invocation")
            {
                new MethodInvocation.CodeExecution(),
                new MethodInvocation.DynamicProxyCall(),
                new MethodInvocation.ReflectionCall()
            };

            var fieldGetGroup = new BenchmarkGroup("Field Get Invocation")
            {
                new FieldGetInvocation.CodeExecution(),
                new FieldGetInvocation.DynamicProxyCall(),
                new FieldGetInvocation.ReflectionCall()
            };

            var fieldSetGroup = new BenchmarkGroup("Field Set Invocation")
            {
                new FieldGetInvocation.CodeExecution(),
                new FieldGetInvocation.DynamicProxyCall(),
                new FieldGetInvocation.ReflectionCall()
            };

            var propertyGetGroup = new BenchmarkGroup("Property Get Invocation")
            {
                new PropertyGetInvocation.CodeExecution(),
                new PropertyGetInvocation.DynamicProxyCall(),
                new PropertyGetInvocation.ReflectionCall()
            };

            var propertySetGroup = new BenchmarkGroup("Property Set Invocation")
            {
                new PropertyGetInvocation.CodeExecution(),
                new PropertyGetInvocation.DynamicProxyCall(),
                new PropertyGetInvocation.ReflectionCall()
            };
            Console.WriteLine("Press ENTER to start");
            Console.ReadLine();

            const int ITERATION = 1000000;
            
            instanceGroup.Run(ITERATION);
            methodGroup.Run(ITERATION);
            fieldGetGroup.Run(ITERATION);
            fieldSetGroup.Run(ITERATION);
            propertyGetGroup.Run(ITERATION);
            propertySetGroup.Run(ITERATION);

            Console.ReadLine();
        }
    }
}
