using System.Collections;
using System.Collections.Generic;

namespace Core.Runtime.Items
{

    public class ItemIterator : IEnumerable<Item>
    {
        private Dictionary<int, Item> m_itemMap;

        public ItemIterator(Dictionary<int, Item> itemMap)
        {
            m_itemMap = itemMap;
        }
        public IEnumerator<Item> GetEnumerator()
        {
            var enumerator = m_itemMap.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current.Value;
            }
            
            enumerator.Dispose();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}