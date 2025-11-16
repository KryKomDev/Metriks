<h1 align="center">Metriks</h1>

<p align="center">A multidimensional dynamic array library for C#.</p>

<div align="center">
    <p>
        <img src="https://img.shields.io/github/license/KryKomDev/Metriks?style=for-the-badge&amp;labelColor=%235F6473&amp;color=%23F2A0A0" alt="GitHub License" />
        <a href="https://www.nuget.org/packages/Metriks"><img src="https://img.shields.io/nuget/v/Metriks?color=F0CA95&amp;style=for-the-badge&amp;labelColor=5F6473" alt="NuGet" /></a>
        <img src="https://img.shields.io/nuget/dt/Metriks?color=E3ED8A&amp;style=for-the-badge&amp;labelColor=5F6473" alt="NuGet Downloads" />
        <img src="https://img.shields.io/github/actions/workflow/status/KryKomDev/Metriks/build.yml?style=for-the-badge&amp;labelColor=%235F6473&amp;color=%2395EC7D" alt="GitHub Actions Workflow Status" />
        <img src="https://img.shields.io/badge/.NET-Standard2.0-7ACFDC?style=for-the-badge&amp;labelColor=5F6473" alt=".NET Standard" />
    </p>
</div>

## Features

The Metriks library contains 2D and 3D dynamic array classes and interfaces.
It also contains some extensions for working with conventional arrays like
length getters or higher order LINQ.

### Lists

Metriks provides 2D, 3D and 4D dynamic array classes that implement their own
multidimensional enumerable interfaces.

Metriks also provides a `ListND` class that can be used to store any number of
dimensions. It however can be slower than the other classes as it is highly 
generalized.

### Fixed origin lists

Metriks also provides fixed origin 2D, 3D and 4D dynamic array classes that can
be used to store data in a rectangular grid with a fixed origin.