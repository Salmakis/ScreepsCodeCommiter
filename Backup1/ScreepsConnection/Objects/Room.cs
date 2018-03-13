/*Copyright (c) 2018 Annika Ryll License: MIT

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace ScreepsConnection
{

	public enum TerrainTypes
	{
		Unknown = -1,
		Ground = 0,
		Wall = 1,
		Swamp = 2
	}

	public class Room
	{
		string roomName;
		public string RoomName => roomName;

		private TerrainTypes[,] terrainData;

		public void SetTerrain(string terrain)
		{
			for (int x = 0; x < 50; x++)
			{
				for (int y = 0; y < 50; y++)
				{
					switch (terrain[x + y * y])
					{
						case '0':
							terrainData[x, y] = TerrainTypes.Ground;
							break;
						case '1':
						case '3':
							terrainData[x, y] = TerrainTypes.Wall;
							break;
						case '2':
							terrainData[x, y] = TerrainTypes.Swamp;
							break;
						default:
							terrainData[x, y] = TerrainTypes.Unknown;
							break;
					}
				}
			}
		}

		public Room(string roomName)
		{
			this.roomName = roomName;
		}
	}
}
