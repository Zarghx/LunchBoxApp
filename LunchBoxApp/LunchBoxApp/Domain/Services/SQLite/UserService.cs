﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LunchBoxApp.Domain.Models;
using LunchBoxApp.Domain.Services.Abstract;
using LunchBoxApp.Domain.Services.SQLite;
using Newtonsoft.Json;

namespace LunchBoxApp.Domain.Services.SQLite
{
    public class UserService : SQLiteServiceBase, IUserService
    {
        private static List<User> Users = new List<User>();
        private static User LoggedInUser;

        public UserService()
        {
            Task.Run(GetJson);
            //try
            //{
            //    Users = connection.Table<User>().ToList();

            //    if (Users.Count == 0)
            //    {
            //        GenerateData();
            //        Users = connection.Table<User>().ToList();
            //    }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e);
            //}
        }

        /// <summary>
        /// Returns all existing users
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            await Task.Delay(0);
            return Users;
        }

        /// <summary>
        /// Returns a user incase it exists (username + password combination)
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User> GetLoginUser(string username, string password)
        {
            await Task.Delay(0);
            var user = Users.FirstOrDefault(u => u.UserName == username);
            if (user.UserPassword == password)
            {
                LoggedInUser = new User();
                LoggedInUser = user;
                return user;
            }
            return null;
        }

        public async Task<User> GetCurrentUser()
        {
            await Task.Delay(0);
            return LoggedInUser;
        }

        /// <summary>
        /// Saves an existing user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task SaveExistingUser(User user)
        {
            await Task.Delay(0);
            var oldUser = Users.FirstOrDefault(i => i.UserId == user.UserId);
            oldUser = user;

            //Dropping & recreating table here because "InsertOrReplace" always inserts, it isn't replacing
            connection.DropTable<User>();
            connection.CreateTable<User>();

            foreach (var _user in Users)
            {
                connection.InsertOrReplace(_user);
            }

            await PutUser(user);
        }

        /// <summary>
        /// Creates a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task CreateNewUser(User user)
        {
            await Task.Delay(0);
            Users.Add(user);
            LoggedInUser = user;
            connection.InsertOrReplace(user);
            await PostUser(user);
        }

        private async Task GetJson()
        {
            try
            {
                string url = new Constants().url + "users";

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(url));

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Users = JsonConvert.DeserializeObject<List<User>>(content);
                }
            }

            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }

        }

        public async Task PostUser(User user)
        {
            await Task.Delay(0);
            var jsonData = JsonConvert.SerializeObject(user);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    new Constants().url + "/users",
                    new StringContent(jsonData, Encoding.UTF8, "application/json"));
                Debug.WriteLine(response.ToString());
            }
        }

        public async Task PutUser(User user)
        {
            await Task.Delay(0);
            var jsonData = JsonConvert.SerializeObject(user);

            using (var client = new HttpClient())
            {
                
                var response = await client.PutAsync(
                    new Constants().url + "/users/" + user.UserId,
                    new StringContent(jsonData, Encoding.UTF8, "application/json"));
                Debug.WriteLine(response.ToString());
            }
        }

        //private void GenerateData()
        //{
        //    try
        //    {
        //        connection.Insert(new User
        //        {
        //            UserId = Guid.NewGuid(),
        //            UserName = "Rinor",
        //            UserFirstName = "Rinor",
        //            UserLastName = "Vuniqi",
        //            UserEmail = "rinor.vuniqi@hotmail.com",
        //            UserPassword = "rinor"
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e);
        //    }
        //}
    }
}