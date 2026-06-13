# Insert Methods

Inserting can be done in two ways:
- At the end of the list
- At a specific position

## Inserting inside lists

### Inserting at the end of the list

Inserting at the end of the list can be done with the
`Add{Dim}()` method, where the `{Dim}` is the name of the added dimension.

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
`InsertAt{Dim}(int index)` method.

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

## Insering inside Spaces

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
