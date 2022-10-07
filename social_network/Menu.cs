using System;
using System.Collections.Generic;
using System.Linq;

namespace social_network
{
    public class Menu
    {
		private Processing processing = new Processing();
		public void ShowMenu()
		{
			char user_input = 'n';
			bool result;
			do
			{
				Console.Clear();
				result = LogIN();
				if (result)
				{
					ShowMainMenu();
				}
				else
				{
					Console.Write("Incorrect username/password! Try again? (y/n): ");
					user_input = Console.ReadLine()[0];
				}
			} while (user_input == 'y');
		}
		private bool LogIN()
		{
			Console.WriteLine("Default username and password is : abcd and 1234\n");
			Console.Write("Enter username: ");
			string username = Console.ReadLine();
			Console.Write("Enter password: ");
			string password = Console.ReadLine();		
			return processing.Log_in(username, password);
		}
		private void ShowMainMenu()
		{
			char userInput;
			do
			{				
				Console.BackgroundColor = ConsoleColor.White;
				Console.Clear();
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(processing.DrawInConsoleBox(processing.Title()));
				string menu = @"
   _   _   _   _     _   _   _   _  
  / \ / \ / \ / \   / \ / \ / \ / \ 
 ( M | a | i | n ) ( M | e | n | u )
  \_/ \_/ \_/ \_/   \_/ \_/ \_/ \_/ 

Please choose the option:
1 - Posts stream
2 - My subscribed
3 - Search
0 - Exit";
				Console.WriteLine(menu);								
				Console.Write("Enter your choice: ");
                userInput = Console.ReadLine()[0];
                MainMenuInput(userInput);
            } while (userInput != '0');
		}
		private void MainMenuInput(char userInput)
		{
			switch (userInput)
			{
				case '1':
					ShowPostsMenu();
					break;
				case '2':
					ShowSubscribedMenu();
					break;
				case '3':
					ShowSearchMenu();
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void ShowPostsMenu()
		{
			char userInput;
			Console.Clear();
			var posts = processing.Scroll_Posts();
			int index = 0;
			bool next_post = true;
			do
			{
				if (next_post)
				{
					Console.Clear();
					Console.WriteLine(posts[index]);
					Console.WriteLine("***********************************************************");
					Console.WriteLine(@"1 - Like    2 - Comments    3 - Next post    0 - Exit");
                }
				Console.Write("Enter your choice: ");
				userInput = Console.ReadLine()[0];
				StreamMenuInput(userInput, posts[index], ref index, ref next_post);
			} while (userInput != '0' && index < posts.Count);
			if (index == posts.Count)
			{
				Console.WriteLine("\nEnd of posts stream\n Press any button...");
				Console.ReadLine();
			}
		}
		private void StreamMenuInput(char userInput, Post post, ref int index, ref bool next_post)
		{
			switch (userInput)
			{
				case '1':
					processing.LikePost(post);
					next_post = false;
					break;
				case '2':
					ShowCommentsMenu(post);
					next_post = true;
					break;
				case '3':
					index++;
					next_post = true;
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void ShowCommentsMenu(Post post)
		{
			char userInput;
			do
			{
				Console.Clear();
				foreach (Comment comment in post.Comments.OrderBy(c => c.CreationDate))
				{
					Console.WriteLine(comment);
					Console.WriteLine("***********************************************************");
				}
				Console.WriteLine("1 - Write comment    0 - Exit");
				Console.Write("Your choice >> ");
				userInput = Console.ReadLine()[0];
				CommentsMenuInput(post, userInput);
			} while (userInput != '0');
        }
		private void CommentsMenuInput(Post post, char userInput)
		{
			string userComment;
			switch (userInput)
			{
				case '1':
					Console.Write("Write your comment:  ");
					userComment = Console.ReadLine();
					processing.WriteComment(post, userComment);
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void ShowSubscribedMenu()
		{
			char userInput;
			Console.Clear();
			List<User> subscribed;
			do
			{
				subscribed = processing.GetSubscribed();
				Console.WriteLine("\nMy Subscribed:\n");
				foreach (var s in subscribed)
				{
					Console.WriteLine(s);
				}
				Console.WriteLine("1 - Unsubsribe    2 - Posts    0 - Exit");
				Console.Write("Enter your choice: ");
				userInput = Console.ReadLine()[0];
				SubscribedMenuInput(userInput);
			} while (userInput != '0');
		}
		private void SubscribedMenuInput(char userInput)
		{
			switch (userInput)
			{
				case '1':
					Unsubscribe_user();
					break;
				case '2':
					ShowSubscribedStream();
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void Unsubscribe_user()
		{
			string choice;
			bool t;
			Console.Write("Write username: ");
			choice = Console.ReadLine();
			t = processing.UnSubscribe(choice);
			if (t)
			{
				Console.WriteLine($"Unsubscribe user: {choice}");
			}
			else
			{
				Console.WriteLine("Error! Wrong username.");
			}
		}
		private void ShowSubscribedStream()
		{
			string choice;
			Console.Write("Write username: ");
			choice = Console.ReadLine();
			if (processing.GetSubscribers(choice))
			{
				var posts = processing.Scroll_Posts(choice);
				if (posts.Count == 0)
				{
					Console.WriteLine("This user hasn't posts yet ");
					return;
				}
				int index = 0;
				char userInput;
				bool next_post = true;
				do
				{
					if (next_post)
					{
						Console.Clear();
						Console.WriteLine(posts[index]);
						Console.WriteLine("1 - Like    2 - Comments    3 - Next post    0 - Exit");
					}
					Console.Write("Your choice >> ");
					userInput = Console.ReadLine()[0];
					StreamMenuInput(userInput, posts[index], ref index, ref next_post);
				} while (userInput != '0' && index < posts.Count);
				if (index == posts.Count)
				{
					Console.WriteLine("\nThat was last post\n Press any button...");
					Console.ReadLine();
				}
			}
			else
			{
				Console.WriteLine("Error! Wrong username.");
			}
		}
		private void ShowSearchMenu()
		{
			char userInput;
			Console.Clear();
			do
			{
				Console.WriteLine("1 - Search    0 - Exit");
				Console.Write("Your choice >> ");
				userInput = Console.ReadLine()[0];
                SearchMenuInput(userInput);
			} while (userInput != '0');
		}
		private void SearchMenuInput(char userInput)
		{
			switch (userInput)
			{
				case '1':
					SearchUser();
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void SearchUser()
		{
			string username;
			Console.Write("Enter username: ");
			username = Console.ReadLine();
			var found = processing.FindUser(username);
			if (found != null)
			{
				ShowUserMenu(found);
			}
			else
			{
				Console.WriteLine("Wrong username!");
			}
		}
		private void ShowUserMenu(User user)
		{
			char userInput;
			Console.Clear();
			do
			{
				Console.WriteLine("Profile:");
				Console.WriteLine(user);
				if (processing.IsSubscribed(user))
				{
					Console.WriteLine("You subscribed on this user.");
				}
				else
				{
					Console.WriteLine("You not subscribed on this user.");
				}
				Console.WriteLine("1 - Posts    2 - Subscribe/Unsubscribe    0 - Exit");
				Console.Write("Your choice >> ");
				userInput = Console.ReadLine()[0];
				UserMenuInput(userInput, user);

			} while (userInput != '0');
		}
		private void UserMenuInput(char userInput, User user)
		{
			switch (userInput)
			{
				case '1':
					ShowUserPostsStream(user);
					break;
				case '2':
					if (processing.IsSubscribed(user))
					{
						processing.UnSubscribe(user.UserName);
						Console.WriteLine($"You not subscribe on user {user.UserName}");
					}
					else
					{
						processing.Subscribe(user.UserName);
						Console.WriteLine($"You subscribe on user {user.UserName}");
					}
					break;
				case '0':
					break;
				default:
					Console.WriteLine("You typed something wrong");
					break;
			}
		}
		private void ShowUserPostsStream(User user)
		{
			char userInput;
			Console.Clear();
			var posts = processing.Scroll_Posts(user.UserName);
			if (posts.Count == 0)
			{
				Console.WriteLine("This user hasn't posts yet ");
				return;
			}
			int index = 0;
			bool next_post = true;
			do
			{
				if (next_post)
				{
					Console.Clear();
					Console.WriteLine(posts[index]);
					Console.WriteLine("1 - Like    2 - Comments    3 - Next post    0 - Exit");
				}
				Console.Write("Your choice >> ");
				userInput = Console.ReadLine()[0];
				StreamMenuInput(userInput, posts[index], ref index, ref next_post);
			} while (userInput != '0' && index < posts.Count);
			if (index == posts.Count)
			{
				Console.WriteLine("\nThat was last post \n Press any button...");
				Console.ReadLine();
			}
		}
	}
}
