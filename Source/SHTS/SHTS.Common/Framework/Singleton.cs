namespace Witbird.SHTS.Common
{
    /// <summary>
    /// Abstract class for Singleton. T should have parameterless public constructor and be sub-class itself.
    /// <example>This is a code example:
    /// <code>
    /// public class SingletonTestObject: Singleton&lt;SingletonTestObject&gt;
    /// {
    /// public string DoSomething(){...}
    /// }
    /// string result = SingletonTestObject.Instance.DoSomething();
    /// </code>
    /// </example>
    /// </summary>
    /// <typeparam name="T">Singleton instance type.</typeparam>
    public abstract class Singleton<T> where T : new()
    {
        #region Singleton

        /// <summary>
        /// The locker for initializing singleton instance.
        /// </summary>
        static readonly object locker = new object();

        /// <summary>
        /// The instance for singleton.
        /// </summary>
        private static T instance;

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>The instance.</value>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            instance = new T();
                        }
                    }
                }
                return instance;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Singleton{T}" /> class.
        /// </summary>
        protected Singleton()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
            //Do nothing here.
        }
    }
}
