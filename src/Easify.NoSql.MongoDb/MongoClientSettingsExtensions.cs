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
using System.Net.Sockets;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace Easify.NoSql.MongoDb
{
    public static class MongoClientSettingsExtensions
    {
        internal struct KeepAliveValues
        {
            public uint OnOff { get; set; }
            public uint KeepAliveTime { get; set; }
            public uint KeepAliveInterval { get; set; }

            public byte[] ToBytes()
            {
                var bytes = new byte[12];
                Array.Copy(BitConverter.GetBytes(OnOff), 0, bytes, 0, 4);
                Array.Copy(BitConverter.GetBytes(KeepAliveTime), 0, bytes, 4, 4);
                Array.Copy(BitConverter.GetBytes(KeepAliveInterval), 0, bytes, 8, 4);
                return bytes;
            }
        }

        internal static void SocketConfigurator(Socket s)
        {
            var keepAliveValues = new KeepAliveValues()
            {
                OnOff = 1,
                KeepAliveTime = 120 * 1000,   // 120 seconds in milliseconds
                KeepAliveInterval = 10 * 1000 // 10 seconds in milliseconds
            };

            s.IOControl(IOControlCode.KeepAliveValues, keepAliveValues.ToBytes(), null);
        }

        public static MongoClientSettings WithDefaultClusterConfiguration(this MongoClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            settings.ClusterConfigurator = cb => cb.ConfigureTcp(tcp => tcp.WithDefaultSocketConfigurator());

            return settings;
        }        
        
        public static TcpStreamSettings WithDefaultSocketConfigurator(this TcpStreamSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            settings.With(socketConfigurator: (Action<Socket>)SocketConfigurator);

            return settings;
        }
    }
}