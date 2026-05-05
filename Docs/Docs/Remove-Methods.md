# Remove Methods

Removing can be done in two ways:
- From the end of the list
- At a specific position

## Removing inside lists

### Removing from the end of the list

Removing from the end of the list can be done with the
`Shrink{Dim}()` method, where the `{Dim}` is the name of the dimension.

For example, if we consider the 3x3 list below and call the
`ShrinkX()` method,

y\x| 0 | 1 | 2 
---|---|---|---
 0 | a | b | c
 1 | d | e | f
 2 | g | h | i 

the resulting list will be:

y\x| 0 | 1 
---|---|---
 0 | a | b
 1 | d | e
 2 | g | h

### Removing at a specific position

Removing at a specific position can be done with the
`RemoveAt{Dim}(int index)` method.

The behaviour of the parameter should be the same as when
using the `System.List<T>.RemoveAt(int index)` method,
meaning that when `index` is 0 the element will be removed
from the beginning of the list.

For example, if we consider the 3x3 list below and call the
`RemoveAtX(1)` method,

y\x| 0 | 1 | 2 
---|---|---|---
 0 | a | b | c
 1 | d | e | f
 2 | g | h | i

the resulting list will be:

y\x| 0 | 1 
---|---|---
 0 | a | c
 1 | d | f
 2 | g | i

---

## Removing inside Spaces

Removing from a plane or space will do the same as removing
from a list with the addition of shifting the origin offset
if the absolute removal position is smaller than the current
origin offset.

### Removing from the end of a plane or space

Removing from the end of a plane or space will do the same as
removing from a list.

### Removing at a specific position

Removing at a specific position will do the same as removing
from a list,

For example, if we consider the 3x3 plane with its origin at
`(1; 0)` below and call the `RemoveAtX(0)` method,

y\x| 0 | 1 | 2 
---|---|---|---
 0 | a | b | c
 1 | d | e | f
 2 | g | h | i

the resulting plane will be:

y\x| 0 | 1
---|---|---
 0 | b | c
 1 | e | f
 2 | h | i

and its origin will be shifted to `(0; 0)`.

However a special case occurs when the absolute removal position
is equal to the current origin offset. In this case the origin
will **not** be shifted.

For example, if we consider the same 3x3 plane with its origin at
`(1; 0)` below and call the `RemoveAtX(1)` method,
the resulting plane will be:

y\x| 0 | 1
---|---|---
 0 | a | c
 1 | d | f
 2 | g | i

and its origin will remain at `(1; 0)`.