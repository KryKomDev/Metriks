
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Metriks;

public class ListND<T> : IEnumerable<T>, IDisposable {

    private const int INITIAL_CAPACITY = 4;
    private const double GROWTH_RATIO = Math.E;
    
    private readonly int _dimensions;
    private T[] _elements;
    private readonly int[] _sizes;
    private int[] _capacities;
    
    /// <summary>
    /// Returns the number of the list's dimensions.
    /// </summary>
    public int Dimensions => _dimensions;

    /// <summary>
    /// Returns the total count of elements contained in the list.
    /// </summary>
    public long GetCount() {
        long p = 1;
        for (int i = 0; i < _dimensions; i++) {
            p *= _sizes[i];
        }

        return p;
    }

    /// <summary>
    /// Returns the total capacity of the list.
    /// </summary>
    public long GetCapacity() {
        long p = 1;
        for (int i = 0; i < _dimensions; i++) {
            p *= _capacities[i];
        }

        return p;
    }
    
    /// <summary>
    /// Returns the total count of elements contained in the list.
    /// </summary>
    public long Count => GetCount();
    
    /// <summary>
    /// Returns the total count of elements contained in the list.
    /// </summary>
    public long Length => GetCount();
    
    /// <summary>
    /// Returns the total capacity of the list.
    /// </summary>
    public long Capacity => GetCapacity();

    /// <summary>
    /// Creates a new dynamic N-Dimensional array.
    /// </summary>
    /// <param name="dimensions">the number of dimensions contained by the list</param>
    public ListND(int dimensions) {
        _dimensions = dimensions;
        _elements = new T[(int)Math.Pow(INITIAL_CAPACITY, dimensions)];
        _sizes = new int[dimensions];
        _capacities = new int[dimensions];
        Extensions.Fill(_capacities, INITIAL_CAPACITY);
    }

    /// <summary>
    /// Creates a new dynamic N-Dimensional array.
    /// </summary>
    /// <param name="dimensions">the number of dimensions contained by the list</param>
    /// <param name="capacity">the initial capacity for every dimension</param>
    public ListND(int dimensions, int capacity) {
        _dimensions = dimensions;
        _elements = new T[(int)Math.Pow(capacity, dimensions)];
        _sizes = new int[dimensions];
        _capacities = new int[dimensions];
        Extensions.Fill(_capacities, capacity);
    }

    /// <summary>
    /// Gets or sets the element at the specified position.
    /// </summary>
    /// <param name="indexes">array of indexes for every dimension</param>
    /// <exception cref="ArgumentException">the number of indexes does not match the number of dimensions</exception>
    public T this[params int[] indexes] {
        get {
            ThrowHelper.ThrowIf(indexes.Length != _dimensions, 
                new ArgumentException("Must specify all dimensions to access."));
            
            return _elements[GetOffset(indexes)];
        }
        set {
            ThrowHelper.ThrowIf(indexes.Length != _dimensions, 
                new ArgumentException("Must specify all dimensions to access."));
            
            _elements[GetOffset(indexes)] = value;
        }
    }

    /// <summary>
    /// Returns the offset in the main <see cref="_elements"/> array for the specified indexes.
    /// </summary>
    private long GetOffset(int[] indexes) {
        long offset = 0;
        long product = 1;
        for (int i = _dimensions - 1; i >= 0; i--) {
            ThrowHelper.ThrowIf(indexes[i] < 0 || indexes[i] >= _sizes[i], 
                new IndexOutOfRangeException($"Index for dimension {i} is out of range."));
                
            offset += product * indexes[i]; 
            product *= _capacities[i];
        }
        
        return offset;
    }

    public void Resize(int dimension, int size) {
        ThrowHelper.ThrowIf(dimension < 0 || dimension >= _dimensions, new ArgumentException("Invalid dimension."));
        ThrowHelper.ThrowIfNegative(size, new ArgumentException("Invalid size."));
        
        int[] newCapacities = new int[_dimensions];
        Array.Copy(_capacities, newCapacities, _dimensions);

        if (_sizes[dimension] > _capacities[dimension]) {
            _capacities[dimension] = size;
        }
        
        long capacity = 1;
        for (int i = 0; i < _dimensions; i++) {
            capacity *= _capacities[i];
        }
        
        var newElements = new T[capacity];
        
        for (int i = 0; i < _dimensions; i++) {
            
        }

        _elements = newElements;
        _capacities = newCapacities;
        _sizes[dimension] = _sizes[dimension] > _capacities[dimension] ? _capacities[dimension] : _sizes[dimension];
    }
    
    public IEnumerator<T> GetEnumerator() {
        throw new ImplementItYourselfException();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
    
    public void Dispose() {
        throw new NotImplementedException();
    }
}