// Title:	Entity.cs
// Author: 	Scott Ellington <scott.ellington@gmail.com>
//
// Copyright (C) 2006 Scott Ellington and authors
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;

using Tao.OpenGl;

namespace SalmonViewer
{
	public struct TexCoord 
	{
		public float U;
		public float V;

		public TexCoord ( float u , float v )
		{
			U = u;
			V = v;
		}
	}
	
	public class MaterialFaces
	{
		public Material Material;
		public ushort[] Faces;
	}
	
	public class Entity : IRenderable
	{
		// TODO: OO this
		// fields should be private
		// constructor with verts and faces
		// normalize in ctor
		
		#region vars
		
		int glListNo = -1;
		
		//public Material material = new Material ();
		List<MaterialFaces> materialFaces = new List<MaterialFaces>();
		
		// The stored vertices 
		public Vector[] vertices;

		// The calculated normals
		public Vector[] normals;
		
		// The indices of the triangles which point to vertices
		public Triangle[] indices;

		// The coordinates which map the texture onto the entity
		public TexCoord[] texcoords;
	
		bool normalized = false;

		public List<MaterialFaces> MaterialFaces {
					get {
						return materialFaces;
					}
				}	
		
		#endregion
		
		#region public methods
		
		/// <summary>
		/// Calculates the surface normals for all vertices.
		/// A surface normal is a vector perpendicular to the tangent plane to that surface.
		/// http://en.wikipedia.org/wiki/Surface_normal
		/// </summary>
		public void CalculateNormals ()
		{
			if ( indices == null ) return;
			
			// a normal is created for each vertex
			normals = new Vector [vertices.Length];

			// first let's create a surface normal for each triangle
			Vector[] temps = new Vector [ indices.Length ];
			for ( int ii=0 ; ii < indices.Length ; ii++ )
			{
				Triangle tr = indices [ii];
				
				Vector v1 = vertices [ tr.Vertex1 ] - vertices  [ tr.Vertex2 ];
				Vector v2 = vertices [ tr.Vertex2 ] - vertices  [ tr.Vertex3 ];

				temps [ii] = v1.CrossProduct ( v2 );
			}
			
			// then merge the triangle's vectors for each vertex
			for ( int ii = 0; ii < vertices.Length ; ii++ )
			{
				// create a new origin vector (0,0,0)
				Vector v = new Vector ();
				
				// we'll do this by looping through all of the triangles
				for ( int jj = 0; jj < indices.Length ; jj++ )
				{
					Triangle tr = indices [jj];
					
					// and check if this triangle shares this vertex
					if ( tr.Vertex1 == ii || tr.Vertex2 == ii || tr.Vertex3 == ii )
					{
						// if so, add this vector to our vector
						// which has the effect of combining the two's magnitude and direction
						// causing a smoothing effect on the entity
						v += temps [jj];
					}
				}

				// finally normalize the vector
				normals [ii] = v.Normalize ();
			}

			normalized = true;
		}

		public void Render ()
		{
			if ( indices == null ) return;
			
			// use a GlList for performance
			if (glListNo >= 0)
			{
				Gl.glCallList(glListNo);
				return;
			}
			
			// first run we create the glList and manually render our entity
			glListNo = Gl.glGenLists(1);
			Gl.glNewList(glListNo, Gl.GL_COMPILE);

			// some 3ds files have no material faces, so we just draw all the faces
			bool noMaterials = materialFaces.Count == 0;
			if (noMaterials)
			{
				MaterialFaces m = new MaterialFaces();
				m.Material = new Material();
				materialFaces.Add(m);	
			}
			
			foreach (MaterialFaces m in materialFaces)
			{
				Material material = m.Material;
			
				// set up light properties
				Gl.glMaterialfv (Gl.GL_FRONT_AND_BACK, Gl.GL_AMBIENT, material.Ambient);
				Gl.glMaterialfv (Gl.GL_FRONT_AND_BACK, Gl.GL_DIFFUSE, material.Diffuse);
				Gl.glMaterialfv (Gl.GL_FRONT_AND_BACK, Gl.GL_SPECULAR, material.Specular);
				Gl.glMaterialf (Gl.GL_FRONT_AND_BACK, Gl.GL_SHININESS, material.Shininess);
		
				// if we have a texture, bind it
				if (material.TextureId >= 0 ) 
				{
					Gl.glBindTexture ( Gl.GL_TEXTURE_2D, material.TextureId ); 
					Gl.glEnable( Gl.GL_TEXTURE_2D );
				}
	
				// Draw every triangle in the entity, it's normal, and it's material (if exists)
				Gl.glBegin ( Gl.GL_TRIANGLES);		
					
				//foreach ( Triangle tri in indices )
				for (int ii = 0; ii < (noMaterials ? indices.Length : m.Faces.Length); ii++)
				{ 
					Triangle tri = noMaterials ? indices[ii] : indices[m.Faces[ii]];
					
					// Vertex 1
					if (normalized) Gl.glNormal3d ( normals[tri.Vertex1].X, normals[tri.Vertex1].Y, normals[tri.Vertex1].Z );
					if ( material.TextureId >= 0 ) Gl.glTexCoord2f ( texcoords [ tri.Vertex1 ].U, texcoords [ tri.Vertex1 ].V);
					Gl.glVertex3d ( vertices[tri.Vertex1].X, vertices[tri.Vertex1].Y, vertices[tri.Vertex1].Z );
	
					// Vertex 2
					if (normalized) Gl.glNormal3d ( normals[tri.Vertex2].X, normals[tri.Vertex2].Y, normals[tri.Vertex2].Z );
					if ( material.TextureId >= 0 ) Gl.glTexCoord2f ( texcoords [ tri.Vertex2 ].U, texcoords [ tri.Vertex2 ].V);
					Gl.glVertex3d ( vertices[tri.Vertex2].X, vertices[tri.Vertex2].Y, vertices[tri.Vertex2].Z );
	
					// Vertex 3
					if (normalized) Gl.glNormal3d ( normals[tri.Vertex3].X, normals[tri.Vertex3].Y, normals[tri.Vertex3].Z );
					if ( material.TextureId >= 0 ) Gl.glTexCoord2f( texcoords [ tri.Vertex3 ].U, texcoords [ tri.Vertex3 ].V);
					Gl.glVertex3d ( vertices[tri.Vertex3].X, vertices[tri.Vertex3].Y, vertices[tri.Vertex3].Z );
				}
				Gl.glEnd();
			}
			
			Gl.glDisable( Gl.GL_TEXTURE_2D );
			
			Gl.glEndList();
		}	
		
		#endregion
	}
}
