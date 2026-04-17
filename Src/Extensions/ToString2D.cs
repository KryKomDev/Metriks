// Metriks
// Copyright (c) KryKom & ZlomenyMesic 2025

using System.Text;

namespace Metriks;

public static class ToString2D {
    
    /// <param name="list">The two-dimensional enumerable to be printed.</param>
    /// <typeparam name="T">The type of elements in the two-dimensional enumerable.</typeparam>
    extension<T>(IEnumerable2D<T> list) {
        
        /// <summary>
        /// Outputs the contents of a two-dimensional enumerable to the console in a formatted structure.
        /// </summary>
        public void Write() {
            Console.WriteLine("[");
            Console.WriteLine(string.Join(",\n", list.Select(l => "   [ " + string.Join(", ", l) + " ]")));
            Console.WriteLine("]");
        }

        /// <summary>
        /// Returns a string representation of the contents of a two-dimensional enumerable in a formatted structure.
        /// </summary>
        public string ToCollectionString() {
            var sb = new StringBuilder();
            sb.AppendLine("[");
            sb.AppendLine(string.Join(",\n", list.Select(l => "   [ " + string.Join(", ", l) + " ]")));
            sb.Append(']');
            return sb.ToString();
        }
    }
}