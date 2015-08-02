using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CSGoTrainer
{
	class Program
	{
		private static bool _exit;
		//private static double _speedDefaultValue;
		private static bool _defaultHealthEnter;
		private static ProcessMemoryHandler _memoryHandler;

		private static void Main(string[] args)
		{
			int defaultHeight = Console.WindowHeight;
			Console.WindowHeight = 35;
			Console.CursorVisible = false;
			Console.Write(" " +
			              "                                                                               \n" +
			              "                                   ╦                                           \n" +
			              "                                  ▐▌$   ╔@╣▓▓▄╖,                               \n" +
			              "                                 ,█▓▓▄▓▓▓▓▒▓███▌▒▒▒$                           \n" +
			              "                                ▄█▓█████▓▒▓█████▒▒▒µ                           \n" +
			              "                               ▄████████▌▒█████▓▓▓▒╣                           \n" +
			              "                               ▀█████▓▓▓▒▒▒▓▓▒▓▓▓▓Ü▒`                          \n" +
			              "                               ▐███▓▓▓▌▒▒▒╫▄▓▓▓▓██Ü ░                          \n" +
			              "                               ▐▓█▓█▓▓▓╣▒▒╢▒▓▓▓▒█▀                             \n" +
			              "                               ▓████▓█▓▒▄╬╢╣▓▓█╣▓C                             \n" +
			              "                               █████▓█▓╫█▌╫▓▓█▓▓▓                              \n" +
			              "                               ███████▓▓▓▓▓███▓▓▌                              \n" +
			              "                ╓╗╖            █████████▓█▓╬▓▓▓▓▓                              \n" +
			              "               ▐▓▓▓▓       ,╓▄▓▓████████▓▒▒▓▓▓▓▓▓L                             \n" +
			              "                ▓▓▓▓µ ,╖@╫▓▓▓▓██████▓▓▓▓╢╢▓▓▓▓▓▓▓▓@,                           \n" +
			              "                ▓▓▓▓▓@╣╢▒▒▒▓▓▓█████████▓▓█▓█▓▓▓▓▓▓▓▓▓▄,                        \n" +
			              "               ▄▓▓▓▓▓╣╢▓▓▓▓▓███▓███████████▓▓▓▓▓▓▓▓▓▓▓▓▓▄                      \n" +
			              "              ████▓▓▓▓▓▒▒▒▒▒██████▓██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▄                    \n" +
			              "             ]██████▓▓▓▓▓▓▓▓▓██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓g                  \n" +
			              "             ╫██████▓▓▓▓▓╢╢╣▓█████▓▓▓▓▓▓▓▓▓▓▓▓▓╣▓▓▓▓▓█▓▓▓▓▓▓▓▓╖                \n" +
			              "             ▓▓▓█████▓▓▓▓▓▄███████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓                \n" +
			              "              ▓▓▓█████▓▓▓█████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▓▓▓▓               \n" +
			              "               ╙▀█████████████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓▓               \n" +
			              "                   Y▓██████████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓████▓▓▓▓▓▓▓▓▓Ü              \n" +
			              "                   ,███████████▓▓▓█▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓▓▓U              \n" +
			              "                   █████▓██████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓▓▓▓               \n" +
			              "                   ███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓               \n" +
			              "                   ▓███▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓█▓▓▓▓▓▓▓█▓▓▀               \n" +
			              "                  ]▓▓██▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓██▓▓▓▓▓▓▓█▓▓                \n" +
			              "                  ▓████▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓███▓▓▓▓▓▓▓▓▓▓▓▓▓                  \n" +
			              "                  ╙▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀                   \n" +
			              "                     - THE OFFICIAL ALF SEAL OF APPROVAL -\n" + 
						 "                          developed by Simon Sessingø ");

			// Let people enjoy the title screen
			Thread.Sleep(4000);

			Console.Clear();

			Config.DefaultHealth = 999;

			Console.WindowHeight = defaultHeight;

			// Create new thread that listens for key inputs

			var keyboardThread = new Thread(StartKeyboardListener) {IsBackground = true};
			keyboardThread.Start();

			_memoryHandler = new ProcessMemoryHandler(Process.GetProcessesByName("csgo").FirstOrDefault());
			_memoryHandler.Open();

			// Create new thread that hacks the game
			var trainerThread = new Thread(GameTrainer) {IsBackground = true};
			trainerThread.Start();

			while (!_exit)
			{
				StringBuilder sf = new StringBuilder();
				sf.AppendLine("");
				sf.AppendLine("  [ CS-GO TRAINER ]  ");
				sf.AppendLine("");
				if (!_memoryHandler.Alive)
				{
					sf.AppendLine("");
					sf.AppendLine("  Please launch CS-GO...");
					_memoryHandler.Process = Process.GetProcessesByName("csgo").FirstOrDefault();
					_memoryHandler.Open();
				}
				else
				{
					sf.AppendLine("  HP: " + Config.CurrentHealth);
					sf.AppendLine("");

					sf.AppendLine(String.Format("  KEY 1: {0} CUSTOM HEALTH (default set to: {1})", ((Config.EnableHealthHack) ? "Disable" : "Enables"), Config.DefaultHealth));

					if (Config.EnableGodMode)
					{
						sf.AppendLine("  KEY 2: Disables GOD MODE");
					}
					else
					{
						sf.AppendLine("  KEY 2: Enables GOD MODE");
					}

					if (_defaultHealthEnter)
					{
						Console.WriteLine("");
						Console.Write("  Enter custom health [1-999]: ");
						int health;
						if (int.TryParse(Console.ReadLine(), out health))
						{
							if (health > 0 && health <= 999)
							{
								_defaultHealthEnter = false;
								Config.EnableHealthHack = true;
								Config.DefaultHealth = health;
							}
						}
					}

				}

				Console.Clear();
				
				Console.WriteLine(sf);
				Thread.Sleep(200);
			}

		}

		private static void GameTrainer()
		{
			while (!_exit)
			{
				if (_memoryHandler.Alive)
				{

					try
					{
						IntPtr moduleBaseaddress = IntPtr.Zero;

						foreach (ProcessModule module in _memoryHandler.Process.Modules)
						{
							if (module.ModuleName.Contains("engine.dll"))
							{
								moduleBaseaddress = module.BaseAddress;
								break;
							}
						}

						if (moduleBaseaddress != IntPtr.Zero)
						{
							var healthAddress = _memoryHandler.ReadMultiLevelPointer((int) moduleBaseaddress + 0x005D7140, 4,
								new int[] {0x18, 0x44, 0x4a4, 0x314, 0x214});

							var healthResult = _memoryHandler.ReadByteArray(healthAddress, 4);

							if (healthResult.Length > 0)
							{
								Config.CurrentHealth = BitConverter.ToInt32(healthResult, 0);
							}

							if (Config.CurrentHealth > 0)
							{

								if (Config.EnableGodMode || (Config.EnableHealthHack && Config.CurrentHealth == 100))
								{
									_memoryHandler.WriteInt32(healthAddress, 999);
								}

							}

						}
					}
					catch (Exception)
					{
						// do nothing
					}

				}
			}
		}

		private static void StartKeyboardListener()
		{
			while (!_exit)
			{
				ConsoleKeyInfo key = Console.ReadKey(true);

				switch (key.Key)
				{
					case ConsoleKey.D1:
						if (!Config.EnableHealthHack)
						{
							_defaultHealthEnter = true;
						}
						Config.EnableHealthHack = (!Config.EnableHealthHack);
						break;
					case ConsoleKey.D2:
						Config.EnableGodMode = (!Config.EnableGodMode);
						break;
					case ConsoleKey.Escape:
						_exit = true;
						break;
				}
			}
		}
	}
}
