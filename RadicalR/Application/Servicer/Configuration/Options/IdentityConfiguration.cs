namespace RadicalR
{
    public class IdentityConfiguration
    {
        public string ApiName { get; set; }

        public string ApiVersion { get; set; }

        public string BaseUrl { get; set; }

        public string ApiBaseUrl { get; set; }

        public string OidcSwaggerUIClientId { get; set; }

        public bool RequireHttpsMetadata { get; set; }

        public string OidcApiName { get; set; }

        public string[] Scopes { get; set; }

        public string[] Roles { get; set; }

        public string AdministrationRole { get; set; }

        public bool CorsAllowAnyOrigin { get; set; }

        public string[] CorsAllowOrigins { get; set; }
    }
}