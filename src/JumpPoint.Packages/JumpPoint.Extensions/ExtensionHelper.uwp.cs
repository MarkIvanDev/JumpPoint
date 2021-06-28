using System;
using System.Collections.Generic;
using System.Text;
using Windows.Foundation.Collections;

namespace JumpPoint.Extensions
{
    public static class ExtensionHelper
    {
        public static ValueSet ToValueSet(this IDictionary<string, object> data)
        {
            try
            {
                var valueSet = new ValueSet();
                foreach (var item in data)
                {
                    valueSet.Add(item.Key, item.Value);
                }
                return valueSet;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
