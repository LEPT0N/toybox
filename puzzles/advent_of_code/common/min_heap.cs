using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace advent_of_code_common.min_heap
{
    [DebuggerDisplay("count = {count}, min = {data[0]}, comparer = ({comparer})", Type = "c_min_heap")]
    public class c_min_heap<T> where T : class
    {
        private int count = 0;
        private T[] data = new T[4];
        private IComparer<T> comparer;

        private static readonly int k_root_index = 1;

        public c_min_heap(IComparer<T> c)
        {
            comparer = c;
        }

        public bool empty()
        {
            return count == 0;
        }

        public void add(T item)
        {
            // Expand data if needed.
            if (count == data.Length - 1)
            {
                T[] new_data = new T[data.Length * 2];
                Array.Copy(data, new_data, count + 1);
                data = new_data;
            }

            // Add item to the end of data
            count++;
            int index = count;
            data[index] = item;

            // Work up the tree swapping items with its parent as needed.
            int parent_index = index / 2;
            while (index > k_root_index && comparer.Compare(data[index], data[parent_index]) < 0)
            {
                T temp = data[index];
                data[index] = data[parent_index];
                data[parent_index] = temp;

                index = parent_index;
                parent_index = index / 2;
            }
        }

        public T remove()
        {
            if (count == 0)
            {
                return null;
            }

            // Grab the root item and fill the root index with what was last in data.
            T result = data[k_root_index];
            data[k_root_index] = data[count];
            data[count] = null;
            count--;

            // TODO: make data smaller

            // Work down the tree swapping parents with the smallest child as needed.
            int parent_index = 1;
            T parent = data[parent_index];

            while (true)
            {
                // Find the two children.
                int left_child_index = parent_index * 2;
                T left_child = (left_child_index <= count) ? data[left_child_index] : null;

                int right_child_index = left_child_index + 1;
                T right_child = (right_child_index <= count) ? data[right_child_index] : null;

                // If no children, done.
                if (left_child == null && right_child == null)
                {
                    break;
                }

                // Find the index of the smallest child
                int smallest_child_index = left_child_index;

                if (left_child != null && right_child != null)
                {
                    if (comparer.Compare(left_child, right_child) > 0)
                    {
                        smallest_child_index = right_child_index;
                    }
                }
                else if (right_child != null)
                {
                    smallest_child_index = right_child_index;
                }

                // If the smallest child is smaller than the parent, swap and keep going. Otherwise we're done.
                if (comparer.Compare(data[parent_index], data[smallest_child_index]) > 0)
                {
                    T temp = data[smallest_child_index];
                    data[smallest_child_index] = data[parent_index];
                    data[parent_index] = temp;

                    parent_index = smallest_child_index;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}
