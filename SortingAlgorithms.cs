using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ITCS_6114_Project_1
{
    internal class SortingAlgorithms
    {
        //InsertionSort
        public static void InsertionSort(List<int> list, int l, int r)
        {
            for (int i = l+1; i < r; i++)
            {
                int key = list[i];
                int j = i - 1;
                while (j >= 0 && list[j] > key)
                {
                    list[j + 1] = list[j];
                    j = j - 1;
                }
                list[j + 1] = key;
            }
        }

        //MergeSort
        public static List<int> MergeSort(List<int> list)
        {
            if (list.Count > 1)
            {
                (List<int> firstHalf, List<int> secondHalf) = (list.Take(list.Count / 2).ToList(), list.Skip(list.Count / 2).ToList());
                firstHalf = MergeSort(firstHalf);
                secondHalf = MergeSort(secondHalf);
                return Merge(firstHalf, secondHalf);
            }
            return list;
        }

        public static List<int> Merge(List<int> firstHalf, List<int> secondHalf)
        {
            List<int> s = new();
            while (!(firstHalf.Count == 0) && !(secondHalf.Count == 0))
            {
                if (firstHalf[0] < secondHalf[0])
                {
                    s.Add(firstHalf[0]);
                    firstHalf.RemoveAt(0);
                }
                else
                {
                    s.Add(secondHalf[0]);
                    secondHalf.RemoveAt(0);
                }
            }

            while (firstHalf.Count > 0)
            {
                s.Add(firstHalf[0]);
                firstHalf.RemoveAt(0);
            }

            while (secondHalf.Count > 0)
            {
                s.Add(secondHalf[0]);
                secondHalf.RemoveAt(0);
            }

            return s;
        }

        //HeapSort
        //Vector-based version, sequential insertion
        //This took forever...
        public static List<int> HeapSort(List<int> list)
        {
            //Vector-based heap to add to sequentially
            List<int> heap = new();

            //Create 'heapified' vector
            foreach (var num in list)
            {
                heap.Add(num);
                int i = heap.Count - 1;
                int parent = (i - 1) / 2;

                //Upheap to ensure heap-order property
                while (i > 0 && heap[i] < heap[parent])
                {
                    (heap[i], heap[parent]) = (heap[parent], heap[i]);
                    i = parent;
                    parent = (i - 1) / 2;
                }
            }

            List<int> sortedArray = new();

            //continuously grab the root node (as it is always the smallest), and add it to sortedArray
            while (heap.Count > 0)
            {
                (heap[0], heap[heap.Count - 1]) = (heap[heap.Count - 1], heap[0]);
                sortedArray.Add(heap[heap.Count - 1]);
                heap.RemoveAt(heap.Count - 1);

                //Downheap to ensure heap-order property
                int parent = 0;
                while (true)
                {
                    int leftChild = 2 * parent + 1;
                    int rightChild = 2 * parent + 2;
                    int smallest = parent;

                    if (leftChild < heap.Count && heap[leftChild] < heap[smallest])
                    {
                        smallest = leftChild;
                    }

                    if (rightChild < heap.Count && heap[rightChild] < heap[smallest])
                    {
                        smallest = rightChild;
                    }

                    if (smallest != parent)
                    {
                        (heap[parent], heap[smallest]) = (heap[smallest], heap[parent]);
                        parent = smallest;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return sortedArray;
        }

        //Vector-based version, in-place
        public static void InPlaceHeapSort(List<int> list)
        {
            int start = list.Count / 2;
            int end = list.Count;

            while (end > 1)
            {
                if (start > 0)
                {
                    start -= 1;
                }
                else
                {
                    end -= 1;
                    (list[0], list[end]) = (list[end], list[0]);
                }

                int root = start;
                while (2 * root + 1 < end)
                {
                    int child = 2 * root + 1;
                    if (child + 1 < end && list[child] < list[child + 1])
                    {
                        child++;
                    }

                    if (list[root] < list[child])
                    {
                        (list[root], list[child]) = (list[child], list[root]);

                        root = child;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        //Quicksort
        //In-Place, random pivot
        public static void InPlaceQuickSort(List<int> list, int l, int r)
        {
            if (l >= r)
            {
                return;
            }
            Random rand = new Random();
            int i = rand.Next(l, r+1);
            int x = list[i];
            (int h, int k) = InPlacePartition(list, l, r, x);
            InPlaceQuickSort(list, l, h - 1);
            InPlaceQuickSort(list, k + 1, r);
        }

        //Modified Quicksort
        //In-Place, Median-of-three, sub-problems use Insertion Sort
        public static void ModifiedQuickSort(List<int> list, int l, int r)
        {
            if (l >= r)
            {
                return;
            }

            if (l + 10 > r)
            {
                InsertionSort(list, l, r + 1);
            }
            else
            {

                int i = MedianOfThree(list, l, r);
                int x = list[i];
                (int h, int k) = InPlacePartition(list, l, r, x);
                ModifiedQuickSort(list, l, h - 1);
                ModifiedQuickSort(list, k + 1, r);
            }
        }

        public static (int h, int k) InPlacePartition(List<int> list, int j, int k, int x)
        {
            while (j <= k)
            {
                while (j <= k && list[j] < x)
                {
                    j++;
                }
                while (j <= k && list[k] > x)
                {
                    k--;
                }
                if (j <= k)
                {
                    (list[j], list[k]) = (list[k], list[j]);
                    j++;
                    k--;
                }
            }
            return (k + 1, j - 1);
        }

        public static int MedianOfThree(List<int> list, int l , int r)
        {
            int mid = l + (r - l) / 2;

            //Make sure the 3 are in correct size order
            if (list[l] > list[mid])
            {
                (list[l], list[mid]) = (list[mid], list[l]);
            }
            if (list[l] > list[r])
            {
                (list[l], list[r]) = (list[r], list[l]);
            }
            if (list[mid] > list[r])
            {
                (list[mid], list[r]) = (list[r], list[mid]);
            }

            //Swap A[center] and A[right – 1] so that pivot is at second last position
            (list[mid], list[r - 1]) = (list[r - 1], list[mid]);
            return r - 1;
        }

        //Testing tool to make sure an array is sorted
        public static bool IsSorted(List<int> list)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i] > list[i + 1])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
