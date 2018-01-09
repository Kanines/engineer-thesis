using System;
using OpenTK;

namespace Visualisation
{
    public class Camera
    {
        readonly Vector3 _startPosition;
        Vector3 _position;
        Angle _pitch;
        Angle _yaw;

        public Camera(Vector3 startPosition)
        {
            _startPosition = startPosition;
            ResetCamera();
        }

        public void ResetCamera()
        {
            _position = _startPosition;
            _pitch = new Angle(-90, Angle.Type.Degrees);
            _yaw = new Angle(0, Angle.Type.Degrees);
        }

        public Matrix4 GetViewMatrix()
        {
            float sinPitch = (float) Math.Sin(_pitch.Radians);
            float cosPitch = (float) Math.Cos(_pitch.Radians);
            float sinYaw = (float) Math.Sin(_yaw.Radians);
            float cosYaw = (float) Math.Cos(_yaw.Radians);

            Vector3 xaxis = new Vector3(cosYaw, 0, -sinYaw);
            Vector3 yaxis = new Vector3(sinYaw*sinPitch, cosPitch, cosYaw*sinPitch);
            Vector3 zaxis = new Vector3(sinYaw*cosPitch, -sinPitch, cosPitch*cosYaw);

            Matrix4 vewMatrix = new Matrix4(
                new Vector4(xaxis.X, yaxis.X, zaxis.X, 0),
                new Vector4(xaxis.Y, yaxis.Y, zaxis.Y, 0),
                new Vector4(xaxis.Z, yaxis.Z, zaxis.Z, 0),
                new Vector4(-Vector3.Dot(xaxis, _position), -Vector3.Dot(yaxis, _position),
                    -Vector3.Dot(zaxis, _position), 1));
            return vewMatrix;
        }

        public void Move(float x, float y, float z)
        {
            Vector3 rotOffset = new Vector3
                (
                (float) (y*Math.Sin(_yaw.Radians) + x*Math.Cos(_yaw.Radians)),
                z,
                (float) (y*Math.Cos(_yaw.Radians) - x*Math.Sin(_yaw.Radians))
                );

            _position += rotOffset;
        }

        public void AddRotation(float x, float y = 0)
        {
            Angle dimX = new Angle(x, Angle.Type.Degrees);
            Angle dimY = new Angle(y, Angle.Type.Degrees);

            _pitch += dimX;
            _yaw += dimY;
        }

        public Vector3 Position
        {
            get { return _position; }
        }

        public float Zoom
        {
            get { return _position.Y/_startPosition.Y; }
        }
    }
}