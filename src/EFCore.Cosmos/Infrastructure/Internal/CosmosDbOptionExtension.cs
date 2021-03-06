// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using Azure.Cosmos;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Cosmos.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Cosmos.Infrastructure.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class CosmosOptionsExtension : IDbContextOptionsExtension
    {
        private string _accountEndpoint;
        private string _accountKey;
        private string _connectionString;
        private string _databaseName;
        private string _region;
        private ConnectionMode? _connectionMode;
        private bool? _limitToEndpoint;
        private Func<ExecutionStrategyDependencies, IExecutionStrategy> _executionStrategyFactory;
        private IWebProxy _webProxy;
        private TimeSpan? _requestTimeout;
        private TimeSpan? _openTcpConnectionTimeout;
        private TimeSpan? _idleTcpConnectionTimeout;
        private int? _gatewayModeMaxConnectionLimit;
        private int? _maxTcpConnectionsPerEndpoint;
        private int? _maxRequestsPerTcpConnection;
        private DbContextOptionsExtensionInfo _info;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public CosmosOptionsExtension()
        {
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected CosmosOptionsExtension([NotNull] CosmosOptionsExtension copyFrom)
        {
            _accountEndpoint = copyFrom._accountEndpoint;
            _accountKey = copyFrom._accountKey;
            _databaseName = copyFrom._databaseName;
            _connectionString = copyFrom._connectionString;
            _region = copyFrom._region;
            _connectionMode = copyFrom._connectionMode;
            _limitToEndpoint = copyFrom._limitToEndpoint;
            _executionStrategyFactory = copyFrom._executionStrategyFactory;
            _webProxy = copyFrom._webProxy;
            _requestTimeout = copyFrom._requestTimeout;
            _openTcpConnectionTimeout = copyFrom._openTcpConnectionTimeout;
            _idleTcpConnectionTimeout = copyFrom._idleTcpConnectionTimeout;
            _gatewayModeMaxConnectionLimit = copyFrom._gatewayModeMaxConnectionLimit;
            _maxTcpConnectionsPerEndpoint = copyFrom._maxTcpConnectionsPerEndpoint;
            _maxRequestsPerTcpConnection = copyFrom._maxRequestsPerTcpConnection;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string AccountEndpoint => _accountEndpoint;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithAccountEndpoint([NotNull] string accountEndpoint)
        {
            if (_connectionString != null)
            {
                throw new InvalidOperationException(CosmosStrings.ConnectionStringConflictingConfiguration);
            }

            var clone = Clone();

            clone._accountEndpoint = accountEndpoint;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string AccountKey => _accountKey;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithAccountKey([NotNull] string accountKey)
        {
            if (_connectionString != null)
            {
                throw new InvalidOperationException(CosmosStrings.ConnectionStringConflictingConfiguration);
            }

            var clone = Clone();

            clone._accountKey = accountKey;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string ConnectionString => _connectionString;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithConnectionString([NotNull] string connectionString)
        {
            if (_accountEndpoint != null || _accountKey != null)
            {
                throw new InvalidOperationException(CosmosStrings.ConnectionStringConflictingConfiguration);
            }

            var clone = Clone();

            clone._connectionString = connectionString;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string DatabaseName => _databaseName;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithDatabaseName([NotNull] string database)
        {
            var clone = Clone();

            clone._databaseName = database;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string Region => _region;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithRegion([NotNull] string region)
        {
            var clone = Clone();

            clone._region = region;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual bool? LimitToEndpoint => _limitToEndpoint;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithLimitToEndpoint(bool enable)
        {
            var clone = Clone();

            clone._limitToEndpoint = enable;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual ConnectionMode? ConnectionMode => _connectionMode;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithConnectionMode(ConnectionMode connectionMode)
        {
            if (!Enum.IsDefined(typeof(ConnectionMode), connectionMode))
            {
                throw new ArgumentOutOfRangeException(nameof(connectionMode));
            }

            var clone = Clone();

            clone._connectionMode = connectionMode;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IWebProxy WebProxy => _webProxy;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithWebProxy([NotNull] IWebProxy proxy)
        {
            var clone = Clone();

            clone._webProxy = proxy;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual TimeSpan? RequestTimeout => _requestTimeout;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithRequestTimeout(TimeSpan timeout)
        {
            var clone = Clone();

            clone._requestTimeout = timeout;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual TimeSpan? OpenTcpConnectionTimeout => _openTcpConnectionTimeout;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithOpenTcpConnectionTimeout(TimeSpan timeout)
        {
            var clone = Clone();

            clone._openTcpConnectionTimeout = timeout;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual TimeSpan? IdleTcpConnectionTimeout => _idleTcpConnectionTimeout;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithIdleTcpConnectionTimeout(TimeSpan timeout)
        {
            var clone = Clone();

            clone._idleTcpConnectionTimeout = timeout;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual int? GatewayModeMaxConnectionLimit => _gatewayModeMaxConnectionLimit;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithGatewayModeMaxConnectionLimit(int connectionLimit)
        {
            var clone = Clone();

            clone._gatewayModeMaxConnectionLimit = connectionLimit;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual int? MaxTcpConnectionsPerEndpoint => _maxTcpConnectionsPerEndpoint;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithMaxTcpConnectionsPerEndpoint(int connectionLimit)
        {
            var clone = Clone();

            clone._maxTcpConnectionsPerEndpoint = connectionLimit;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual int? MaxRequestsPerTcpConnection => _maxRequestsPerTcpConnection;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual CosmosOptionsExtension WithMaxRequestsPerTcpConnection(int requestLimit)
        {
            var clone = Clone();

            clone._maxRequestsPerTcpConnection = requestLimit;

            return clone;
        }

        /// <summary>
        ///     A factory for creating the default <see cref="IExecutionStrategy" />, or <see langword="null" /> if none has been
        ///     configured.
        /// </summary>
        public virtual Func<ExecutionStrategyDependencies, IExecutionStrategy> ExecutionStrategyFactory => _executionStrategyFactory;

        /// <summary>
        ///     Creates a new instance with all options the same as for this instance, but with the given option changed.
        ///     It is unusual to call this method directly. Instead use <see cref="DbContextOptionsBuilder" />.
        /// </summary>
        /// <param name="executionStrategyFactory"> The option to change. </param>
        /// <returns> A new instance with the option changed. </returns>
        public virtual CosmosOptionsExtension WithExecutionStrategyFactory(
            [CanBeNull] Func<ExecutionStrategyDependencies, IExecutionStrategy> executionStrategyFactory)
        {
            var clone = Clone();

            clone._executionStrategyFactory = executionStrategyFactory;

            return clone;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        protected virtual CosmosOptionsExtension Clone() => new CosmosOptionsExtension(this);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void ApplyServices(IServiceCollection services)
            => services.AddEntityFrameworkCosmos();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual void Validate(IDbContextOptions options)
        {
        }

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private string _logFragment;
            private long? _serviceProviderHash;

            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            private new CosmosOptionsExtension Extension
                => (CosmosOptionsExtension)base.Extension;

            public override bool IsDatabaseProvider => true;

            public override long GetServiceProviderHashCode()
            {
                if (_serviceProviderHash == null)
                {
                    long hashCode;
                    if (!string.IsNullOrEmpty(Extension._connectionString))
                    {
                        hashCode = Extension._connectionString.GetHashCode();
                    }
                    else
                    {
                        hashCode = Extension._accountEndpoint.GetHashCode();
                        hashCode = (hashCode * 397) ^ Extension._accountKey.GetHashCode();
                    }

                    hashCode = (hashCode * 397) ^ (Extension._region?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 3) ^ (Extension._connectionMode?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 3) ^ (Extension._limitToEndpoint?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (Extension._webProxy?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (Extension._requestTimeout?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (Extension._openTcpConnectionTimeout?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (Extension._idleTcpConnectionTimeout?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 131) ^ (Extension._gatewayModeMaxConnectionLimit?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 397) ^ (Extension._maxTcpConnectionsPerEndpoint?.GetHashCode() ?? 0);
                    hashCode = (hashCode * 131) ^ (Extension._maxRequestsPerTcpConnection?.GetHashCode() ?? 0);
                    _serviceProviderHash = hashCode;
                }

                return _serviceProviderHash.Value;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                Check.NotNull(debugInfo, nameof(debugInfo));

                if (!string.IsNullOrEmpty(Extension._connectionString))
                {
                    debugInfo["Cosmos:" + nameof(ConnectionString)] = Extension._connectionString.GetHashCode().ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    debugInfo["Cosmos:" + nameof(AccountEndpoint)] =
                    Extension._accountEndpoint.GetHashCode().ToString(CultureInfo.InvariantCulture);
                    debugInfo["Cosmos:" + nameof(AccountKey)] = Extension._accountKey.GetHashCode().ToString(CultureInfo.InvariantCulture);
                }

                debugInfo["Cosmos:" + nameof(CosmosDbContextOptionsBuilder.Region)] =
                    (Extension._region?.GetHashCode() ?? 0).ToString(CultureInfo.InvariantCulture);
            }

            public override string LogFragment
            {
                get
                {
                    if (_logFragment == null)
                    {
                        var builder = new StringBuilder();

                        builder.Append("ServiceEndPoint=").Append(Extension._accountEndpoint).Append(' ');

                        builder.Append("Database=").Append(Extension._databaseName).Append(' ');

                        _logFragment = builder.ToString();
                    }

                    return _logFragment;
                }
            }
        }
    }
}
