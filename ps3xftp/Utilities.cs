﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ps3Xftp.Extensions;
using Ps3Xftp.Models;

namespace Ps3Xftp
{
    internal static class Utilities
    {
        /// <summary>
        ///     Web address link pointing to the project repo hosted on GitHub
        /// </summary>
        internal const string GithubProject = "https://github.com/mostlyash/ps3xftp/";
        
        /// <summary>
        ///     Gets the mods details from the database
        /// </summary>
        /// <returns></returns>
        internal static ModsData GetModsData()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("https://www.dropbox.com/s/9kzqk21hkz2nt14/modsdata.json?raw=true").Result;

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Bad response {response.StatusCode}");

                var responseData = response.Content.ReadAsStringAsync().Result;

                if (IsValidJson(responseData))
                    return JsonConvert.DeserializeObject<ModsData>(responseData);

                dynamic data = JsonConvert.DeserializeObject(responseData);

                throw new Exception(data.data.message.ToString());
            }
        }
        
        /// <summary>
        ///     Gets the game details from the database
        /// </summary>
        /// <returns></returns>
        internal static GamesData GetGameData()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("https://www.dropbox.com/s/98bp8y8ii1o7y64/gamedata.json?raw=true").Result;

                if (response.StatusCode != HttpStatusCode.OK)
                    throw new Exception($"Bad response {response.StatusCode}");

                var responseData = response.Content.ReadAsStringAsync().Result;

                if (IsValidJson(responseData))
                    return JsonConvert.DeserializeObject<GamesData>(responseData);

                dynamic data = JsonConvert.DeserializeObject(responseData);

                throw new Exception(data.data.message.ToString());
            }
        }

        /// <summary>
        ///     Uploads the specified local file to the appropriate location on the console
        /// </summary>
        /// <param name="ps3Address">PS3 IP address</param>
        /// <param name="localFile">Path of the local file</param>
        /// <param name="consoleFile">Path of the uploading file directory</param>
        internal static void FileToPs3(string ps3Address, string localFile, string consoleFile)
        {
            using (var ps3 = new Ps3Ftp(ps3Address))
            {
                if (!ps3.IsConnected)
                    throw new Exception("Unable to connect to console");

                var fileName = consoleFile.Contains("/")
                    ? consoleFile.Substring(consoleFile.LastIndexOf('/')).Replace("/", "").Replace("//", "")
                    : consoleFile;
                var dirPath = consoleFile.Contains("/")
                    ? consoleFile.Substring(0, consoleFile.LastIndexOf('/')) + '/'
                    : "dev_hdd0/";
                ps3.SetCurrentDirectory(dirPath);
                ps3.PutFile(localFile, fileName);
            }
        }

        /// <summary>
        ///     Gets the game data matching the title
        /// </summary>
        /// <param name="gameTitle">Title of the game</param>
        /// <returns>Game information</returns>
        internal static GamesData.Game GetGameByTitle(string gameTitle)
        {
            foreach (var game in Ps3Xftp.GamesData.Games)
                if (game.Title == gameTitle)
                    return game;
            throw new Exception("Unable to find game data for the specified title");
        }

        /// <summary>
        /// Start a new instance of the report mod template
        /// </summary>
        /// <param name="modItem">Mod info to fill with</param>
        internal static void OpenReportTemplate(ModsData.ModItem modItem)
        {
            Process.Start($"{GithubProject}issues/new?" +
                          $"title=[Report] {modItem.Name} (v{modItem.Version})" +
                          "&labels=mod-request&" +
                          $"body=Id: {modItem.Id}%0A" +
                          $"Game Id: {modItem.GameId}%0A" +
                          $"Author: {modItem.Author}%0A" +
                          $"Install Paths: {modItem.InstallPaths.ToList()}%0A" +
                          "----------------------- %0A" +
                          "*Please include additional information about the issue you are experiencing...");
        }

        /// <summary>
        ///     Determines a valid json response
        /// </summary>
        /// <param name="data">Json data to validate</param>
        /// <returns>Whether text is valid json format</returns>
        private static bool IsValidJson(string data)
        {
            try
            {
                var unused = JToken.Parse(data);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}