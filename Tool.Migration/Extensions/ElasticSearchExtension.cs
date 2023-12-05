using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;
using Elasticsearch.Net;
using Nest;
using Tool.VIP.Migrator.Index;

namespace Tool.VIP.Migrator.Extensions
{
    public static class ElasticSearchExtensions
    {
        public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
        {
            var diseasesIndexName = configuration.GetSection("ElasticSearch:Indices:VehicleMonitor")?.Value;

            IConnectionPool pool = null;
            if (configuration.GetSection("ElasticSearch:Url").GetChildren().Count() > 0)
            {
                IConfigurationSection[] arrUris = null;
                if (configuration.GetSection("ElasticSearch:Url").GetChildren().Count() > 0)
                    arrUris = configuration.GetSection("ElasticSearch:Url").GetChildren().ToArray();
                else
                    arrUris = new IConfigurationSection[] {
                        configuration.GetSection("ElasticSearch:Url") };
                var uris = arrUris.Select(v => new Uri(v.Value)).ToArray();
                pool = new StaticConnectionPool(uris);
            }
            else
            {
                var uri = new Uri(configuration.GetSection("ElasticSearch:Url").Value);
                pool = new SingleNodeConnectionPool(uri);
            }
            var settings = new ConnectionSettings(pool)
                .DefaultIndex(diseasesIndexName)
                .EnableDebugMode()
                .PrettyJson()
                .RequestTimeout(TimeSpan.FromMinutes(2));

            if (!String.IsNullOrEmpty(configuration.GetSection("ElasticSearch:Username").Value))
            {
                settings.ServerCertificateValidationCallback((o, certificate, chain, errors) => true);
                settings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);
                settings.BasicAuthentication(configuration.GetSection("ElasticSearch:Username").Value,
                                            configuration.GetSection("ElasticSearch:Password").Value);
            }

            // Set defaut mapping
            settings.DefaultMappingFor<UserSettingIndex>(m => m.IndexName(configuration.GetSection("ElasticSearch:Indices:UserSetting")?.Value));
            settings.DefaultMappingFor<VehicleMonitorIndex>(m => m.IndexName(configuration.GetSection("ElasticSearch:Indices:VehicleMonitor")?.Value));

            //TODO handle later for authentication elasticsearch
            var client = new ElasticClient(settings);

            if (!client.Indices.Exists(configuration.GetSection("ElasticSearch:Indices:UserSetting")?.Value).Exists)
            {
                client.Indices.Create(configuration.GetSection("ElasticSearch:Indices:UserSetting").Value, c => c
                              .Map<UserSettingIndex>(mm => mm
                                  .AutoMap()
                                  .Properties(p => p
                                      .Keyword(k => k.Name(n => n.Id))
                                      .Text(t => t.Name(n => n.Username).Fields(f => f.Keyword(k => k.Name("username"))))
                                  )
                              ));
            }

            if (!client.Indices.Exists(configuration.GetSection("ElasticSearch:Indices:VehicleMonitor")?.Value).Exists)
            {
                client.Indices.Create(configuration.GetSection("ElasticSearch:Indices:VehicleMonitor").Value, c => c
                              .Map<VehicleMonitorIndex>(mm => mm
                                  .AutoMap()
                                  .Properties(p => p
                                      .Keyword(k => k.Name(n => n.Id))
                                      .Text(t => t.Name(n => n.Plate).Fields(f => f.Keyword(k => k.Name("plate"))))
                                      .Text(t => t.Name(n => n.ActualPlate).Fields(f => f.Keyword(k => k.Name("actualplate"))))
                                      .Text(t => t.Name(n => n.CompanyName).Fields(f => f.Keyword(k => k.Name("companyname"))))
                                  )
                              ));
            }

            services.AddSingleton<IElasticClient>(client);
        }
    }
}
