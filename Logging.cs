using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace MC_BSR_S2_Calculator {
    internal class Logging {

        // --- VARIABLES ---

        // -- STATIC --

        // - Logger Factory -

        public static ILoggerFactory AppLoggerFactory {
            get {
                if (field is null) {
                    throw new NullReferenceException("AppLoggerFactory has not yet been set");
                }
                return field;
            }
            private set => field = value;
        }

        // - Loggers -

        public static ILogger SwitchManagement {
            get {
                if (field is null) {
                    throw new NullReferenceException("SwitchManagement has not been set");
                }
                return field;
            }
            private set => field = value;
        }

        // --- CONSTRUCTOR ---

        public static void Initialize() {
            // get members from the class
            // - LogSettings is an internal static class which contains const LogLevels named via prefix of '_do'
            //   otherwise, LogSettings has nothing else inside of it. This file has been excluded from git
            //   because changes to logging don't need to be tracked
            var propertyInfos = typeof(Logging)
                .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            var fieldInfos = typeof(LogSettings)
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            // get logger members
            var loggerInfos = propertyInfos
                .Where(property => (
                    (property.PropertyType == typeof(ILogger))
                    && (property.SetMethod is not null)
                    && (property.SetMethod.IsPrivate)
                ));

            // get log levels
            var loglevelInfos = fieldInfos
                .Where(field => field.FieldType == typeof(LogLevel));

            // set logger factory
            AppLoggerFactory = LoggerFactory.Create(builder => {
#if DEBUG
                // debug
                builder.AddDebug();

                // set filtering
                foreach (var propertyInfo in loggerInfos) {
                    // find corresponding log level by name
                    FieldInfo? logLevelInfo = loglevelInfos.FirstOrDefault(logLevel => logLevel.Name == $"_do{propertyInfo.Name}");
                    if (logLevelInfo is null) { throw new NullReferenceException("log level info was not set"); }

                    // add filter
                    builder.AddFilter(propertyInfo.Name, (LogLevel)logLevelInfo.GetValue(null)!);
                }
#endif
            });

            // set logger for each logger
            foreach (var propertyInfo in loggerInfos) {
                var logger = AppLoggerFactory.CreateLogger(propertyInfo.Name);
                propertyInfo.SetValue(null, logger);
            }
        }
    }
}
