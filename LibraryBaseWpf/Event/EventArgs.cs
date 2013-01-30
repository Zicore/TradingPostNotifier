using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace LibraryBase.Wpf.Event
{
    /// <summary>
    /// Type Save Generic Eventargs
    /// </summary>
    /// <typeparam name="T">Type of the Argument Value</typeparam>
    /// <remarks>This generic inherited version of the default eventargs can be used to avoid many dummy eventarg classes. </remarks>
    public class EventArgs<T> : System.EventArgs
    {
        //[ContractInvariantMethod]
        //void ObjectInvariant()
        //{
        //    Contract.Invariant(this.Value != null);
        //}

        private T m_Value;

        /// <summary>
        /// Gets or sets the value of the EventArgs.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                return m_Value;
            }
            private set
            {

                m_Value = value;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEventArgs&lt;T&gt;"/> class.
        /// </summary>
        public EventArgs()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericEventArgs&lt;T&gt;"/> class.
        /// </summary>
        public EventArgs(T value)
            : this()
        {
            //Contract.Requires<ArgumentNullException>(value != null);
            //Contract.Ensures(this.Value != null);
            this.Value = value;
        }
    }
}
