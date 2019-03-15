﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracking.ReadModel.Cache
{
    public class RedisCacheService //: ICacheService
    {
       // private readonly ISettings _settings;
        private readonly IDatabase _cache;
        private static ConnectionMultiplexer _connectionMultiplexer;

        static RedisCacheService()
        {
            var connection = "microcouriers.redis.cache.windows.net,abortConnect=false,ssl=true,password=microcouriers.redis.cache.windows.net:6380,password=UaIei+7JYi+3TinfolGRob5lIQ4HZ1Uk8IKyIcHcjfc=,ssl=True,abortConnect=False"; //ConfigurationManager.AppSettings["RedisConnection"];
            _connectionMultiplexer = ConnectionMultiplexer.Connect(connection);
        }

        public RedisCacheService()
        {
           // _settings = settings;
            _cache = _connectionMultiplexer.GetDatabase();
        }

        public bool Exists(string key)
        {
            return _cache.KeyExists(key);
        }

        public void Save(string key, string value)
        {
            var ts = TimeSpan.FromMinutes(60);
            _cache.StringSet(key, value, ts);
        }

        public string Get(string key)
        {
            return _cache.StringGet(key);
        }

        public void Remove(string key)
        {
            _cache.KeyDelete(key);
        }

        public void Clear()
        {
            var endpoints = _connectionMultiplexer.GetEndPoints(true);           
            foreach (var endpoint in endpoints)
            {
                var server = _connectionMultiplexer.GetServer(endpoint);
                server.FlushAllDatabases();
            }
        }

        public void ClearAllKeys()
        {
            var endpoints = _connectionMultiplexer.GetEndPoints();
            var server = _connectionMultiplexer.GetServer(endpoints.First());
            //FlushDatabase didn't work for me: got error admin mode not enabled error
            //server.FlushDatabase();
            var keys = server.Keys();
            foreach (var key in keys)
            {
                Console.WriteLine("Removing Key {0} from cache", key.ToString());
                _cache.KeyDelete(key);
            }
        }
    }
}