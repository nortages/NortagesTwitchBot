﻿using System;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

namespace TwitchBot.Main.ExtensionsMethods
{
    public static class TwitchClientExtensions
    {
        public static void TimeoutModer(this TwitchClient twitchClient, string channel, string username,
            TimeSpan timeoutTime, string reason = "")
        {
            twitchClient.TimeoutUser(channel, username, timeoutTime, reason);
            Task.Delay(timeoutTime.Add(TimeSpan.FromSeconds(1)))
                .ContinueWith(t => twitchClient.SendMessage(channel, $"/mod {username}"));
        }

        public static void SendMessageWithDelay(this TwitchClient client, string channel, string message,
            TimeSpan delay)
        {
            Task.Delay(delay).ContinueWith(t => client.SendMessage(channel, message));
        }

        public static bool IsMeOrBroadcaster(this ChatMessage chatMessage)
        {
            return chatMessage.IsMe ||
                   chatMessage.IsBroadcaster ||
                   string.Equals(chatMessage.Username, BotService.OwnerUsername, BotService.StringComparison);
        }

        public static bool IsMe(this WhisperMessage whisperMessage)
        {
            return string.Equals(whisperMessage.Username, BotService.OwnerUsername, BotService.StringComparison) ||
                   string.Equals(whisperMessage.Username, BotService.BotUsername, BotService.StringComparison);
            ;
        }

        public static void FullConnect(this TwitchClient twitchClient)
        {
            var oSignalEvent = new ManualResetEvent(false);

            twitchClient.OnConnected += (sender, args) =>
            {
                Console.WriteLine("On connected");
                oSignalEvent.Set();
            };
            twitchClient.Connect();
    
            oSignalEvent.WaitOne();
            oSignalEvent.Reset();
        }
    }
}