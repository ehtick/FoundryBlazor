
using BlazorThreeJS.Maths;
using System.Linq.Dynamic.Core.CustomTypeProviders;

namespace FoundryBlazor.Extensions;


public static class Vector3MathExtensions
{

    //add 2 vectors to create third
    public static Vector3 Add(this Vector3 a, Vector3 b)
    {
        return new Vector3
        {
            X = a.X + b.X,
            Y = a.Y + b.Y,
            Z = a.Z + b.Z
        };
    }

    //subtract 2 vectors to create third
    public static Vector3 Subtract(this Vector3 a, Vector3 b)
    {
        return new Vector3
        {
            X = a.X - b.X,
            Y = a.Y - b.Y,
            Z = a.Z - b.Z
        };
    }

    //center point of 2 vectors
    public static Vector3 Center(this Vector3 a, Vector3 b)
    {
        return new Vector3
        {
            X = (a.X + b.X) / 2,
            Y = (a.Y + b.Y) / 2,
            Z = (a.Z + b.Z) / 2
        };
    }
    //multiply vector by scalar
    public static Vector3 Multiply(this Vector3 a, double scalar)
    {
        return new Vector3
        {
            X = a.X * scalar,
            Y = a.Y * scalar,
            Z = a.Z * scalar
        };
    }

    // center of 

    public static Vector3 BoundingBox(this Vector3 a, Vector3 b)
    {
        return new Vector3
        {
            X = Math.Abs(a.X - b.X),
            Y = Math.Abs(a.Y - b.Y),
            Z = Math.Abs(a.Z - b.Z)
        };
    }


}

