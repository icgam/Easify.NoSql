// This software is part of the Easify.Ef Library
// Copyright (C) 2018 Intermediate Capital Group
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 


using System;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Easify.NoSql.MongoDb.ApplicationInsights
{
    public static class TelemetryExtensions
    {
        public static void AttachTelemetry(this MongoClientSettings settings, TelemetryClient telemetryClient, TelemetryContext context)
        {
            var notTrackedCommands  = new[] {"isMaster", "buildInfo", "getLastError", "saslStart", "saslContinue"};
            var hashSet = notTrackedCommands.Select(c => c.ToLower()).ToDictionary(c => c);

            settings.ClusterConfigurator = clusterConfigurator =>
            {
                clusterConfigurator.Subscribe<CommandSucceededEvent>(e =>
                {
                    if (hashSet.ContainsKey(e.CommandName.ToLower()))
                        return;

                    var message = e.CommandName;
                    telemetryClient.TraceMongoEvents(context, message, DateTime.Now.Subtract(e.Duration), e.Duration, true);
                });

                clusterConfigurator.Subscribe<CommandFailedEvent>(e =>
                {
                    if (hashSet.ContainsKey(e.CommandName.ToLower()))
                        return;

                    var message = $"{e.CommandName} - {e.ToString()}";
                    telemetryClient.TraceMongoEvents(context, message, DateTime.Now.Subtract(e.Duration), e.Duration, false);
                });
            };
        }

        private static void TraceMongoEvents(this TelemetryClient telemetryClient, TelemetryContext context, string message, DateTimeOffset startTime, TimeSpan duration, bool success)
        {
            var dependency = new DependencyTelemetry()
            {
                Type = "MongoDB",
                Name = context.Subject,
                Data = message,
                Timestamp = startTime,
                Duration = duration,
                Success = success
            };

            dependency.Context.Operation.Id = context.CorrelationId;
            telemetryClient.TrackDependency(dependency);
        }
    }
}
