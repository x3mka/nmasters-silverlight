using System;
using System.Collections.ObjectModel;

namespace NMasters.Silverlight.Net.Http.Headers
{
    internal class ObjectCollection<T> : Collection<T> where T: class
    {
        private static readonly Action<T> defaultValidator;
        private Action<T> validator;

        static ObjectCollection()
        {
            ObjectCollection<T>.defaultValidator = new Action<T>(ObjectCollection<T>.CheckNotNull);
        }

        public ObjectCollection() : this(ObjectCollection<T>.defaultValidator)
        {
        }

        public ObjectCollection(Action<T> validator)
        {
            this.validator = validator;
        }

        private static void CheckNotNull(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
        }

        protected override void InsertItem(int index, T item)
        {
            if (this.validator != null)
            {
                this.validator(item);
            }
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (this.validator != null)
            {
                this.validator(item);
            }
            base.SetItem(index, item);
        }
    }
}

