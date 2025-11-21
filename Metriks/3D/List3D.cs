using System.Collections;

namespace Metriks;

public class List3D<T> : IList3D<T>, ICollection3D, IReadOnlyList3D<T> {
    
    private const int INITIAL_CAPACITY = 4;
    private const float GROWTH_FACTOR = 2f;
    
    // contains the elements
    // indexing: [x][y][z]
    // if you think this is not good, I say IT IS THE OPTIMAL WAY for memory and performance
    private T[][][] _items;

    private int _xSize;
    private int _ySize;
    private int _zSize;

    private int _xCapacity;
    private int _yCapacity;
    private int _zCapacity;
    
    public int Count => _xSize * _ySize * _zSize;
    
    public int XCount => _xSize;
    public int YCount => _ySize;
    public int ZCount => _zSize;
    
    public int XCapacity => _xCapacity;
    public int YCapacity => _yCapacity;
    public int ZCapacity => _zCapacity;
    
    public T this[int x, int y, int z] {
        
        [Pure]
        get {
            if (x < 0 || x > _xSize) throw new IndexOutOfRangeException();
            if (y < 0 || y > _ySize) throw new IndexOutOfRangeException();
            if (z < 0 || z > _zSize) throw new IndexOutOfRangeException();

            return _items[x][y][z];
        }
        
        set {
            if (x < 0 || x > _xSize) throw new IndexOutOfRangeException();
            if (y < 0 || y > _ySize) throw new IndexOutOfRangeException();
            if (z < 0 || z > _zSize) throw new IndexOutOfRangeException();

            _items[x][y][z] = value;
        }
    }

    #if NETSTANDARD_2_1_OR_GREATER || NET5_0_OR_GREATER
    
    public T this[Index x, Index y, Index z] {
        
        [Pure]
        get {
            int xo = x.GetOffset(_xSize);
            int yo = y.GetOffset(_ySize);
            int zo = z.GetOffset(_zSize);
            
            if (xo < 0 || xo > _xSize) throw new IndexOutOfRangeException();
            if (yo < 0 || yo > _ySize) throw new IndexOutOfRangeException();
            if (zo < 0 || zo > _zSize) throw new IndexOutOfRangeException();
            
            return _items[xo][yo][zo];
        }

        set {
            int xo = x.GetOffset(_xSize);
            int yo = y.GetOffset(_ySize);
            int zo = z.GetOffset(_zSize);
            
            if (xo < 0 || xo > _xSize) throw new IndexOutOfRangeException();
            if (yo < 0 || yo > _ySize) throw new IndexOutOfRangeException();
            if (zo < 0 || zo > _zSize) throw new IndexOutOfRangeException();
            
            _items[xo][yo][zo] = value;
        }
    }

    #endif
    
    public List3D() : this(INITIAL_CAPACITY) { }

    public List3D(int capacity) : this(capacity, capacity, capacity) { }
    
    public List3D(int xCapacity, int yCapacity, int zCapacity) {
        _xCapacity = xCapacity;
        _yCapacity = yCapacity;
        _zCapacity = zCapacity;
        
        _xSize = 0;
        _ySize = 0;
        _zSize = 0;
        
        _items = new T[_xCapacity][][];
    }

    public List3D(T[,,] arr) : this(arr.Len0, arr.Len1, arr.Len2) {
        _xSize = arr.Len0;
        _ySize = arr.Len1;
        _zSize = arr.Len2;
        
        for (int x = 0; x < _xSize; x++) {
            _items[x] = new T[_ySize][];
            
            for (int y = 0; y < _ySize; y++) {
                _items[x][y] = new T[_zSize];
                
                for (int z = 0; z < _zSize; z++) {
                    _items[x][y][z] = arr[x, y, z];
                }
            }
        }
    }
    
    public IEnumerator<IEnumerable<IEnumerable<T>>> GetEnumerator() {
        for (int x = 0; x < _xSize; x++) {
            yield return _items[x];
        }
    }

    IEnumerator<IEnumerable<IEnumerable>> IEnumerable3D.GetEnumerator() => GetEnumerator();

    public void CopyTo(Array array, Point3D index) {
        if (array.Len0 < _xSize + index.X) throw new ArgumentException("Destination array is not large enough in x dimension.");
        if (array.Len1 < _ySize + index.Y) throw new ArgumentException("Destination array is not large enough in y dimension.");
        if (array.Len2 < _zSize + index.Z) throw new ArgumentException("Destination array is not large enough in z dimension.");

        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                for (int z = 0; z < _zSize; z++) {
                    array.SetValue(_items[x][y][z], x + index.X, y + index.Y, z + index.Z);
                }
            }
        }
    }

    public void CopyTo(T[,,] array, Point3D index) {
        if (array.Len0 < _xSize + index.X) throw new ArgumentException("Destination array is not large enough in x dimension.");
        if (array.Len1 < _ySize + index.Y) throw new ArgumentException("Destination array is not large enough in y dimension.");
        if (array.Len2 < _zSize + index.Z) throw new ArgumentException("Destination array is not large enough in z dimension.");

        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                for (int z = 0; z < _zSize; z++) {
                    array[x + index.X, y + index.Y, z + index.Z] = _items[x][y][z];
                }
            }
        }
    }

    public bool IsReadOnly => false;
    
    public void Clear() {
        _xCapacity = INITIAL_CAPACITY;
        _yCapacity = INITIAL_CAPACITY;
        _zCapacity = INITIAL_CAPACITY;
        
        _xSize = 0;
        _ySize = 0;
        _zSize = 0;
        
        _items = new T[_xCapacity][][];
    }
    
    public void AddX() {
        if (_xSize + 1 > _xCapacity) {
            _xCapacity = (int)(_xCapacity * GROWTH_FACTOR);
            
            var newItems = new T[_xCapacity][][];
            
            Array.Copy(_items, newItems, _xSize);
        }

        // create a new x-row
        _items[_xSize] = new T[_yCapacity][];

        // set up the new x-row
        for (int y = 0; y < _ySize; y++) {
            _items[_xSize][y] = new T[_zCapacity];
        }
        
        _xSize++;
    }
    
    public void AddY() {
        if (_ySize + 1 > _yCapacity) {
            _yCapacity = (int)(_yCapacity * GROWTH_FACTOR);

            for (int x = 0; x < _xSize; x++) {
                var newArray = new T[_yCapacity][];
                
                Array.Copy(_items[x], newArray, _ySize);
                
                _items[x] = newArray;
            }
        }

        // create a new y-row
        _items[_xSize - 1][_ySize] = new T[_zCapacity];
        
        // set up the new y-row
        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize ; y++) {
                _items[x][y] = new T[_zCapacity];
            }
        }

        _ySize++;
    }
    
    public void AddZ() {
        if (_zSize + 1 > _zCapacity) {
            _zCapacity = (int)(_zCapacity * GROWTH_FACTOR);

            for (int x = 0; x < _xSize; x++) {
                for (int y = 0; y < _ySize; y++) {
                    var newArray = new T[_zCapacity];
                    
                    Array.Copy(_items[x][y], newArray, _zSize);
                    
                    _items[x][y] = newArray;
                }
            }
        }
        
        // z-row cannot be created as it is a 1D array,
        // so we just increment the size
        _zSize++;
    }
    
    public void ShrinkX() {
        if (_xSize == 0)
            throw new InvalidOperationException("Cannot shrink a List3D with x-size = 0.");
        
        _xSize--;

        _items[_xSize] = null!;
    }
    
    public void ShrinkY() {
        if (_ySize == 0)
            throw new InvalidOperationException("Cannot shrink a List3D with y-size = 0.");

        _ySize--;

        for (int x = 0; x < _xSize; x++) {
            _items[x][_ySize] = null!;
        }
    }
    
    public void ShrinkZ() {
        if (_zSize == 0)
            throw new InvalidOperationException("Cannot shrink a List3D with z-size = 0.");
        
        _zSize--;

        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                _items[x][y][_zSize] = default!;
            }
        }
    }
    
    public bool Contains(T value) {
        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                for (int z = 0; z < _zSize; z++) {
                    if (EqualityComparer<T>.Default.Equals(_items[x][y][z], value)) return true;
                }
            }
        }
        
        return false;
    }
    
    public bool ContainsAtX(int x, T value) {
        for (int y = 0; y < _ySize; y++) {
            for (int z = 0; z < _zSize; z++) {
                if (EqualityComparer<T>.Default.Equals(_items[x][y][z], value)) return true;
            }
        }
        
        return false;
    }
    
    public bool ContainsAtY(int y, T value) {
        for (int x = 0; x < _ySize; x++) {
            for (int z = 0; z < _zSize; z++) {
                if (EqualityComparer<T>.Default.Equals(_items[x][y][z], value)) return true;
            }
        }
        
        return false;
    }
    
    public bool ContainsAtZ(int z, T value) {
        for (int x = 0; x < _ySize; x++) {
            for (int y = 0; y < _zSize; y++) {
                if (EqualityComparer<T>.Default.Equals(_items[x][y][z], value)) return true;
            }
        }
        
        return false;
    }

    public void InsertAtX(int x) {
        if (x < 0 || x > _xSize) 
            throw new IndexOutOfRangeException();
        
        if (_xSize + 1 > _xCapacity) {
            _xCapacity = (int)(_xCapacity * GROWTH_FACTOR);
        }

        var newItems = new T[_xCapacity][][];
        
        Array.Copy(_items, newItems, x);
        Array.Copy(_items, x, newItems, x + 1, _xSize - x);
        
        newItems[x] = new T[_yCapacity][];

        for (int y = 0; y < _ySize; y++) {
            newItems[x][y] = new T[_zCapacity];
        }

        _items = newItems;
        _xSize++;
    }
    
    public void InsertAtY(int y) {
        if (y < 0 || y > _ySize) 
            throw new IndexOutOfRangeException();
        
        if (_ySize + 1 > _yCapacity) {
            _yCapacity = (int)(_yCapacity * GROWTH_FACTOR);
        }

        for (int x = 0; x < _xSize; x++) {
            var newArray = new T[_yCapacity][];
            
            Array.Copy(_items[x], newArray, y);
            Array.Copy(_items[x], y, newArray, y + 1, _ySize - y);
            
            newArray[y] = new T[_zCapacity];
            
            _items[x] = newArray;
        }
        
        _ySize++;
    }
    
    public void InsertAtZ(int z) {
        if (z < 0 || z > _zSize) 
            throw new IndexOutOfRangeException();
        
        if (_zSize + 1 > _zCapacity) {
            _zCapacity = (int)(_zCapacity * GROWTH_FACTOR);
        }

        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                var newArray = new T[_zCapacity];
                
                Array.Copy(_items[x][y], newArray, z);
                Array.Copy(_items[x][y], z, newArray, z + 1, _zSize - z);

                newArray[z] = default!;
                
                _items[x][y] = newArray;
            }
        }
        
        _zSize++;
    }
    
    public void RemoveAtX(int x) {
        if (x < 0 || x >= _xSize) throw new IndexOutOfRangeException();
        
        var newItems = new T[_xCapacity][][];
        Array.Copy(_items, newItems, x);
        Array.Copy(_items, x + 1, newItems, x, _xSize - x);
        
        _items = newItems;
        _xSize--;
    }
    
    public void RemoveAtY(int y) {
        if (y < 0 || y >= _ySize) throw new IndexOutOfRangeException();

        for (int x = 0; x < _xSize; x++) {
            var newArray = new T[_yCapacity][];
            
            Array.Copy(_items[x], newArray, y);
            Array.Copy(_items[x], y + 1, newArray, y, _ySize - y);
            
            _items[x] = newArray;
        }
        
        _ySize--;
    }
    
    public void RemoveAtZ(int z) {
        if (z < 0 || z >= _zSize) throw new IndexOutOfRangeException();

        for (int x = 0; x < _xSize; x++) {
            for (int y = 0; y < _ySize; y++) {
                var newArray = new T[_zCapacity];
                
                Array.Copy(_items[x][y], newArray, z);
                Array.Copy(_items[x][y], z + 1, newArray, z, _zSize - z);
                
                _items[x][y] = newArray;
            }
        }
        
        _zSize--;
    }
}