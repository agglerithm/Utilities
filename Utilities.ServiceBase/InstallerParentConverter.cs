using System;
using System.Collections;
using System.ComponentModel;

namespace Utilities.WinMockups
{
    internal class InstallerParentConverter : ReferenceConverter
    {
        public InstallerParentConverter(Type type)
            : base(type)
        {
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            TypeConverter.StandardValuesCollection standardValues = base.GetStandardValues(context);
            object instance = context.Instance;
            int index1 = 0;
            int index2 = 0;
            object[] objArray = new object[standardValues.Count - 1];
            for (; index1 < standardValues.Count; ++index1)
            {
                if (standardValues[index1] != instance)
                {
                    objArray[index2] = standardValues[index1];
                    ++index2;
                }
            }
            return new TypeConverter.StandardValuesCollection((ICollection)objArray);
        }
    }
}