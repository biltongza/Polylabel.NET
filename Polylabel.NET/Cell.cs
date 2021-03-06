﻿namespace Polylabel.NET
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Represents a portion of an area covering a polygon
    /// </summary>
    public struct Cell : IComparable<Cell>
    {
        /// <summary>
        /// Square root of 2
        /// </summary>
        public static readonly double SquareRootOf2 = Math.Sqrt(2d);
        
        public Vector<double> Center { get; }
        
        /// <summary>
        /// Half the cell size
        /// </summary>
        public double HalfCellSize { get; }
        
        /// <summary>
        /// Distance from cell center to polygon
        /// </summary>
        public double DistanceFromCenterToPolygon { get; }

        /// <summary>
        /// Max distance to polygon wihin cell 
        /// </summary>
        public double MaxDistanceToPolygonWithinCell { get; }

        public double Fitness { get; }

        public double MaxFitness { get; }

        /// <summary>
        /// Initializes a new instance of the Cell class.
        /// </summary>
        /// <param name="centerX">The cell center X coordinate.</param>
        /// <param name="centerY">The cell center Y coordinate.</param>
        /// <param name="halfCellSize">Half the cell size.</param>
        /// <param name="polygon">The polygon.</param>
        public Cell(Vector<double> center, double halfCellSize, Vector<double>[][] polygon, Func<Vector<double>, double, double> fitnessFunction)
        {
            this.Center = center;
            this.HalfCellSize = halfCellSize;
            this.DistanceFromCenterToPolygon = Polylabel.GetDistanceFromPointToPolygonOutline(this.Center, polygon);
            this.MaxDistanceToPolygonWithinCell = this.DistanceFromCenterToPolygon + this.HalfCellSize * SquareRootOf2;
            this.Fitness = fitnessFunction(this.Center, this.DistanceFromCenterToPolygon);
            this.MaxFitness = fitnessFunction(this.Center, this.MaxDistanceToPolygonWithinCell);
        }

        /// <summary>
        /// Compares this Cell's max distance to polygon to another's.
        /// </summary>
        /// <param name="other">The other cell to compare to.</param>
        /// <returns>The difference between the two distances.</returns>
        public int CompareTo(Cell other)
        {
            return this.MaxFitness.CompareTo(other.MaxFitness);
        }
    }
}
