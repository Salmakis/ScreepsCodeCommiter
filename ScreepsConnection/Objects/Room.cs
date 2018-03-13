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
