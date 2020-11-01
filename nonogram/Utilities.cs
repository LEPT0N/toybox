using System;

namespace Nonogram
{
    public static class Utilities
    {
        public static string To_String<T>(this T[] list)
        {
            return "[ " + string.Join(", ", list) + " ]";
        }

        public static void Swap_Elements<T>(this T[] list, int i, int j)
        {
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        public static void Reverse_Elements<T>(this T[] list, int i, int j)
        {
            while (i < j)
            {
                list.Swap_Elements(i, j);
                i++;
                j--;
            }
        }

        // If there is another possible permutation, return 'true' and edit the input to be the next permutation
        // Written by me after understanding this explanation of std:next_permutation:
        //      https://stackoverflow.com/questions/11483060/stdnext-permutation-implementation-explanation
        // To use this, the first permutation is in ascending order, and the last permutation is in descending order.
        public static bool Get_Next_Permutation(this int[] list)
        {
            int first = 0;
            int last = list.Length - 1;

            int i = last - 1;

            // Scan from the end of the list to find the first element not in descending order.
            while (i >= first)
            {
                if (list[i] < list[i + 1])
                {
                    // list[i + 1, last] is in descending order, but list[i] doesn't follow the trend.

                    // find the entry in list[i + 1, last] that is the next largest after list[i]
                    // We're guaranteed to find something since list[i] < list[i + 1]
                    int next_largest = last;
                    while (list[i] >= list[next_largest])
                    {
                        next_largest--;
                    }

                    // put the next largest element into list[i] by swapping it with list[i]
                    // Note: list[i + 1, last] is still in descending order after this!
                    list.Swap_Elements(i, next_largest);

                    // Now that the next-largest element is in list[i], reverse list[i + 1, last] so that it's in ascending order.
                    list.Reverse_Elements(i + 1, last);

                    return true;
                }

                i--;
            }

            // The entire list is in descending order. No more permutations!
            return false;
        }
    }
}
