using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace multitenant_aad_federation.owin_integration.Helpers
{
    public class MetadataHelper
    {
        public static async Task<string> GetMetadataDocumentAsync(string address, CancellationToken cancel)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("address");

            var httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(address, cancel).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
