# Insert / Remove Behviour

This document describes how multi-dimensional lists should behave
when inserting or removing.

**NOTE**:
This document gives examples only on 2D lists and planes.

---

## Insering

Inserting can be done in two ways:
- At the end of the list
- At a specific position

### Inserting at the end of the list

Inserting at the end of the list can be done with the 
`AddDim()` method.

For example, if we have a list of size 3x3, represented by
the table below, and call the `AddX()` method,

y\x| 0 | 1 | 2 
---|---|---|---
 0 | a | b | c
 1 | d | e | f
 2 | g | h | i

The resulting list will be:

y\x| 0 | 1 | 2 | 3
---|---|---|---|---
 0 | a | b | c |
 1 | d | e | f |
 2 | g | h | i |

### Inserting at a specific position

Inserting at a specific position can be done with the
`InsertAtDim(int dimName)` method.

The behaviour of the parameter should be the same as when 
using the `System.List<T>.Insert(int index)` method,
meaning that when `index` is 0 the element will be inserted 
at the beginning of the list.

For example, if we consider the 3x3 list below and call the
`InsertAtX(1)` method,

y\x| 0 | 1 | 2 
---|---|---|---
 0 | a | b | c
 1 | d | e | f
 2 | g | h | i

the resulting list will be:

y\x| 0 | 1 | 2 | 3
---|---|---|---|---
 0 | a |   | b | c
 1 | d |   | e | f
 2 | g |   | h | i

---

## Insering on Planes / Spaces

Planes and Spaces are a special case of lists. The keep track
of where the origin is located.

### Inserting at the end of a plane or space

Inserting to the end of a plane or space will do the same as
inserting to the end of a list.

### Inserting at a specific position

Inserting at a specific position will do the same as inserting
at a specific position in a list, with the addition of shifting
the origin if the absolute insertion position is smaller than
the current origin offset.

For example, if we consider the 3x3 plane with its origin at 
`(1; 0)` below and call the `InsertAtX(1)` method,

y\x| 0 | 1 | 2 
---|---|---|---
 0 | a | b | c
 1 | d | e | f
 2 | g | h | i

the resulting plane will be:

y\x| 0 | 1 | 2 | 3
---|---|---|---|---
 0 | a |   | b | c
 1 | d |   | e | f
 2 | g |   | h | i

and its origin will be shifted to `(2; 0)`.

---

## Removing

Removing can be done in two ways:
- From the end of the list
- At a specific position

### Removing from the end of the list

Removing from the end of the list can be done with the 
`RemoveDim()` method.

For example, if we consider the 3x3 list below and call the
`RemoveDim()` method,

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
`RemoveAtDim(int dimName)` method.

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

## Removing on Planes / Spaces

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