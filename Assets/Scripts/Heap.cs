using System;
using System.Linq;
using System.Text;

// @author Egor Grishechko https://egorikas.com/max-and-min-heap-implementation-with-csharp/
public class Heap<T> where T : IComparable {
    private const int DefaultSize = 10;
    private T[] _heap;
    private int _size;
    private bool _maxHeap;
    
    public Heap(int size = DefaultSize, bool maxHeap = true) {
        _heap = new T[size];
        _maxHeap = maxHeap;
    }

    private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
    private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
    private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

    private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < _size;
    private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < _size;
    private bool IsRoot(int elementIndex) => elementIndex == 0;

    private T GetLeftChild(int elementIndex) => _heap[GetLeftChildIndex(elementIndex)];
    private T GetRightChild(int elementIndex) => _heap[GetRightChildIndex(elementIndex)];
    private T GetParent(int elementIndex) => _heap[GetParentIndex(elementIndex)];

    private void Swap(int firstIndex, int secondIndex) {
        (_heap[firstIndex], _heap[secondIndex]) = (_heap[secondIndex], _heap[firstIndex]);
    }

    public bool IsEmpty() {
        return _size == 0;
    }

    public int Size() {
        return _size;
    }

    public T Peek() {
        if (_size <= 0)
            throw new IndexOutOfRangeException();

        return _heap[0];
    }

    public T Pop() {
        
        if (_size <= 0)
            throw new IndexOutOfRangeException("Heap empty");

        T result = _heap[0];

        _heap[0] = _heap[_size - 1];
        _size--;

        ReCalculateDown();
        
        return result;
    }

    public void Add(T element) {
        if (_size == _heap.Length)
            DoubleHeap();

        _heap[_size] = element;
        _size++;
        ReCalculateUp();
    }

    private void ReCalculateDown() {
        int index = 0;
        while (HasLeftChild(index)) {
            var biggerIndex = GetLeftChildIndex(index);
            if (HasRightChild(index) &&
                Compare(GetRightChild(index), GetLeftChild(index)) > 0) {
                biggerIndex = GetRightChildIndex(index);
            }

            if (_heap[biggerIndex] != null && _heap[index] != null 
                && Compare(_heap[biggerIndex], _heap[index]) < 0) {
                break;
            }

            Swap(biggerIndex, index);
            index = biggerIndex;
        }
    }

    private void ReCalculateUp() {
        var index = _size - 1;
        while (!IsRoot(index) && 
               Compare(_heap[index],  GetParent(index)) > 0)
        {
            int parentIndex = GetParentIndex(index);
            Swap(parentIndex, index);
            index = parentIndex;
        }
    }

    private int Compare(T element, T contender) {
        int val = element.CompareTo(contender);
        val = _maxHeap ? val : -val;
        
        return val;
    }

    public override string ToString() {
        StringBuilder sb = new StringBuilder("[");
        
        _heap.ToList().ForEach(i => {
            if (sb.Length > 1) // if before first element in heap, don't add ", "
                sb.Append(", ");
            
            sb.Append(i);
        });
        sb.Append("]");
        return sb.ToString();
    }
    
    private void DoubleHeap()
    {
        var copy = new T[_heap.Length * 2];
        for (int i = 0; i < _heap.Length; i++)
        {
            copy[i] = _heap[i];
        }
        _heap = copy;
    }
}