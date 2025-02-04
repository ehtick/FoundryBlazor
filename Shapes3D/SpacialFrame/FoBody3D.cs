using System;
using System.Numerics;

// Example FoBody3D class to demonstrate usage
public class FoBody3D
{
    protected Vector3D position = new();
    protected Vector3D scale = new(1, 1, 1);
    protected Vector3D rotation = new();
    protected Vector3D pinPoint = new();
    protected Matrix3D? _matrix;

    public double X { get => position.X; set { position.X = value; Smash(); } }
    public double Y { get => position.Y; set { position.Y = value; Smash(); } }
    public double Z { get => position.Z; set { position.Z = value; Smash(); } }

    public double RotationX { get => rotation.X; set { rotation.X = value; Smash(); } }
    public double RotationY { get => rotation.Y; set { rotation.Y = value; Smash(); } }
    public double RotationZ { get => rotation.Z; set { rotation.Z = value; Smash(); } }

    protected void Smash()
    {
        _matrix = Matrix3D.SmashMatrix(_matrix);
    }

    public Matrix3D GetMatrix()
    {
        if (_matrix == null)
        {
            _matrix = Matrix3D.NewMatrix();
            _matrix.AppendTransform(
                position.X, position.Y, position.Z,
                scale.X, scale.Y, scale.Z,
                rotation.X, rotation.Y, rotation.Z,
                pinPoint.X, pinPoint.Y, pinPoint.Z
            );
        }
        return _matrix;
    }
}