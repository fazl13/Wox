using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using NLog;
using ToolGood.Words;
using Wox.Infrastructure.Logger;
using Wox.Infrastructure.UserSettings;

namespace Wox.Infrastructure
{
    public class Alphabet
    {
        private Settings _settings;
        private MemoryCache _cache;
        
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        public void Initialize()
        {
            _settings = Settings.Instance;
            NameValueCollection config = new NameValueCollection();
            config.Add("pollingInterval", "00:05:00");
            config.Add("physicalMemoryLimitPercentage", "1");
            config.Add("cacheMemoryLimitMegabytes", "30");
            _cache = new MemoryCache("AlphabetCache", config);
        }

        public string ChangeInputLayoutToEng(string content)
        {
            const string prefix = "ENG_";
            var result = _cache[prefix + content] as string;
            if (result == null)
            {
                result = content.RusToEng();

                var policy = new CacheItemPolicy
                {
                    SlidingExpiration = new TimeSpan(12, 0, 0)
                };
                _cache.Set(prefix + content, result, policy);
            }
            return result;
        }

        public string ChangeInputLayoutToRus(string content)
        {
            const string prefix = "RUS_";
            var result = _cache[prefix + content] as string;
            if (result == null)
            {
                result = content.EngToRus();

                var policy = new CacheItemPolicy
                {
                    SlidingExpiration = new TimeSpan(12, 0, 0)
                };
                _cache.Set(prefix + content, result, policy);
            }
            return result;
        }

        public string Translate(string content)
        {
            if (Regex.IsMatch(content, "[А-Яа-я]"))
            {
                var result = _cache[content] as string;
                if (result == null)
                {
                    result = content.RusToEng();
                    
                    var policy = new CacheItemPolicy();
                    policy.SlidingExpiration = new TimeSpan(12, 0, 0);
                    _cache.Set(content, result, policy);
                }
                return result;
            }

            

            if (_settings.ShouldUsePinyin)
            {
                string result = _cache[content] as string;
                if (result == null)
                {
                    if (WordsHelper.HasChinese(content))
                    {
                        result = WordsHelper.GetFirstPinyin(content);
                    }
                    else
                    {
                        result = content;
                    }
                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.SlidingExpiration = new TimeSpan(12, 0, 0);
                    _cache.Set(content, result, policy);
                }
                return result;
            }
            else
            {
                return content;
            }
        }
    }
}
