using System;

namespace Visualisation
{
    public class Angle
    {
        public Angle()
        {
        }

        public Angle(double input, Type angleType = Type.Radians)
        {
            switch (angleType)
            {
                case Type.Degrees:
                    Degrees = input;
                    break;
                case Type.Radians:
                    Radians = input;
                    break;
            }
        }

        double _degrees;

        public double Degrees
        {
            get { return _degrees; }
            set
            {
                _degrees = value;
                _radians = ToRadians(value);
                UpdateFixedAngles();
            }
        }

        double _radians;

        public double Radians
        {
            get { return _radians; }
            set
            {
                _radians = value;
                _degrees = ToDegrees(value);
                UpdateFixedAngles();
            }
        }

        double _radians2Pi;

        public enum Type
        {
            Radians,
            Degrees
        }

        public static double ToRadians(double val)
        {
            return val/(180/Math.PI);
        }

        public static double ToDegrees(double val)
        {
            return val*(180/Math.PI);
        }

        public static double FixAngle(double val, Type type)
        {
            switch (type)
            {
                case Type.Radians:
                    if (val < 0)
                        return 2*Math.PI - Math.Abs(val)%(2*Math.PI);
                    if (val > 2*Math.PI)
                        return val%(2*Math.PI);
                    return val;
                case Type.Degrees:
                    if (val < 0)
                        return 360 - Math.Abs(val)%360;
                    if (val > 360)
                        return val%360;
                    return val;
            }
            return -1;
        }

        void UpdateFixedAngles()
        {
            _radians2Pi = FixAngle(_radians, Type.Radians);
        }

        public void FixAngles()
        {
            UpdateFixedAngles();
            Radians = _radians2Pi;
        }

        public static Angle operator +(Angle a1, Angle a2)
        {
            return new Angle(a1.Radians + a2.Radians);
        }

        public static Angle operator -(Angle a1, Angle a2)
        {
            return new Angle(a1.Radians - a2.Radians);
        }

        public static implicit operator double(Angle angleobj)
        {
            return angleobj.Radians;
        }

        public override string ToString()
        {
            return Degrees + " degrees";
        }
    }
}