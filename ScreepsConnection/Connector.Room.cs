using ScreepsConnection;
using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScreepsConnection
{
	public partial class Connector
	{
		public async Task<Room> GetRoom(string roomName, int shardID)
		{
			var node = await RequestGet($"/api/game/room-terrain?room={roomName}&shard=shard{shardID}&encoded=1");
			try
			{
				if (node["ok"] == 1)
				{

					Room room = new Room(node["terrain"][0]["room"]);
					room.SetTerrain(node["terrain"][0]["terrain"]);
					return room;
				}
				return null;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return null;
			}
		}
	}
}
