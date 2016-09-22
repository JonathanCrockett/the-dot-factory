using System;
using System.Drawing;

namespace TheDotFactory
{
    [Serializable]
    public struct Border
    {
        private int _top;
        private int _left;
        private int _right;
        private int _bottom;

        public static readonly Border Empty = new Border(0);
        
        public Border(int all)
        {
            _top = _left = _right = _bottom = all;
        }
        
        public Border(int left, int top, int right, int bottom)
        {
            _top = top;
            _left = left;
            _right = right;
            _bottom = bottom;
        }
        
        public int All
        {
            get { return _top == _left && _top == _right && _top == _bottom? _top : -1; }
            set { _top = _left = _right = _bottom = value; }
        }
        
        public int Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }
            
        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }
            
        public int Right
        {
            get { return _right; }
            set {  _right = value; }
        }
            
        public int Top
        {
            get { return _top; }
            set  { _top = value; }
        }
        
        public int Horizontal { get { return Left + Right; } }

        public int Vertical { get { return Top + Bottom; } }

        public Size Size { get { return new Size(Horizontal, Vertical); } }

        public override bool Equals(object other)
        {
            if (other is Border)
            {
                return ((Border)other) == this;
            }
            return false;
        }

        public static bool operator ==(Border p1, Border p2)
        {
            return p1.Left == p2.Left 
                && p1.Top == p2.Top 
                && p1.Right == p2.Right 
                && p1.Bottom == p2.Bottom;
        }

        public static bool operator !=(Border p1, Border p2)
        {
            return p1.Left != p2.Left
                || p1.Top != p2.Top
                || p1.Right != p2.Right
                || p1.Bottom != p2.Bottom;
    }

        public override string ToString()
        {
            return "{Left=" + Left.ToString() + ",Top=" + Top.ToString() + ",Right=" + Right.ToString() + ",Bottom=" + Bottom.ToString() + "}";
        }

        public override int GetHashCode()
        {
            return Left/4 + Top/4 + Bottom/4 + Right/4;
        }

        public bool IsValid()
        {
            return _bottom >= 0 && _top >= 0 && _left >= 0 && _right >= 0;
        }
    }
}
