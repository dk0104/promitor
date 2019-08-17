﻿using System.Collections.Generic;
using AutoMapper;
using Microsoft.Azure.Management.Monitor.Fluent.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Promitor.Core.Scraping.Configuration.Serialization;
using Promitor.Core.Scraping.Configuration.Serialization.Enum;
using Promitor.Core.Scraping.Configuration.Serialization.v1.Model;
using Promitor.Core.Scraping.Configuration.Serialization.v1.Model.Metrics;
using Promitor.Core.Scraping.Configuration.Serialization.v1.Model.Metrics.ResourceTypes;
using Promitor.Integrations.AzureStorage;

namespace Promitor.Scraper.Tests.Unit.Builders.Metrics.v1
{
    public class MetricsDeclarationBuilder
    {
        private readonly AzureMetadataV1 _azureMetadata;
        private readonly List<MetricDefinitionV1> _metrics = new List<MetricDefinitionV1>();
        private MetricDefaultsV1 _metricDefaults = new MetricDefaultsV1
        {
            Scraping = new ScrapingV1 { Schedule = @"0 * * ? * *" }
        };

        public MetricsDeclarationBuilder(AzureMetadataV1 azureMetadata)
        {
            _azureMetadata = azureMetadata;
        }

        public static MetricsDeclarationBuilder WithMetadata(string tenantId = "tenantId", string subscriptionId = "subscriptionId", string resourceGroupName = "resourceGroupName")
        {
            var azureMetadata = new AzureMetadataV1
            {
                TenantId = tenantId,
                SubscriptionId = subscriptionId,
                ResourceGroupName = resourceGroupName
            };

            return new MetricsDeclarationBuilder(azureMetadata);
        }

        public static MetricsDeclarationBuilder WithoutMetadata()
        {
            return new MetricsDeclarationBuilder(azureMetadata: null);
        }

        public MetricsDeclarationBuilder WithDefaults(MetricDefaultsV1 defaults)
        {
            _metricDefaults = defaults;

            return this;
        }

        public string Build(IMapper mapper)
        {
            var metricsDeclaration = new MetricsDeclarationV1
            {
                Version = SpecVersion.v1.ToString(),
                AzureMetadata = _azureMetadata,
                MetricDefaults = _metricDefaults,
                Metrics = _metrics
            };

            var configurationSerializer = new ConfigurationSerializer(NullLogger.Instance, mapper);
            return configurationSerializer.Serialize(metricsDeclaration);
        }

        public MetricsDeclarationBuilder WithServiceBusMetric(string metricName = "promitor-service-bus", string metricDescription = "Description for a metric", string queueName = "promitor-queue", string serviceBusNamespace = "promitor-namespace", string azureMetricName = "Total")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new ServiceBusQueueMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                QueueName = queueName,
                Namespace = serviceBusNamespace,
                AzureMetricConfiguration = azureMetricConfiguration
            };
            _metrics.Add(metric);

            return this;
        }

        public MetricsDeclarationBuilder WithContainerInstanceMetric(string metricName = "promitor-container-instance", string metricDescription = "Description for a metric", string containerGroup = "promitor-group", string azureMetricName = "Total")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new ContainerInstanceMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                ContainerGroup = containerGroup,
                AzureMetricConfiguration = azureMetricConfiguration
            };

            _metrics.Add(metric);

            return this;
        }

        public MetricsDeclarationBuilder WithContainerRegistryMetric(string metricName = "promitor-container-registry", string metricDescription = "Description for a metric", string registryName = "promitor-container-registry", string azureMetricName = "Total")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new ContainerRegistryMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                RegistryName = registryName,
                AzureMetricConfiguration = azureMetricConfiguration
            };
            _metrics.Add(metric);

            return this;
        }

        public MetricsDeclarationBuilder WithCosmosDbMetric(string metricName = "promitor-cosmosdb", string metricDescription = "Description for a metric", string dbName = "promitor-cosmosdb", string azureMetricName = "TotalRequests")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new CosmosDbMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                DbName = dbName,
                AzureMetricConfiguration = azureMetricConfiguration
            };
            _metrics.Add(metric);

            return this;
        }

        public MetricsDeclarationBuilder WithAzureStorageQueueMetric(string metricName = "promitor", string metricDescription = "Description for a metric", string queueName = "promitor-queue", string accountName = "promitor-account", string sasToken = "?sig=promitor", string azureMetricName = AzureStorageConstants.Queues.Metrics.MessageCount)
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var secret = new SecretV1
            {
                RawValue = sasToken
            };

            var metric = new StorageQueueMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                QueueName = queueName,
                AccountName = accountName,
                SasToken = secret,
                AzureMetricConfiguration = azureMetricConfiguration
            };
            _metrics.Add(metric);

            return this;
        }

        public MetricsDeclarationBuilder WithVirtualMachineMetric(string metricName = "promitor-virtual-machine", string metricDescription = "Description for a metric", string virtualMachineName = "promitor-virtual-machine-name", string azureMetricName = "Total")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new VirtualMachineMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                VirtualMachineName = virtualMachineName,
                AzureMetricConfiguration = azureMetricConfiguration
            };

            _metrics.Add(metric);

            return this;
        }

        public MetricsDeclarationBuilder WithNetworkInterfaceMetric(string metricName = "promitor-network-interface", string metricDescription = "Description for a metric", string networkInterfaceName = "promitor-network-interface-name", string azureMetricName = "Total")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new NetworkInterfaceMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                NetworkInterfaceName = networkInterfaceName,
                AzureMetricConfiguration = azureMetricConfiguration
            };

            _metrics.Add(metric);

            return this;
        }

        public MetricsDeclarationBuilder WithGenericMetric(string metricName = "foo", string metricDescription = "Description for a metric", string resourceUri = "Microsoft.ServiceBus/namespaces/promitor-messaging", string filter = "EntityName eq \'orders\'", string azureMetricName = "Total")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new GenericAzureMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                ResourceUri = resourceUri,
                Filter = filter,
                AzureMetricConfiguration = azureMetricConfiguration
            };
            _metrics.Add(metric);

            return this;
        }

        private AzureMetricConfigurationV1 CreateAzureMetricConfiguration(string azureMetricName)
        {
            return new AzureMetricConfigurationV1
            {
                MetricName = azureMetricName,
                Aggregation = new MetricAggregationV1
                {
                    Type = AggregationType.Average
                }
            };
        }

        public MetricsDeclarationBuilder WithRedisCacheMetric(string metricName = "promitor-redis", string metricDescription = "Description for a metric", string cacheName = "promitor-redis", string azureMetricName = "CacheHits")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new RedisCacheMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                CacheName = cacheName,
                AzureMetricConfiguration = azureMetricConfiguration
            };
            _metrics.Add(metric);

            return this;
        }

        public MetricsDeclarationBuilder WithPostgreSqlMetric(string metricName = "promitor-postgresql", string metricDescription = "Description for a metric", string serverName = "promitor-postgresql", string azureMetricName = "cpu_percent")
        {
            var azureMetricConfiguration = CreateAzureMetricConfiguration(azureMetricName);
            var metric = new PostgreSqlMetricDefinitionV1
            {
                Name = metricName,
                Description = metricDescription,
                ServerName = serverName,
                AzureMetricConfiguration = azureMetricConfiguration
            };
            _metrics.Add(metric);

            return this;
        }
    }
}