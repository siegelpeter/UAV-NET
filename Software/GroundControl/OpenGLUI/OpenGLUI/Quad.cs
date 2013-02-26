// Quad.cs
//
// Copyright (c) 2008 Scott Ellington
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//

using System;

namespace SalmonViewer
{
	public struct Quad
	{
		public int Vertex1;
		public int Vertex2;
		public int Vertex3;
		public int Vertex4;

		public Quad ( int v1, int v2, int v3, int v4 )
		{
			Vertex1 = v1;
			Vertex2 = v2;
			Vertex3 = v3;
			Vertex4 = v4;
		}

		public override string ToString ()
		{
			return String.Format ( "v1: {0} v2: {1} v3: {2} v4: {3}", Vertex1, Vertex2, Vertex3, Vertex4 );
		}
	}
}
