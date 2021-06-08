using System;
using Microsoft.Extensions.Configuration;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting.Display;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.Loki;
using Serilog.Sinks.Http.Loki.BatchFormatters;
using Serilog.Sinks.Http.Loki.HttpClients;
using Serilog.Sinks.Http.Loki.Labels;

namespace Serilog
{
    /// <summary>
    /// Class containing extension methods to <see cref="LoggerConfiguration"/>, configuring sinks
    /// sending log events over the network using HTTP to Loki.
    /// </summary>
    public static class LoggerHttpSinkConfigurationExtensions
    {
        /// <summary>
        /// Default Template
        /// </summary>
        public const string DefaultTemplate = "{Message:lj}{NewLine}{Exception}";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="serverUrl"></param>
        /// <param name="pushDataPath"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="queueLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration HttpLoki(
            this LoggerSinkConfiguration sinkConfiguration,
            string serverUrl,
            string pushDataPath = LokiCredentials.PushDataPath,
            int batchPostingLimit = 1000,
            int? queueLimit = null,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
            => sinkConfiguration.HttpLoki(
                new NoAuthCredentials(serverUrl, pushDataPath),
                batchPostingLimit,
                queueLimit,
                period,
                restrictedToMinimumLevel,
                httpClient,
                outputTemplate,
                formatProvider,
                logLabelProvider,
                configuration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="serverUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="pushDataPath"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="queueLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration HttpLoki(
            this LoggerSinkConfiguration sinkConfiguration,
            string serverUrl,
            string username,
            string password,
            string pushDataPath = LokiCredentials.PushDataPath,
            int batchPostingLimit = 1000,
            int? queueLimit = null,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
            => sinkConfiguration.HttpLoki(
                new BasicAuthCredentials(serverUrl, username, password, pushDataPath),
                batchPostingLimit,
                queueLimit,
                period,
                restrictedToMinimumLevel,
                httpClient,
                outputTemplate,
                formatProvider,
                logLabelProvider,
                configuration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="credentials"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="queueLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration HttpLoki(
            this LoggerSinkConfiguration sinkConfiguration,
            LokiCredentials credentials,
            int batchPostingLimit = 1000,
            int? queueLimit = null,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
        {
            httpClient ??= new DefaultLokiHttpClient();
            httpClient.SetAuthCredentials(credentials);
            var batchFormatter = GetLokiBatchFormatter(credentials, logLabelProvider);
            var textFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Http(
                credentials.Url,
                batchPostingLimit,
                queueLimit,
                period,
                textFormatter,
                batchFormatter,
                restrictedToMinimumLevel,
                httpClient,
                configuration);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="serverUrl"></param>
        /// <param name="pushDataPath"></param>
        /// <param name="bufferBaseFileName"></param>
        /// <param name="bufferFileSizeLimitBytes"></param>
        /// <param name="bufferFileShared"></param>
        /// <param name="retainedBufferFileCountLimit"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration DurableHttpLokiUsingFileSizeRolledBuffers(
            this LoggerSinkConfiguration sinkConfiguration,
            string serverUrl,
            string pushDataPath = LokiCredentials.PushDataPath,
            string bufferBaseFileName = "Buffer-{Date}.json",
            long? bufferFileSizeLimitBytes = ByteSize.GB,
            bool bufferFileShared = false,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
            => sinkConfiguration.DurableHttpLokiUsingFileSizeRolledBuffers(
                new NoAuthCredentials(serverUrl, pushDataPath),
                bufferBaseFileName,
                bufferFileSizeLimitBytes,
                bufferFileShared,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period,
                restrictedToMinimumLevel,
                httpClient,
                outputTemplate,
                formatProvider,
                logLabelProvider,
                configuration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="serverUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="pushDataPath"></param>
        /// <param name="bufferBaseFileName"></param>
        /// <param name="bufferFileSizeLimitBytes"></param>
        /// <param name="bufferFileShared"></param>
        /// <param name="retainedBufferFileCountLimit"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration DurableHttpLokiUsingFileSizeRolledBuffers(
            this LoggerSinkConfiguration sinkConfiguration,
            string serverUrl,
            string username,
            string password,
            string pushDataPath = LokiCredentials.PushDataPath,
            string bufferBaseFileName = "Buffer-{Date}.json",
            long? bufferFileSizeLimitBytes = ByteSize.GB,
            bool bufferFileShared = false,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
            => sinkConfiguration.DurableHttpLokiUsingFileSizeRolledBuffers(
                new BasicAuthCredentials(serverUrl, username, password, pushDataPath),
                bufferBaseFileName,
                bufferFileSizeLimitBytes,
                bufferFileShared,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period,
                restrictedToMinimumLevel,
                httpClient,
                outputTemplate,
                formatProvider,
                logLabelProvider,
                configuration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="credentials"></param>
        /// <param name="bufferBaseFileName"></param>
        /// <param name="bufferFileSizeLimitBytes"></param>
        /// <param name="bufferFileShared"></param>
        /// <param name="retainedBufferFileCountLimit"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration DurableHttpLokiUsingFileSizeRolledBuffers(
            this LoggerSinkConfiguration sinkConfiguration,
            LokiCredentials credentials,
            string bufferBaseFileName = "Buffer",
            long? bufferFileSizeLimitBytes = ByteSize.GB,
            bool bufferFileShared = false,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
        {
            httpClient ??= new DefaultLokiHttpClient();
            httpClient.SetAuthCredentials(credentials);
            var batchFormatter = GetLokiBatchFormatter(credentials, logLabelProvider);
            var textFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.DurableHttpUsingFileSizeRolledBuffers(
                credentials.Url,
                bufferBaseFileName,
                bufferFileSizeLimitBytes,
                bufferFileShared,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period,
                textFormatter,
                batchFormatter,
                restrictedToMinimumLevel,
                httpClient,
                configuration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="serverUrl"></param>
        /// <param name="pushDataPath"></param>
        /// <param name="bufferPathFormat"></param>
        /// <param name="bufferFileSizeLimitBytes"></param>
        /// <param name="bufferFileShared"></param>
        /// <param name="retainedBufferFileCountLimit"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration DurableHttpLokiUsingTimeRolledBuffers(
            this LoggerSinkConfiguration sinkConfiguration,
            string serverUrl,
            string pushDataPath = LokiCredentials.PushDataPath,
            string bufferPathFormat = "Buffer-{Date}.json",
            long? bufferFileSizeLimitBytes = ByteSize.GB,
            bool bufferFileShared = false,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
            => sinkConfiguration.DurableHttpLokiUsingTimeRolledBuffers(
                new NoAuthCredentials(serverUrl, pushDataPath),
                bufferPathFormat,
                bufferFileSizeLimitBytes,
                bufferFileShared,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period,
                restrictedToMinimumLevel,
                httpClient,
                outputTemplate,
                formatProvider,
                logLabelProvider,
                configuration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="serverUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="pushDataPath"></param>
        /// <param name="bufferPathFormat"></param>
        /// <param name="bufferFileSizeLimitBytes"></param>
        /// <param name="bufferFileShared"></param>
        /// <param name="retainedBufferFileCountLimit"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration DurableHttpLokiUsingTimeRolledBuffers(
            this LoggerSinkConfiguration sinkConfiguration,
            string serverUrl,
            string username,
            string password,
            string pushDataPath = LokiCredentials.PushDataPath,
            string bufferPathFormat = "Buffer-{Date}.json",
            long? bufferFileSizeLimitBytes = ByteSize.GB,
            bool bufferFileShared = false,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
            => sinkConfiguration.DurableHttpLokiUsingTimeRolledBuffers(
                new BasicAuthCredentials(serverUrl, username, password, pushDataPath),
                bufferPathFormat,
                bufferFileSizeLimitBytes,
                bufferFileShared,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period,
                restrictedToMinimumLevel,
                httpClient,
                outputTemplate,
                formatProvider,
                logLabelProvider,
                configuration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sinkConfiguration"></param>
        /// <param name="credentials"></param>
        /// <param name="bufferPathFormat"></param>
        /// <param name="bufferFileSizeLimitBytes"></param>
        /// <param name="bufferFileShared"></param>
        /// <param name="retainedBufferFileCountLimit"></param>
        /// <param name="batchPostingLimit"></param>
        /// <param name="period"></param>
        /// <param name="restrictedToMinimumLevel"></param>
        /// <param name="httpClient"></param>
        /// <param name="outputTemplate"></param>
        /// <param name="formatProvider"></param>
        /// <param name="logLabelProvider"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static LoggerConfiguration DurableHttpLokiUsingTimeRolledBuffers(
            this LoggerSinkConfiguration sinkConfiguration,
            LokiCredentials credentials,
            string bufferPathFormat = "Buffer-{Date}.json",
            long? bufferFileSizeLimitBytes = ByteSize.GB,
            bool bufferFileShared = false,
            int? retainedBufferFileCountLimit = 31,
            int batchPostingLimit = 1000,
            TimeSpan? period = null,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            ILokiHttpClient? httpClient = null,
            string? outputTemplate = DefaultTemplate,
            IFormatProvider? formatProvider = null,
            ILogLabelProvider? logLabelProvider = null,
            IConfiguration? configuration = null
            )
        {
            httpClient ??= new DefaultLokiHttpClient();
            httpClient.SetAuthCredentials(credentials);
            var batchFormatter = GetLokiBatchFormatter(credentials, logLabelProvider);
            var textFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.DurableHttpUsingTimeRolledBuffers(
                credentials.Url,
                bufferPathFormat,
                bufferFileSizeLimitBytes,
                bufferFileShared,
                retainedBufferFileCountLimit,
                batchPostingLimit,
                period,
                textFormatter,
                batchFormatter,
                restrictedToMinimumLevel,
                httpClient,
                configuration);
        }

        private static ILokiBatchFormatter GetLokiBatchFormatter(LokiCredentials credentials, ILogLabelProvider? logLabelProvider)
        {
            logLabelProvider ??= new DefaultLogLabelProvider();
            if (credentials.Url.EndsWith(LokiCredentials.DeprecatedPushDataPath))
            {
                return new DeprecatedLokiBatchFormatter(logLabelProvider);
            }
            else
            {
                return new LokiBatchFormatter(logLabelProvider);
            }
        }
    }
}
