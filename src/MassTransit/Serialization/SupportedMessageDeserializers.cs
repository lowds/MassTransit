﻿// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Monitoring.Introspection;


    public class SupportedMessageDeserializers :
        IMessageDeserializer
    {
        readonly ContentType _contentType = new ContentType("application/*");
        readonly IDictionary<string, IMessageDeserializer> _deserializers;

        public SupportedMessageDeserializers(params IMessageDeserializer[] deserializers)
        {
            _deserializers = new Dictionary<string, IMessageDeserializer>(StringComparer.OrdinalIgnoreCase);

            foreach (IMessageDeserializer deserializer in deserializers)
                AddSerializer(deserializer);
        }

        async Task IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("supportedContentTypes");
            foreach (var deserializer in _deserializers.Values)
            {
                await deserializer.Probe(scope);
            }
        }

        public ContentType ContentType
        {
            get { return _contentType; }
        }

        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            IMessageDeserializer deserializer;
            if (!TryGetSerializer(receiveContext.ContentType, out deserializer))
            {
                throw new SerializationException(
                    string.Format("No deserializer was registered for the message content type: {0}. Supported content types include {1}",
                        receiveContext.ContentType, string.Join(", ", _deserializers.Values.Select(x => x.ContentType))));
            }

            return deserializer.Deserialize(receiveContext);
        }

        bool TryGetSerializer(ContentType contentType, out IMessageDeserializer deserializer)
        {
            if (contentType == null)
                throw new ArgumentNullException("contentType");
            if (string.IsNullOrWhiteSpace(contentType.MediaType))
                throw new ArgumentException("The media type must be specified", "contentType");

            return _deserializers.TryGetValue(contentType.MediaType, out deserializer);
        }

        void AddSerializer(IMessageDeserializer deserializer)
        {
            if (deserializer == null)
                throw new ArgumentNullException("deserializer");

            string contentType = deserializer.ContentType.MediaType;

            if (_deserializers.ContainsKey(contentType))
                return;

            _deserializers[contentType] = deserializer;
        }
    }
}