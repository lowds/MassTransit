﻿// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Policies
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Monitoring.Introspection;


    public class CancelRetryPolicy :
        IRetryPolicy
    {
        readonly CancellationToken _cancellationToken;
        readonly IRetryPolicy _retryPolicy;

        public CancelRetryPolicy(IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            _retryPolicy = retryPolicy;
            _cancellationToken = cancellationToken;
        }

        Task IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("cancel");

            return _retryPolicy.Probe(scope);
        }

        public IRetryContext GetRetryContext()
        {
            return new CancelRetryContext(_retryPolicy.GetRetryContext(), _cancellationToken);
        }

        public bool CanRetry(Exception exception)
        {
            return !_cancellationToken.IsCancellationRequested && _retryPolicy.CanRetry(exception);
        }
    }
}