// Title:	ThreeDSFile.cs
// Author: 	Scott Ellington <scott.ellington@gmail.com>
//
// Copyright (C) 2006-2007 Scott Ellington and authors
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
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Drawing;

namespace SalmonViewer
{
	/// <summary>
	/// 3ds file loader.
	/// Binds materials directly into OpenGL
	/// </summary>
	public class ThreeDSFile
	{	
		#region classes
		
		class ThreeDSChunk
		{
			public ushort ID;
			public uint Length;
			public int BytesRead;

			public ThreeDSChunk ( BinaryReader reader )
			{
				// 2 byte ID
				ID = reader.ReadUInt16();

				// 4 byte length
				Length = reader.ReadUInt32 ();

				// = 6
				BytesRead = 6; 
			}
		}
		
		#endregion
		
		#region Enums		
		
		enum Groups
		{
			C_PRIMARY      = 0x4D4D,
			C_OBJECTINFO   = 0x3D3D, 
			C_VERSION      = 0x0002,
			C_EDITKEYFRAME = 0xB000,          
			C_MATERIAL     = 0xAFFF,    
			C_MATNAME      = 0xA000, 
			C_MATAMBIENT   = 0xA010,
			C_MATDIFFUSE   = 0xA020,
			C_MATSPECULAR  = 0xA030,
			C_MATSHININESS = 0xA040,
			C_MATMAP       = 0xA200,
			C_MATMAPFILE   = 0xA300,
			C_OBJECT       = 0x4000,   
			C_OBJECT_MESH  = 0x4100,
			C_OBJECT_VERTICES    = 0x4110, 
			C_OBJECT_FACES       = 0x4120,
			C_OBJECT_MATERIAL    = 0x4130,
			C_OBJECT_UV		= 0x4140
		}

		#endregion		
		
		#region Vars

		Dictionary < string, Material > materials = new Dictionary < string, Material > ();
		
		string base_dir;
		
		BinaryReader reader;
		
		double maxX, maxY, maxZ, minX, minY, minZ;
		
		int version = -1;
		
		#endregion		
		
		#region public properties
		
		Model model = new Model ();
		public Model Model {
			get {
				return model;
			}
		}
		
		public int Version {
			get {
				return version;
			}
		}

		public double MaxX {
			get {
				return maxX;
			}
		}

		public double MaxY {
			get {
				return maxY;
			}
		}

		public double MaxZ {
			get {
				return maxZ;
			}
		}

		public double MinX {
			get {
				return minX;
			}
		}

		public double MinY {
			get {
				return minY;
			}
		}

		public double MinZ {
			get {
				return minZ;
			}
		}
		
		#endregion
		
		#region Constructors		
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="file_name">
		/// A <see cref="System.String"/>.  The path to the 3ds file
		/// </param>
		public ThreeDSFile ( string file_name )
		{
			if (string.IsNullOrEmpty(file_name))
			{
				throw new ArgumentNullException("file_name");
			}
			
			if (!File.Exists(file_name))
			{
				throw new ArgumentException("3ds file could not be found", "file_name");
			}
				
			// 3ds models can use additional files which are expected in the same directory
			base_dir =  new FileInfo ( file_name ).DirectoryName + "/";
			
			maxX = maxY = maxZ = double.MinValue;
			minX = minY = minZ = double.MaxValue;
		
			FileStream file = null;
			try
			{
				// create a binary stream from this file
				file  = new FileStream(file_name, FileMode.Open, FileAccess.Read); 
				reader = new BinaryReader ( file );
				reader.BaseStream.Seek (0, SeekOrigin.Begin); 

				// 3ds files are in chunks
				// read the first one
				ThreeDSChunk chunk = new ThreeDSChunk ( reader );
				
				if ( chunk.ID != (short) Groups.C_PRIMARY )
				{
					throw new FormatException ( "Not a proper 3DS file." );
				}

				// recursively process chunks
				ProcessChunk ( chunk );
			}
			finally
			{
				// close up everything
				if (reader != null) reader.Close ();
				if (file != null) file.Close ();
			}			
		}

		#endregion		
		
		#region Helper methods
		
		void ProcessChunk ( ThreeDSChunk chunk )
		{
			// process chunks until there are none left
			while ( chunk.BytesRead < chunk.Length )
			{
				// grab a chunk
				ThreeDSChunk child = new ThreeDSChunk ( reader );

				// process based on ID
				switch ((Groups) child.ID)
				{
					case Groups.C_VERSION:

						version = reader.ReadInt32 ();
						child.BytesRead += 4;
					
						break;

					case Groups.C_OBJECTINFO:

						// not sure whats up with this chunk
						//SkipChunk ( obj_chunk );
						//child.BytesRead += obj_chunk.BytesRead;
						//ProcessChunk ( child );
					
						// blender 3ds export (others?) uses this
						// in the hierarchy of objects and materials
						// so lets process the next (child) chunk
					
						break;					

					case Groups.C_MATERIAL:

						ProcessMaterialChunk ( child );

						break;

					case Groups.C_OBJECT:

						// string name = 
						ProcessString ( child );
						
						Entity e = ProcessObjectChunk ( child );
						e.CalculateNormals ();
						model.Entities.Add ( e );

						break;

					default:

						SkipChunk ( child );
						break;

				}

				chunk.BytesRead += child.BytesRead;
				
				//Console.WriteLine ( "ID: {0} Length: {1} Read: {2}", chunk.ID.ToString("x"), chunk.Length , chunk.BytesRead );
			}
		}

		void ProcessMaterialChunk ( ThreeDSChunk chunk )
		{
			string name = string.Empty;
			Material m = new Material ();
			
			while ( chunk.BytesRead < chunk.Length )
			{
				ThreeDSChunk child = new ThreeDSChunk ( reader );
				
				switch ((Groups) child.ID)
				{
					case Groups.C_MATNAME:

						name = ProcessString ( child );
						
						break;
				
					case Groups.C_MATAMBIENT:

						m.Ambient = ProcessColorChunk ( child );
						break;
						
					case Groups.C_MATDIFFUSE:

						m.Diffuse = ProcessColorChunk ( child );
						break;
						
					case Groups.C_MATSPECULAR:

						m.Specular = ProcessColorChunk ( child );
						break;
						
					case Groups.C_MATSHININESS:

						m.Shininess = ProcessPercentageChunk ( child );

						break;
						
					case Groups.C_MATMAP:

						ProcessPercentageChunk ( child );
	
						ProcessTexMapChunk ( child , m );
						
						break;
						
					default:

						SkipChunk ( child );
						break;

				}
				chunk.BytesRead += child.BytesRead;
			}
			materials.Add ( name, m );
		}

		void ProcessTexMapChunk ( ThreeDSChunk chunk, Material m )
		{
			while ( chunk.BytesRead < chunk.Length )
			{
				ThreeDSChunk child = new ThreeDSChunk ( reader );
				switch ((Groups) child.ID)
				{
					case Groups.C_MATMAPFILE:

						string name = ProcessString ( child );
						//Console.WriteLine ( "	Texture File: {0}", name );

						// use System.Drawing to try and load this image
					
						//FileStream fStream;
						Bitmap bmp;
						try 
						{
							//fStream = new FileStream(base_dir + name, FileMode.Open, FileAccess.Read);
							bmp = new Bitmap ( base_dir + name );
						}
						catch ( Exception ex )
						{
							// couldn't find the file
							Console.WriteLine ( "	ERROR: could not load file '{0}': {1}", base_dir + name, ex.Message );
							break;
						}

						// Flip image (needed so texture is the correct way around!)
						bmp.RotateFlip(RotateFlipType.RotateNoneFlipY); 
						
						System.Drawing.Imaging.BitmapData imgData = bmp.LockBits ( new Rectangle(new Point(0, 0), bmp.Size), 
								System.Drawing.Imaging.ImageLockMode.ReadOnly,
								System.Drawing.Imaging.PixelFormat.Format32bppArgb);								
//								System.Drawing.Imaging.PixelFormat.Format24bppRgb ); 
									
						m.BindTexture ( imgData.Width, imgData.Height, imgData.Scan0 );
						
						bmp.UnlockBits(imgData);
						bmp.Dispose();
						
						/*
						BinaryReader br = new BinaryReader(fStream);

						br.ReadBytes ( 14 ); // skip file header
					
						uint offset = br.ReadUInt32 (  );
						//br.ReadBytes ( 4 ); // skip image header
						uint biWidth = br.ReadUInt32 ();
						uint biHeight = br.ReadUInt32 ();
						Console.WriteLine ( "w {0} h {1}", biWidth, biHeight );
						br.ReadBytes ( (int) offset - 12  ); // skip rest of image header
						
						byte[,,] tex = new byte [ biHeight , biWidth , 4 ];
						
						for ( int ii=0 ; ii <  biHeight ; ii++ )
						{
							for ( int jj=0 ; jj < biWidth ; jj++ )
							{
								tex [ ii, jj, 0 ] = br.ReadByte();
								tex [ ii, jj, 1 ] = br.ReadByte();
								tex [ ii, jj, 2 ] = br.ReadByte();
								tex [ ii, jj, 3 ] = 255;
								//Console.Write ( ii + " " );
							}
						}

						br.Close();
						fStream.Close();
						m.BindTexture ( (int) biWidth, (int) biHeight, tex );
						*/
					
						break;

					default:

						SkipChunk ( child );
						break;

				}
				chunk.BytesRead += child.BytesRead;
			}
		}

		float[] ProcessColorChunk ( ThreeDSChunk chunk )
		{
			ThreeDSChunk child = new ThreeDSChunk ( reader );
			float[] c = new float[] { (float) reader.ReadByte() / 256 , (float) reader.ReadByte() / 256 , (float) reader.ReadByte() / 256 };
			//Console.WriteLine ( "R {0} G {1} B {2}", c.R, c.B, c.G );
			chunk.BytesRead += (int) child.Length;	
			return c;
		}

		int ProcessPercentageChunk ( ThreeDSChunk chunk )
		{
			ThreeDSChunk child = new ThreeDSChunk ( reader );
			int per = reader.ReadUInt16 ();
			child.BytesRead += 2;
			chunk.BytesRead += child.BytesRead;
			return per;
		}

		Entity ProcessObjectChunk ( ThreeDSChunk chunk )
		{
			return ProcessObjectChunk ( chunk, new Entity() );
		}

		Entity ProcessObjectChunk ( ThreeDSChunk chunk, Entity e )
		{
			while ( chunk.BytesRead < chunk.Length )
			{
				ThreeDSChunk child = new ThreeDSChunk ( reader );

				switch ((Groups) child.ID)
				{
					case Groups.C_OBJECT_MESH:

						ProcessObjectChunk ( child , e );
						break;

					case Groups.C_OBJECT_VERTICES:

						e.vertices = ReadVertices ( child );
						break;

					case Groups.C_OBJECT_FACES:

						e.indices = ReadIndices ( child );

						if ( child.BytesRead < child.Length )
							ProcessObjectChunk ( child, e );
						break;

					case Groups.C_OBJECT_MATERIAL:

						string name2 = ProcessString ( child );
						Console.WriteLine ( "	Uses Material: {0}", name2 );

						Material mat;
						if ( materials.TryGetValue ( name2, out mat ) )
						{
							//e.material = mat;
							
							MaterialFaces m = new MaterialFaces();
							m.Material = mat;
						
							int nfaces = reader.ReadUInt16 ();
							child.BytesRead += 2;
							//Console.WriteLine ( nfaces );
							m.Faces = new UInt16[nfaces];
							
							for ( int ii=0; ii<nfaces; ii++)
							{
								//Console.Write ( reader.ReadUInt16 () + " " );
								m.Faces[ii] = reader.ReadUInt16 ();
								child.BytesRead += 2;
							}
						
							e.MaterialFaces.Add(m);
						}
						else
						{
							Console.WriteLine ( " Warning: Material '{0}' not found. ", name2 );
							//throw new Exception ( "Material not found!" );
							
							SkipChunk ( child );
						}
					
						break;

					case Groups.C_OBJECT_UV:

						int cnt = reader.ReadUInt16 ();
						child.BytesRead += 2;

						Console.WriteLine ( "	TexCoords: {0}", cnt );
						e.texcoords = new TexCoord [ cnt ];
						for ( int ii=0; ii<cnt; ii++ )
							e.texcoords [ii] = new TexCoord ( reader.ReadSingle (), reader.ReadSingle () );
						
						child.BytesRead += ( cnt * ( 4 * 2 ) );
						
						break;
						
					default:

						SkipChunk ( child );
						break;

				}
				chunk.BytesRead += child.BytesRead;
				//Console.WriteLine ( "	ID: {0} Length: {1} Read: {2}", chunk.ID.ToString("x"), chunk.Length , chunk.BytesRead );
			}
			return e;
		}

		void SkipChunk ( ThreeDSChunk chunk )
		{
			int length = (int) chunk.Length - chunk.BytesRead;
			reader.ReadBytes ( length );
			chunk.BytesRead += length;			
		}

		string ProcessString ( ThreeDSChunk chunk )
		{
			StringBuilder sb = new StringBuilder ();

			byte b = reader.ReadByte ();
			int idx = 0;
			while ( b != 0 )
			{
				sb.Append ( (char) b);
				b = reader.ReadByte ();
				idx++;
			}
			chunk.BytesRead += idx+1;

			return sb.ToString();
		}

		Vector[] ReadVertices ( ThreeDSChunk chunk )
		{
			ushort numVerts = reader.ReadUInt16 ();
			chunk.BytesRead += 2;
			Console.WriteLine ( "	Vertices: {0}", numVerts );
			Vector[] verts = new Vector[numVerts];

			for ( int ii=0; ii < verts.Length ; ii++ )
			{
				float f1 = reader.ReadSingle();
				float f2 = reader.ReadSingle();
				float f3 = reader.ReadSingle();

				Vector v = new Vector ( f1, f3, -f2 );
				
				// track the boundaries of this model
				if (v.X > maxX) maxX = v.X;
				if (v.Y > maxY) maxY = v.Y;
				if (v.Z > maxZ) maxZ = v.Z;
				
				if (v.X < minX) minX = v.X;
				if (v.Y < minY) minY = v.Y;
				if (v.Z < minZ) minZ = v.Z;
				
				verts[ii] = v;
				//Console.WriteLine ( verts [ii] );
			}

			//Console.WriteLine ( "{0}   {1}", verts.Length * ( 3 * 4 ), chunk.Length - chunk.BytesRead );

			chunk.BytesRead += verts.Length * ( 3 * 4 ) ;
			//chunk.BytesRead = (int) chunk.Length;
			//SkipChunk ( chunk );

			return verts;
		}

		Triangle[] ReadIndices ( ThreeDSChunk chunk )
		{
			ushort numIdcs = reader.ReadUInt16 ();
			chunk.BytesRead += 2;
			Console.WriteLine ( "	Indices: {0}", numIdcs );
			Triangle[] idcs = new Triangle[numIdcs];

			for ( int ii=0; ii < idcs.Length ; ii++ )
			{
				idcs [ii] = new Triangle ( reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16() );
				//Console.WriteLine ( idcs [ii] );

				// flags
				reader.ReadUInt16 ();
			}
			chunk.BytesRead += ( 2 * 4 ) * idcs.Length;
			//Console.WriteLine ( "b {0} l {1}", chunk.BytesRead, chunk.Length);

			//chunk.BytesRead = (int) chunk.Length;
			//SkipChunk ( chunk );

			return idcs;
		}

		#endregion
	}
}
