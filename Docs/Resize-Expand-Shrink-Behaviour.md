# Resize / Expand / Shrink Behaviour

This document describes the behaviour of the `Resize` and `Shrink` methods.

## Resize

Resizes the inner `_items` array to the specified dimensions and 
copies the old array into the new one where possible.

## Expand

Expands the size of the list, enlarging the capacity to the input
size, if the list's capacity is not enough.

**NOTE**: if any of the inputted sizes is smaller than the current 
size of the list in that dimension, the method will throw and IOp 
exception.

## Shrink 

Shrinks the size of the list to the input size.

**NOTE**: if any of the inputted sizes is larger than the current
size of the list in that dimension, the method will throw and IOp
exception.