// OBJFile.cs
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
using System.IO;
using System.Collections.Generic;

namespace SalmonViewer
{
	public class ObjFile
	{
		public ObjFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("fileName");
			}
			
			if (!File.Exists(fileName))
			{
				throw new ArgumentException("3ds file could not be found", "fileName");
			}

			Model model = new Model();
			Entity entity = new Entity();
			model.Entities.Add(entity);
			
			List<Vector> vectors = new List<Vector>();
			List<Vector> normals = new List<Vector>();

			List<Quad> quads = new List<Quad>();
			List<Triangle> tris = new List<Triangle>();

			using (StreamReader sr = File.OpenText(fileName))
			{
				
				int curLineNo = 0;						
				
				string line = null;
				bool done = false;
				while ((line = sr.ReadLine()) != null)
				{
					curLineNo++;
					
					if (done || line.Trim() == string.Empty ||  line.StartsWith("#"))
					{
						continue;
					}
					
					string[] parts = line.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
					
					switch (parts[0])
					{
					case "v":
						// this is a vertex 
						vectors.Add(ParseVector(parts));
						break;
					case "vn":
						normals.Add(ParseVector(parts));
						break;
					//case "g":
				//		done = true;
				//		break;
					case "f":
						// a face
						               
						
						if (parts.Length > 5)
						{
							throw new NotSupportedException( string.Format("Face found with more than four indices (line {0})", curLineNo));
						}
						
						if (parts.Length < 3)
						{
							throw new FormatException(string.Format("Face found with less three indices (line {0})", curLineNo));							
						}
						
						//Console.WriteLine(line);
						
						// apparently we cannot make the assumption that all faces are of the same number of vertices.
						
						if (parts.Length == 4)
						{
							tris.Add(new Triangle(ParseFacePart(parts[1]), ParseFacePart(parts[2]), ParseFacePart(parts[3])));
						}
						else
						{
							quads.Add(new Quad(ParseFacePart(parts[1]), ParseFacePart(parts[2]), ParseFacePart(parts[3]), ParseFacePart(parts[4])));
			
						}
						
						break;
					}
					
				}
									Console.WriteLine("v: {0} n: {1} q:{2}", vectors.Count,normals.Count, quads.Count);
				
			}
				
		}
		
		static Vector ParseVector(string[] parts)
		{			
			return new Vector( Double.Parse(parts[1]), Double.Parse(parts[2]), Double.Parse(parts[3]));
		}                     
		
				
		static int ParseFacePart(string part)
		{
			string[] pieces = part.Split('/');
			return int.Parse( pieces[0]);
		}
	}
}
