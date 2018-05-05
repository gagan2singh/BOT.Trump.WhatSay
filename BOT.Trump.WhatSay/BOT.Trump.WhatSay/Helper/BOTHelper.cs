using BOT.Trump.WhatSay.Model;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace BOT.Trump.WhatSay.Helper
{
    public static class BOTHelper
    {
        static List<Tweet> trumpTweets = new List<Tweet>();

        public static List<Tweet> TrumpTweets { get => trumpTweets; set => trumpTweets = value; }

        public static void LoadTweetData()
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnection"].ToString());
            var myClient = storageAccount.CreateCloudBlobClient();
            var container = myClient.GetContainerReference("trump-bot");
            container.CreateIfNotExists(BlobContainerPublicAccessType.Blob);

            //lines modified
            var blockBlob = container.GetBlockBlobReference("Trump Twitter Archive.csv");
            //Load each one from the file
            StreamReader sr = new StreamReader(blockBlob.OpenRead());
            while (!sr.EndOfStream)
            {
                string tweetLine = string.Empty;
                tweetLine = sr.ReadLine();
                try
                {
                    Tweet tweet = new Tweet
                    {
                        source = tweetLine.Split(',')[0],
                        text = tweetLine.Split(',')[1],
                        created_at = tweetLine.Split(',')[2],
                        retweet_count = tweetLine.Split(',')[3],
                        favorite_count = tweetLine.Split(',')[4],
                        is_retweet = tweetLine.Split(',')[5],
                        id_str = tweetLine.Split(',')[6]
                    };

                    TrumpTweets.Add(tweet);
                }
                catch (Exception )
                {
                    // Ignore exception
                }
            }
        }

        /// <summary>
        /// Return the tweets
        /// </summary>
        /// <param name="userMessage">User Message</param>
        public static List<Tweet> ReturnTrumpTweet(string userMessage)
        {
            List<Tweet> tweets = new List<Tweet>();
            //Reset relevance to zero for each new search
            foreach (Tweet tweetentry in TrumpTweets)
            {
                tweetentry.RelevanceScore = 0;
            }

            // Update the relevance for each entered code
            for (int i = 0; i < userMessage.Split(' ').Count(); i++)
            {
                foreach (Tweet tweetentry in TrumpTweets)
                {
                    if (tweetentry.text.ToLower().Contains(userMessage.ToLower().Split(' ')[i]))
                    {
                        tweetentry.RelevanceScore++;
                    }
                }
            }            

            // Get the relevant data
            List<Tweet> topFiveSortedTweets = TrumpTweets.OrderByDescending(o => o.RelevanceScore).Take(5).ToList(); ;

            return topFiveSortedTweets;
        }
    }



}