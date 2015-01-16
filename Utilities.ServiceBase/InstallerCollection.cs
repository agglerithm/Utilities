using System;
using System.Collections;

namespace Utilities.WinMockups
{
    public class InstallerCollection : CollectionBase
    {
        private Installer owner;

        public Installer this[int index]
        {
            get
            {
                return (Installer)this.List[index];
            }
            set
            {
                this.List[index] = (object)value;
            }
        }

        internal InstallerCollection(Installer owner)
        {
            this.owner = owner;
        }

        public int Add(Installer value)
        {
            return this.List.Add((object)value);
        }

        public void AddRange(InstallerCollection value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            int count = value.Count;
            for (int index = 0; index < count; ++index)
                this.Add(value[index]);
        }

        public void AddRange(Installer[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");
            for (int index = 0; index < value.Length; ++index)
                this.Add(value[index]);
        }

        public bool Contains(Installer value)
        {
            return this.List.Contains((object)value);
        }

        public void CopyTo(Installer[] array, int index)
        {
            this.List.CopyTo((Array)array, index);
        }

        public int IndexOf(Installer value)
        {
            return this.List.IndexOf((object)value);
        }

        public void Insert(int index, Installer value)
        {
            this.List.Insert(index, (object)value);
        }

        public void Remove(Installer value)
        {
            this.List.Remove((object)value);
        }

        protected override void OnInsert(int index, object value)
        {
            if (value == this.owner)
                throw new ArgumentException("CantAddSelf");
            int num = CompModSwitches.InstallerDesign.TraceVerbose ? 1 : 0;
            ((Installer)value).parent = this.owner;
        }

        protected override void OnRemove(int index, object value)
        {
            int num = CompModSwitches.InstallerDesign.TraceVerbose ? 1 : 0;
            ((Installer)value).parent = (Installer)null;
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
            if (newValue == this.owner)
                throw new ArgumentException("CantAddSelf");
            int num = CompModSwitches.InstallerDesign.TraceVerbose ? 1 : 0;
            ((Installer)oldValue).parent = (Installer)null;
            ((Installer)newValue).parent = this.owner;
        }
    }
}