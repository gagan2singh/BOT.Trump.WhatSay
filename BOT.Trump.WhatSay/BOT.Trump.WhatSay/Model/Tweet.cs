namespace BOT.Trump.WhatSay.Model
{
    public class Tweet
    {
        public string source { get; set; }
        public string text;
        public string created_at;
        public string retweet_count;
        public string favorite_count;
        public string is_retweet;
        public string id_str;

        public int RelevanceScore;
    }
}