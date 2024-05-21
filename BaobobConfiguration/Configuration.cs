namespace BaobobConfiguration
{
	using BaobobCore;
	using Newtonsoft.Json.Linq;

	public struct RabbitMQConfig
	{
		public RabbitMQConfig(string host, string id, string pw,
			List<KeyValuePair<string, string>> pubsub)
		{
			this.Host = host;
			this.Id = id;
			this.Pw = pw;
			this.PubSub = pubsub;
		}

		public readonly string Host;
		public readonly string Id;
		public readonly string Pw;
		public readonly List<KeyValuePair<string, string>> PubSub;
	}

	public struct RedisConfig
	{
		public RedisConfig(string ip, int port)
		{
			this.Ip = ip;
			this.Port = port;
		}

		public readonly string Ip;
		public readonly int Port;
	}

	public struct DBConfig
	{
		public DBConfig(string account, string game)
		{
			this.Account = account;
			this.Game = game;
		}

		public readonly string Account;
		public readonly string Game;
	}

	public struct TableConfig
	{
		public TableConfig(string[] tableNames)
		{
			this.TableName = tableNames;
		}

		public readonly string[] TableName;
	}

	public struct ServerConfig
	{
		public ServerConfig(int id, string name, string address, short port)
		{
			this.Port = port;
			this.ServerId = id;
			this.address = address;
			this.name = name;
		}

		public readonly short Port;
		public readonly int ServerId;
		public readonly string name;
		public readonly string address;
	}

	public class Configuration
	{
		public static ServerConfig ServerConfig { get; set; }
		public static DBConfig DBConfig { get; set; }
		public static TableConfig TableConfig { get; set; }
		public static RedisConfig RedisConfig { get; set; }
		public static RabbitMQConfig RabbitMQConfig { get; set; }

		// config
		public bool LoadConfig(string filePath)
		{
			try
			{
				if (File.Exists(filePath))
				{
					using (var read = new StreamReader(filePath))
					{
						var jsonString = read.ReadToEnd();
						JObject json = JObject.Parse(jsonString);

						//Server Config
						JToken? serverInfo = json["server info"];
						if (serverInfo != null && serverInfo.HasValues)
						{
							var id = (int)serverInfo["serverid"]!;
							var name = (string)serverInfo["name"]!;
							var ip = (string)serverInfo["ip"]!;
							var port = (short)serverInfo["port"]!;
							var ServerConfig = new ServerConfig(id, name, ip, port);
						}
						else
						{
							Logger.Error("Failed Load Server Configuration");
							return false;
						}

						// DB Config
						JToken? database = json["database"];
						if (database != null && database.HasValues)
						{
							var account = (string)database["account"]!;
							var game = (string)database["game"]!;
							DBConfig = new DBConfig(
								account,
								game);
						}
						else
						{
							Logger.Error("Failed Load Database Configuration");
							return false;
						}

						JToken? tableData = json["table data"];
						if (tableData != null && tableData.HasValues)
						{
							var arr = tableData.ToArray();
							TableConfig = new TableConfig(Array.ConvertAll(arr, e => (string)e!)!);
						}
						else
						{
							Logger.Error("Failed Load Table Configuration");
							return false;
						}

						JToken? redisData = json["redis"];
						if (redisData != null && redisData.HasValues)
						{
							var ip = (string)redisData["ip"]!;
							var port = (int)redisData["port"]!;
							RedisConfig = new RedisConfig(ip, port);
						}
						else
						{
							Logger.Error("Failed Load Redis Configuration");
							return false;
						}

						JToken? rabbitMqData = json["rabbitmq"];
						if (rabbitMqData != null && rabbitMqData.HasValues)
						{
							var host = (string)rabbitMqData["host"]!;
							var id = (string)rabbitMqData["id"]!;
							var pw = (string)rabbitMqData["pw"]!;
							var arr = rabbitMqData["pubsub"]!.ToArray();
							List<KeyValuePair<string, string>> list = new();
							foreach (var e in arr)
							{
								var key = (string)e.First!;
								var value = (string)e.Last!;
								list.Add(new KeyValuePair<string, string>(key, value));
							}
							RabbitMQConfig = new RabbitMQConfig(host, id, pw, list);
						}
						else
						{
							Logger.Error("Failed Load RabbitMQ Configuration");
							return false;
						}
					}
				}
				else
				{
					Logger.Error("File not Exist");
					return false;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e.Message);
				return false;
			}
			return true;
		}
	}
}