using System;
using System.Drawing;
using System.Linq;

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

        public static Border GetBorders(Bitmap bmp, Color borderColor)
        {
            return GetBorders(bmp, new Color[] { borderColor });
        }

        public static Border GetBorders(Bitmap bmp, Color[] borderColorList)
        {
            int[] pixel = MyExtensions.ToArgbArray(bmp);
            Border b = new Border();
            int width = bmp.Width, height = bmp.Height;
            int[] borderColorListInt = borderColorList.Select<Color, int>(p => p.ToArgb()).ToArray();

            Func<int, int, int> getPixel = delegate (int x, int y)
            {
                return pixel[y * width + x];
            };

            // returns whether a bitmap column is empty (empty means all is border color)
            Func<int, bool> columnIsEmpty = delegate (int column)
            {
                // for each row in the column
                for (int row = 0; row < height; ++row)
                {
                    // is the pixel black?
                    if (!borderColorListInt.Contains(getPixel(column, row)))
                    {
                        // found. column is not empty
                        return false;
                    }
                }

                // column is empty
                return true;
            };

            // returns whether a bitmap row is empty (empty means all is border color)
            Func<int, bool> rowIsEmpty = delegate (int row)
            {
                // for each column in the row
                for (int column = 0; column < width; ++column)
                {
                    // is the pixel black?
                    if (!borderColorListInt.Contains(getPixel(column, row)))
                    {
                        // found. row is not empty
                        return false;
                    }
                }

                // row is empty
                return true;
            };

            for (b.Left = 0; b.Left < width; ++b.Left)
            {
                if (!columnIsEmpty(b.Left)) break;
            }
            for (b.Right = width - 1; b.Right >= 0; --b.Right)
            {
                if (!columnIsEmpty(b.Right)) break;
            }
            for (b.Top = 0; b.Top < height; ++b.Top)
            {
                if (!rowIsEmpty(b.Top)) break;
            }
            for (b.Bottom = height - 1; b.Bottom >= 0; --b.Bottom)
            {
                if (!rowIsEmpty(b.Bottom)) break;
            }

            return b;
        }
    }
}
