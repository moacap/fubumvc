using System.Collections.Generic;
using System.Net;
using FubuCore.Binding;
using FubuCore.Conversion;
using FubuMVC.Core.Assets.Caching;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Etags;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetEtagInvocationFilter : IBehaviorInvocationFilter
    {
        private readonly IEtagCache _cache;

        public AssetEtagInvocationFilter(IEtagCache cache)
        {
            _cache = cache;
        }

        public DoNext Filter(ServiceArguments arguments)
        {
            string etag = null;

            arguments.Get<IRequestData>().ValuesFor(RequestDataSource.Header)
                .Value(HttpRequestHeaders.IfNoneMatch, value => etag = value.RawValue as string);

            if (etag == null) return DoNext.Continue;

            var resourceHash = arguments.Get<ICurrentChain>().ResourceHash();
            var currentEtag = _cache.Current(resourceHash);

            if (etag != currentEtag) return DoNext.Continue;

            var httpWriter = arguments.Get<IHttpWriter>();
            
            httpWriter.WriteResponseCode(HttpStatusCode.NotModified);
            
            _cache.HeadersForEtag(etag).Each(x => httpWriter.AppendHeader(x.Name, x.Value));

            return DoNext.Stop;
        }
    }
}