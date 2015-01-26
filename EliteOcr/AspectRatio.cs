using System;

namespace EliteTrader.EliteOcr
{
    public struct AspectRatio : IEquatable<AspectRatio>
    {
        private readonly int _width;
        private readonly int _height;

        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public AspectRatio(int width, int height)
        {
            _width = width;
            _height = height;
        }

        private decimal Ratio
        {
            get { return ((decimal) Width)/Height; }
        }

        public bool Equals(AspectRatio other)
        {
            return Ratio == other.Ratio;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AspectRatio && Equals((AspectRatio) obj);
        }

        public override int GetHashCode()
        {
            return Ratio.GetHashCode();
        }

        public static bool operator ==(AspectRatio left, AspectRatio right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AspectRatio left, AspectRatio right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}", Width, Height);
        }
    }
}
